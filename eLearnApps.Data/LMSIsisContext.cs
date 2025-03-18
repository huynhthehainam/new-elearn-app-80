using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace eLearnApps.Data
{
    public class LMSIsisContext : DbContext, IDbContext
    {
        public LMSIsisContext(DbContextOptions<LMSIsisContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            var assembly = Assembly.GetExecutingAssembly();

            var mappingTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract) // Ensure it's a concrete class
                .Where(t => t.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                .Where(t => t.Namespace != null && t.Namespace.EndsWith(".LMSIsisMapping")); // Namespace must end with "Mapping"

            foreach (var type in mappingTypes)
            {
                var configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration((dynamic)configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }

        #region Methods

        /// <summary>
        /// Detach an entity from the change tracker.
        /// </summary>
        /// <param name="entity">Entity to detach</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// Executes a raw SQL query that returns elements of the given type.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements returned.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query.</param>
        /// <returns>Resulting elements</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            // In EF Core, use FromSqlRaw on a DbSet<TElement>.
            return Database.SqlQueryRaw<TElement>(sql, parameters).ToList();
        }

        /// <summary>
        /// Executes a raw SQL command.
        /// </summary>
        /// <param name="sql">SQL command string.</param>
        /// <param name="doNotEnsureTransaction">If true, executes without wrapping in an explicit transaction.</param>
        /// <param name="timeout">Command timeout in seconds.</param>
        /// <param name="parameters">Parameters for the command.</param>
        /// <returns>Number of affected rows.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            // Optionally set the command timeout.
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
                // Execute within a transaction.
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
        /// <returns>A transaction object.</returns>
        public IDbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }

        /// <summary>
        /// Gets the DbSet for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>DbSet for the entity.</returns>
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        #endregion

        public DatabaseFacade GetDatabase()
        {
            return Database;
        }

        #region Removed EF6 Properties

        // EF Core does not support these properties in the same way as EF6.
        // public virtual bool ProxyCreationEnabled { get; set; }
        // public virtual bool AutoDetectChangesEnabled { get; set; }

        #endregion
    }
}
