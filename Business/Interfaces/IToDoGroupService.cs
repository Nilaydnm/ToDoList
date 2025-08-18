using Business.DTOs;
using Business.Results;
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
        Task<ToDoGroup> GetByIdWithTasksAsync(int id);
        Task<List<ToDoGroup>> GetGroupsWithTasksByUserIdAsync(int userId);
        Task<OperationResult> CreateAsync(ToDoGroup group, int userId);
        Task<OperationResult> UpdateValidatedAsync(ToDoGroup group, int userId);
        Task<List<ToDoGroupStatsDto>> GetGroupStatsByUserIdAsync(int userId);
    }
}
