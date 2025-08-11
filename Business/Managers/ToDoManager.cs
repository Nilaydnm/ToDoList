using Business.Interfaces;
using Business.Results;
using DataAccess.Interfaces;
using Entities;
using FluentValidation;
using System.Linq.Expressions;

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

        public async Task AddAsync(ToDo todo)
        {
            var validationResult = await _validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _toDoRepository.AddAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(ToDo todo)
        {
            var validationResult = await _validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _toDoRepository.UpdateAsync(todo);
            await _toDoRepository.SaveChangesAsync();
        }

        public async Task UpdateWithoutValidationAsync(ToDo todo)
        {
            await _toDoRepository.UpdateAsync(todo);
            await _toDoRepository.SaveChangesAsync(); 
        }

        public async Task<List<ToDo>> GetByUserIdAsync(int userId)
            => await _toDoRepository.GetByUserIdAsync(userId);

        public async Task<List<ToDo>> GetByUserIdAsync(int userId, bool isDeleted = false)
            => await _toDoRepository.GetAllAsync(t => t.UserId == userId, isDeleted);

        public async Task<ToDo> GetByIdAsync(int id, bool isDeleted)
            => await _toDoRepository.GetByIdAsync(id, isDeleted);

        public async Task<int?> GetGroupIdByToDoIdAsync(int todoId, bool isDeleted = false)
        {
            var todo = await _toDoRepository.GetByIdAsync(todoId, isDeleted);
            return todo?.GroupId;
        }

        public Task<List<ToDo>> GetAllAsync(Expression<Func<ToDo, bool>> filter = null, bool isDeleted = false)
            => _toDoRepository.GetAllAsync(filter, isDeleted);

     
        public async Task DeleteAsync(int id, DeleteAction action = DeleteAction.Soft)
        {
            var entity = await _toDoRepository.GetByIdAsync(id, isDeleted: true);
            if (entity is null) return;

            switch (action)
            {
                case DeleteAction.Hard:
                    _toDoRepository.Remove(entity);
                    await _toDoRepository.SaveChangesAsync();
                    break;

                case DeleteAction.Soft:
                    if (!entity.IsDeleted)
                    {
                        entity.IsDeleted = true;
                        _toDoRepository.Update(entity);
                        await _toDoRepository.SaveChangesAsync();
                    }
                    break;

                case DeleteAction.UndoSoft:
                    if (entity.IsDeleted)
                    {
                        entity.IsDeleted = false;
                        _toDoRepository.Update(entity);
                        await _toDoRepository.SaveChangesAsync();
                    }
                    break;
            }
        }

        public async Task<OperationResult> CreateAsync(ToDo todo, int userId)
        {
            todo.UserId = userId;
            todo.CreatedAt = DateTime.UtcNow;

            var vr = await _validator.ValidateAsync(todo);
            if (!vr.IsValid)
                return OperationResult.Fail(vr.Errors.Select(e => e.ErrorMessage).ToArray());

            await _toDoRepository.AddAsync(todo);
            await _toDoRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }
    }
}
