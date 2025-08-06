using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IToDoService : IGenericService<ToDo>
    {
        Task<List<ToDo>> GetAllAsync();
        Task<ToDo> GetByIdAsync(int id);
        Task AddAsync(ToDo todo);
        Task UpdateAsync(ToDo todo);
        Task DeleteAsync(ToDo todo);
        Task<List<ToDo>> GetByUserIdAsync(int userId);
        Task UpdateWithoutValidationAsync(ToDo todo);
    }
}
