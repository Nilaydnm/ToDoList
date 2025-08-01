using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IToDoGroupRepository : IGenericRepository<ToDoGroup>
    {
        Task DeleteAsync(ToDoGroup group);
        Task<ToDoGroup> GetByIdWithTasksAsync(int id);
        Task<List<ToDoGroup>> GetByUserIdAsync(int userId);
        Task<List<ToDoGroup>> GetGroupsWithTasksAsync();
    }
}

