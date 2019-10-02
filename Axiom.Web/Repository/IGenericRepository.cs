using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Axiom.Data.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> AsNoTracking { get; }

        IQueryable<T> Table { get; }

        T GetById(object id);

        void Insert(T entity);

        void Update(T entity, bool changeState = true);

        void SaveChanges();

        void Delete(T entity);

        IQueryable<T> GetAllLazyLoad(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] children);

        //IList<TEntity> QuerySQL<TEntity>(string commandText, params object[] parameters);

        IEnumerable<TElement> ExecuteSQL<TElement>(string commandText, params object[] parameters);
    }
}
