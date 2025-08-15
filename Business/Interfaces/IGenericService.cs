using Business.Results;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
   
    public interface IGenericService<T>
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, bool isDeleted = false);
        Task<T> GetByIdAsync(int id, bool isDeleted = false);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id, DeleteAction action = DeleteAction.Soft);

    }
}
