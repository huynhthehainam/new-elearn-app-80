using eLearnApps.Core.Extension;
using eLearnApps.Core.Settings;
using eLearnApps.Data.Interface.Logging;
using eLearnApps.Data.Logging;
using eLearnApps.Entity;
using eLearnApps.Entity.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using eLearnApps.Entity.LmsTools;
using eLearnApps.Core.Domain.Users; // For HttpContext access if needed

namespace eLearnApps.Data
{
    public class DataContext : DbContext, IDbContext
    {
        // You can inject options via DI.
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    : base(options)
        {

            // In EF Core, there's no need for Database.SetInitializer.
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically apply configurations from the assembly.
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var mappingTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract) // Ensure it's a concrete class
                                                        //.Where(t => typeof(IEntityTypeConfiguration<>).IsAssignableFrom(t)) // Must implement IEntityTypeConfiguration<T>
                 .Where(t => t.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                .Where(t => t.Namespace != null && t.Namespace.EndsWith(".Mapping")).ToList(); // Namespace must end with "Mapping"

            foreach (var type in mappingTypes)
            {
                var configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration((dynamic)configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }

        // Replace the EF6 auditDao with your updated logging mechanism.
        private IAuditDao _auditDao => new AuditDao(_configuration);

        public override int SaveChanges()
        {
            // Retrieve app settings (assumed to be registered with DI) via a static helper or service locator.
            // Here we use a simplified static access pattern (e.g. ELearnAppContext.Current) as in your original code.
            //var setting = ELearnAppContext.Current.Resolve<IAppSettings>();
            var enableAuditLog = _configuration.GetValue<bool>("EnableAuditLog");
            if (enableAuditLog)
            {
                OnBeforeSaveChanges();
            }
            return base.SaveChanges();
        }

        private void OnBeforeSaveChanges()
        {
            // Ensure EF Core's change tracker has detected changes.
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            // Resolve HttpContext (adapt this as needed—EF Core doesn't provide HttpContextBase).
            var httpContext = _httpContextAccessor.HttpContext;
            LoggedInUserInfo userInfo = new LoggedInUserInfo();
            if (httpContext is not null)
            {
                userInfo = httpContext.GetLoggedInUserInfo();
            }


            foreach (var entry in ChangeTracker.Entries())
            {
                // Skip auditing for Audit entities or unchanged/detached entries.
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userInfo.UserId.ToString(),
                    OrgUnitId = userInfo.CourseId
                };

                // Loop through each property on the entity.
                foreach (var property in entry.Entity.GetType().GetTypeInfo().DeclaredProperties)
                {
                    // Skip auditing the key property (assumed to be named "Id")
                    if (property.Name == "Id")
                    {
                        var pi = entry.Entity.GetType().GetProperty(property.Name);
                        if (pi?.GetValue(entry.Entity) == null)
                            continue;
                        auditEntry.KeyValues["Id"] = pi.GetValue(entry.Entity).ToString();
                        continue;
                    }

                    var propInfo = entry.Entity.GetType().GetProperty(property.Name);
                    if (propInfo == null || propInfo.GetValue(entry.Entity) == null)
                        continue;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = Core.AuditType.Create;
                            auditEntry.NewValues[property.Name] = propInfo.GetValue(entry.Entity).ToString();
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = Core.AuditType.Delete;
                            // Get the database value for the property.
                            var dbValuesForDelete = entry.GetDatabaseValues();
                            if (dbValuesForDelete != null)
                            {
                                auditEntry.OldValues[property.Name] = dbValuesForDelete.GetValue<object>(property.Name)?.ToString();
                            }
                            break;
                        case EntityState.Modified:
                            // Get the original value.
                            var dbValues = entry.GetDatabaseValues();
                            string currentValue = dbValues?.GetValue<object>(property.Name)?.ToString() ?? "null";
                            string newValue = propInfo.GetValue(entry.Entity)?.ToString();
                            if (currentValue != newValue)
                            {
                                auditEntry.ChangedColumns.Add(property.Name);
                                auditEntry.AuditType = Core.AuditType.Update;
                                auditEntry.OldValues[property.Name] = currentValue;
                                auditEntry.NewValues[property.Name] = newValue;
                            }
                            break;
                    }
                }

                auditEntries.Add(auditEntry);
            }

            // Offload the audit insert to a background thread.
            ThreadPool.QueueUserWorkItem(_ =>
            {
                _auditDao.Insert(auditEntries.Select(x => x.ToAudit()).ToList());
            });
        }

        #region Methods

        /// <summary>
        /// Gets the underlying DatabaseFacade.
        /// </summary>
        public DatabaseFacade GetDatabase()
        {
            return Database;
        }

        /// <summary>
        /// Detaches an entity from the change tracker.
        /// </summary>
        /// <param name="entity">Entity to detach.</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// Executes a raw SQL query that returns elements of the given type.
        /// </summary>
        /// <typeparam name="TElement">The type of elements returned.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="parameters">Parameters for the query.</param>
        /// <returns>Results as an IEnumerable of TElement.</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            // EF Core: Use FromSqlRaw on a DbSet<TElement>.
            return Database.SqlQueryRaw<TElement>(sql, parameters).ToList();
        }

        /// <summary>
        /// Executes a raw SQL command.
        /// </summary>
        /// <param name="sql">SQL command string.</param>
        /// <param name="doNotEnsureTransaction">If true, executes without an explicit transaction.</param>
        /// <param name="timeout">Command timeout in seconds.</param>
        /// <param name="parameters">Command parameters.</param>
        /// <returns>Number of affected rows.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            if (timeout.HasValue)
            {
                Database.SetCommandTimeout(timeout.Value);
            }

            int result;
            if (doNotEnsureTransaction)
            {
                result = Database.ExecuteSqlRaw(sql, parameters);
            }
            else
            {
                using (var transaction = Database.BeginTransaction())
                {
                    result = Database.ExecuteSqlRaw(sql, parameters);
                    transaction.Commit();
                }
            }

            return result;
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>IDbContextTransaction instance.</returns>
        public IDbContextTransaction BeginTransaction()
        {
            // Note: In EF Core, use Database.BeginTransaction() which returns an IDbContextTransaction.
            // If you need a DbContextTransaction-like type, you may create an adapter.
            // Here, we return null to indicate that you should use Database.BeginTransaction() directly.
            return Database.BeginTransaction();
        }

        /// <summary>
        /// Gets the DbSet for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>DbSet for TEntity.</returns>
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        #endregion

        #region Removed EF6 Properties

        // The following properties have no direct counterparts in EF Core:
        // - ProxyCreationEnabled
        // - AutoDetectChangesEnabled

        #endregion
    }
}
