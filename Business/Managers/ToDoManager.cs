using AutoMapper;
using Business.DTOs;
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
        private readonly IToDoGroupRepository _toDoGroupRepository;
        private readonly IMapper _mapper;

        public ToDoManager(IToDoRepository toDoRepository, IValidator<ToDo> validator, IToDoGroupRepository toDoGroupRepository,IMapper mapper)
        {
            _toDoRepository = toDoRepository;
            _validator = validator;
            _toDoGroupRepository = toDoGroupRepository;
            _mapper = mapper;
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
            => await _toDoRepository.GetByIdAsync(id, isDeleted: false);

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
                        await _toDoRepository.UpdateAsync(entity);
                        await _toDoRepository.SaveChangesAsync();
                    }
                    break;

                case DeleteAction.UndoSoft:
                    if (entity.IsDeleted)
                    {
                        entity.IsDeleted = false;
                        await _toDoRepository.UpdateAsync(entity);
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
                return OperationResultWithValidation.Fail(vr);

            await _toDoRepository.AddAsync(todo);
            await _toDoRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }


        public async Task<OperationResult> UpdateValidatedAsync(ToDo todo, int userId)
        {
            var existing = await _toDoRepository.GetByIdAsync(todo.Id, isDeleted: false);
            if (existing == null)
                return OperationResult.Fail("Görev bulunamadı.");

            if (existing.UserId != userId)
                return OperationResult.Fail("Bu görevi düzenleme yetkiniz yok.");

            existing.Title = todo.Title?.Trim();
            existing.Deadline = todo.Deadline;

            var vr = await _validator.ValidateAsync(existing);
            if (!vr.IsValid)
                return OperationResultWithValidation.Fail(vr);

            await _toDoRepository.UpdateAsync(existing); 
            await _toDoRepository.SaveChangesAsync();

            return OperationResult.Ok();
        }

        public async Task<OperationResult<int>> ToggleCompleteAsync(int id, int userId)
        {
            var t = await _toDoRepository.GetByIdAsync(id, isDeleted: false);
            if (t is null) return OperationResult<int>.Fail("Görev bulunamadı.");
            if (t.UserId != userId) return OperationResult<int>.Fail("Yetkiniz yok.");

            t.IsCompleted = !t.IsCompleted;
            t.CompletedAt = t.IsCompleted ? DateTime.UtcNow : null;

            await _toDoRepository.UpdateAsync(t);            
            await _toDoRepository.SaveChangesAsync();

            return OperationResult<int>.Ok(t.GroupId ?? 0);
        }

        public async Task<List<ToDoDeadlineDto>> GetDeadlineInfoByGroupAsync(int groupId, int userId)
        {
            var group = await _toDoGroupRepository.GetByIdWithTasksAsync(groupId);
            if (group is null || group.UserId != userId)
                return new List<ToDoDeadlineDto>();

            var todos = (group.ToDos ?? Enumerable.Empty<ToDo>()).Where(t => !t.IsDeleted).ToList();

            var list = _mapper.Map<List<ToDoDeadlineDto>>(todos);

            var now = DateTime.Now;
            foreach (var d in list)
                d.Delta = d.Deadline.HasValue ? d.Deadline.Value - now : (TimeSpan?)null;

            return list;
        }


    }
}
