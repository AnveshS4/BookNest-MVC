using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T>  where T : class
    {

        //T  - Category
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null); //Retreive All category like on Display List
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);//specific category like edit we do need only one data with param we can fetch here we use linq concept
        void Add(T entity);
      
        void Remove(T entity);  
        void RemoveRange(IEnumerable<T> entities);
    }
}
