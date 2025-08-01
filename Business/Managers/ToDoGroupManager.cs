using Business.Interfaces;
using Business.ValidationRules;
using DataAccess.Interfaces;
using Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Managers
{
    public class ToDoGroupManager : IToDoGroupService
    {
        private readonly IToDoGroupRepository _toDoGroupRepository;
        private readonly IValidator<ToDoGroup> _validator;


        public ToDoGroupManager(IToDoGroupRepository toDoGroupRepository,IValidator<ToDoGroup>validator)
        {
            _toDoGroupRepository = toDoGroupRepository;
            _validator = validator;
        }

        public async Task AddAsync(ToDoGroup group)
        {
            
            var validationResult = await _validator.ValidateAsync(group);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new FluentValidation.ValidationException("ToDoGroup doğrulama hataları: " + string.Join(", ", errors));
            }

            await _toDoGroupRepository.AddAsync(group);
            await _toDoGroupRepository.SaveChangesAsync();
        }


        public async Task<ToDoGroup> GetByIdWithTasksAsync(int id)
        {
            return await _toDoGroupRepository.GetByIdWithTasksAsync(id);
        }
        public async Task<List<ToDoGroup>> GetGroupsWithTasksAsync()
        {
            return await _toDoGroupRepository.GetGroupsWithTasksAsync();
        }

        
        public async Task DeleteAsync(ToDoGroup group)
        {
            await _toDoGroupRepository.DeleteAsync(group);
            await _toDoGroupRepository.SaveChangesAsync();
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ToDoGroup>> GetAllAsync()
        {
            return await _toDoGroupRepository.GetAllAsync();
        }
        

        public async Task<ToDoGroup> GetByIdAsync(int id)
        {
            return await _toDoGroupRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(ToDoGroup entity)
        {
            _toDoGroupRepository.Update(entity);
            await _toDoGroupRepository.SaveChangesAsync();
        }


        public async Task<List<ToDoGroup>> GetByUserIdAsync(int userId)
        {
            return await _toDoGroupRepository.GetByUserIdAsync(userId);
        }
        public async Task<List<ToDoGroup>> GetGroupsWithTasksByUserIdAsync(int userId)
        {
            return await _toDoGroupRepository.GetGroupsWithTasksByUserIdAsync(userId);
        }

    }
}

