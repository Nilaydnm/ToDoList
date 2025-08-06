using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using DataAccess.Interfaces;
using Entities;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Business.Managers
{
    public class ToDoManager : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IValidator<ToDo> _validator;
        public ToDoManager(IToDoRepository toDoRepository, IValidator<ToDo> validator)
        {
            _toDoRepository = toDoRepository;
            _validator = validator;
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
            var validationResult = await _validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(validationResult.Errors);
            }
            
            await _toDoRepository.AddAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(ToDo todo)
        {
            var validationResult = await _validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ValidationException(validationResult.Errors);
            }
            
            await _toDoRepository.UpdateAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }

        public async Task UpdateWithoutValidationAsync(ToDo todo)
        {
            await _toDoRepository.UpdateAsync(todo);
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

        public Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
