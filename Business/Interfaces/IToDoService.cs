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
    public interface IToDoService : IGenericService<ToDo>
    {
        Task<List<ToDo>> GetByUserIdAsync(int userId, bool isDeleted = false);
        Task<int?> GetGroupIdByToDoIdAsync(int todoId, bool isDeleted = false);
        Task UpdateWithoutValidationAsync(ToDo todo);
        Task<OperationResult> CreateAsync(ToDo todo, int userId);
        Task<OperationResult> UpdateValidatedAsync(ToDo todo, int userId);
        Task<OperationResult<int>> ToggleCompleteAsync(int id, int userId);
        Task<List<ToDoDeadlineDto>> GetDeadlineInfoByGroupAsync(int groupId, int userId);

    }
}
