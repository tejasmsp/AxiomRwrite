using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;


namespace Axiom.Data.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Fields

        /// <summary>
        /// The _context
        /// </summary>
        private readonly DBMethods context;

        /// <summary>
        /// The _entities
        /// </summary>
        private IDbSet<T> entities;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref=""/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GenericRepository()
        {
            this.context = new DBMethods();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets no tracking when u read data for not edit
        /// </summary>
        public virtual IQueryable<T> AsNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }

        /// <summary>
        /// Gets whole table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (this.entities == null)
                {
                    this.entities = this.context.Set<T>();
                }

                return this.entities;
            }
        }

        /// <summary>
        /// get record by id
        /// </summary>
        /// <param name="id">the id</param>
        /// <returns>
        /// return entity
        /// </returns>
        public virtual T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        /// <summary>
        /// insert record
        /// </summary>
        /// <param name="entity">the entity</param>
        /// <exception cref="System.ArgumentNullException">return entity</exception>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                this.Entities.Add(entity);

                ////this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }

                var fail = new Exception(msg, dbex);
                throw fail;
            }
        }

        /// <summary>
        /// update record
        /// </summary>
        /// <param name="entity">the entity</param>
        /// <param name="changeState">if set to <c>true</c> [change state].</param>
        /// <exception cref="System.ArgumentNullException">return entity</exception>
        public virtual void Update(T entity, bool changeState = true)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (changeState)
                {
                    this.context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }

                ////this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                var fail = new Exception(msg, dbex);
                throw fail;
            }
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        public virtual void SaveChanges()
        {
            this.context.SaveChanges();
        }

        /// <summary>
        /// delete record
        /// </summary>
        /// <param name="entity">the entity</param>
        /// <exception cref="System.ArgumentNullException">the entity</exception>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                this.context.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                this.Entities.Remove(entity);

                ////this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                var fail = new Exception(msg, dbex);
                throw fail;
            }
        }

        /// <summary>
        /// Gets all lazy load.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="children">The children.</param>
        /// <returns>return entity</returns>
        public virtual IQueryable<T> GetAllLazyLoad(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] children)
        {
            IQueryable<T> query = this.Entities;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = children.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">The Parameters</param>
        /// <returns>return entities</returns>
        public IList<TEntity> QuerySQL<TEntity>(string commandText, params object[] parameters) where TEntity : class
        {
            this.context.Database.CommandTimeout = 300;
            return this.context.QuerySQL<TEntity>(commandText, parameters);
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>return element</returns>
        public IEnumerable<TElement> ExecuteSQL<TElement>(string commandText, params object[] parameters)
        {
            this.context.Db.CommandTimeout = 300;
            return this.context.ExecuteSQL<TElement>(commandText, parameters);
        }

        #endregion
    }
}
