using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using DataAccess.Interfaces;
using Entities;
using System.Text;
using System.Threading.Tasks;

namespace Business.Managers
{
    public class ToDoManager : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;

        public ToDoManager(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<List<ToDo>> GetAllAsync()
        {
            return await _toDoRepository.GetAllAsync();
        }

        public async Task<ToDo> GetByIdAsync(int id)
        {
            return await _toDoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(ToDo todo)
        {
            await _toDoRepository.AddAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(ToDo todo)
        {
            await _toDoRepository.UpdateAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }
        public async Task<List<ToDo>> GetByUserIdAsync(int userId)
        {
            return await _toDoRepository.GetByUserIdAsync(userId);
        }

        public async Task DeleteAsync(ToDo todo)
        {
            await _toDoRepository.DeleteAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }
    }
}
