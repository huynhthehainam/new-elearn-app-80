
using System.Text;
using eLearnApps.Entity;
using Microsoft.EntityFrameworkCore;

namespace eLearnApps.Data
{
    /// <summary>
    ///     Entity Framework repository
    /// </summary>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        #region Ctor


        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public Repository(IDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Utilities

        /// <summary>
        ///     Get full error
        /// </summary>
        /// <param name="exc">Exception</param>
        /// <returns>Error</returns>
        protected string GetFullErrorText(DbUpdateException exc)
        {
            var msg = new StringBuilder();
            msg.AppendLine("An error occurred while updating the entries. See the inner exception for details.");
            msg.AppendLine(exc.Message);

            if (exc.InnerException != null)
            {
                msg.AppendLine("Inner Exception:");
                msg.AppendLine(exc.InnerException.Message);
            }

            foreach (var entry in exc.Entries)
            {
                msg.AppendLine($"Entity of type {entry.Entity.GetType().Name} in state {entry.State} caused the error.");
            }

            return msg.ToString();
        }

        #endregion

        #region Fields

        private readonly IDbContext _context;
        private DbSet<T>? _entities;

        #endregion

        #region Methods

        /// <summary>
        ///     Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T? GetById(object id)
        {
            //see some suggested performance optimization
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            return Entities.Find(id);
        }

        /// <summary>
        ///     Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(T entity)
        {
            try
            {


                Entities.Add(entity);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {


                foreach (var entity in entities)
                    Entities.Add(entity);

                _context.SaveChanges();
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        /// <summary>
        ///     Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(T entity)
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            try
            {


                if (entities.Count() > 0)
                {
                    foreach (var entity in entities)
                    {
                        try
                        {
                            _context.Detach(entity);
                        }
                        catch
                        {
                            // need to log
                        }

                        ((DbContext)_context).Entry(entity).State = EntityState.Modified;
                    }

                    _context.SaveChanges();
                }


                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            try
            {


                Entities.Remove(entity);

                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        /// <summary>
        ///     Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {

                var es = entities.ToList();
                if (es.Count() > 0)
                {
                    foreach (var entity in es)
                    {
                        try
                        {
                            Entities.Attach(entity);
                        }
                        catch
                        {
                            // need to log
                        }
                        Entities.Remove(entity);
                    }

                    _context.SaveChanges();
                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(GetFullErrorText(dbEx), dbEx);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a table
        /// </summary>
        public virtual IQueryable<T> Table => Entities;

        /// <summary>
        ///     Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only
        ///     operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking => Entities.AsNoTracking();

        public virtual IDbContext Context => _context;

        /// <summary>
        ///     Entities
        /// </summary>
        protected virtual DbSet<T> Entities => _entities ?? (_entities = _context.Set<T>());

        #endregion
    }
}