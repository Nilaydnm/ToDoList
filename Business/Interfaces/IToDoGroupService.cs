using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IToDoGroupService : IGenericService<ToDoGroup>
    {
        Task<List<ToDoGroup>> GetGroupsWithTasksAsync();
        Task AddAsync(ToDoGroup group);
        Task<ToDoGroup> GetByIdWithTasksAsync(int id);

        Task DeleteAsync(ToDoGroup group);
    }
}
