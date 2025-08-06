using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Entities;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        Task<T> GetByIdAsync(int id, bool isDeleted = false);
        Task<List<T>> GetAllAsync(bool isDeleted = false);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, bool isDeleted = false);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task SaveChangesAsync();

    }
}
