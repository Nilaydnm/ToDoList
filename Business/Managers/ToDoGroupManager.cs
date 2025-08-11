using Business.Interfaces;
using Business.Results;
using DataAccess.Interfaces;
using Entities;
using FluentValidation;
using System.Linq.Expressions;

namespace Business.Managers
{
    public class ToDoGroupManager : IToDoGroupService
    {
        private readonly IToDoGroupRepository _toDoGroupRepository;
        private readonly IValidator<ToDoGroup> _validator;
        private readonly IToDoRepository _todoRepo;

        public ToDoGroupManager(
            IToDoGroupRepository toDoGroupRepository,
            IValidator<ToDoGroup> validator,
            IToDoRepository toDoRepository)
        {
            _toDoGroupRepository = toDoGroupRepository;
            _validator = validator;
            _todoRepo = toDoRepository;
        }

        public async Task AddAsync(ToDoGroup group)
        {
            var validationResult = await _validator.ValidateAsync(group);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _toDoGroupRepository.AddAsync(group);
            await _toDoGroupRepository.SaveChangesAsync();
        }

        public async Task<ToDoGroup> GetByIdWithTasksAsync(int id)
            => await _toDoGroupRepository.GetByIdWithTasksAsync(id);

        public async Task<List<ToDoGroup>> GetGroupsWithTasksAsync()
            => await _toDoGroupRepository.GetGroupsWithTasksAsync();

        public async Task<List<ToDoGroup>> GetAllAsync()
            => await _toDoGroupRepository.GetAllAsync();

        public async Task<ToDoGroup> GetByIdAsync(int id)
            => await _toDoGroupRepository.GetByIdAsync(id);

        public async Task UpdateAsync(ToDoGroup entity)
        {
            _toDoGroupRepository.Update(entity);
            await _toDoGroupRepository.SaveChangesAsync();
        }

        public async Task<List<ToDoGroup>> GetByUserIdAsync(int userId)
            => await _toDoGroupRepository.GetByUserIdAsync(userId);

        public async Task<List<ToDoGroup>> GetGroupsWithTasksByUserIdAsync(int userId)
            => await _toDoGroupRepository.GetGroupsWithTasksByUserIdAsync(userId);

        public Task<List<ToDoGroup>> GetAllAsync(Expression<Func<ToDoGroup, bool>> filter = null, bool isDeleted = false)
            => _toDoGroupRepository.GetAllAsync(filter, isDeleted);

        public Task<ToDoGroup> GetByIdAsync(int id, bool isDeleted = false)
            => _toDoGroupRepository.GetByIdAsync(id, isDeleted);

        
        public async Task DeleteAsync(int id, DeleteAction action = DeleteAction.Soft)
        {
            var group = await _toDoGroupRepository.GetByIdAsync(id, isDeleted: true);
            if (group is null) return;

            switch (action)
            {
                case DeleteAction.Hard:
                    {
                       
                        var todos = await _todoRepo.GetAllAsync(t => t.GroupId == id, isDeleted: true);
                        foreach (var t in todos)
                            _todoRepo.Remove(t);
                        await _todoRepo.SaveChangesAsync();

                        _toDoGroupRepository.Remove(group);
                        await _toDoGroupRepository.SaveChangesAsync();
                        break;
                    }

                case DeleteAction.Soft:
                    {
                        if (!group.IsDeleted)
                        {
                            group.IsDeleted = true;
                            _toDoGroupRepository.Update(group);
                            await _toDoGroupRepository.SaveChangesAsync();
                        }

                       
                        var childs = await _todoRepo.GetAllAsync(t => t.GroupId == id, isDeleted: true);
                        foreach (var t in childs)
                        {
                            if (!t.IsDeleted)
                            {
                                t.IsDeleted = true;
                                _todoRepo.Update(t);
                            }
                        }
                        await _todoRepo.SaveChangesAsync();
                        break;
                    }

                case DeleteAction.UndoSoft:
                    {
                        if (group.IsDeleted)
                        {
                            group.IsDeleted = false;
                            _toDoGroupRepository.Update(group);
                            await _toDoGroupRepository.SaveChangesAsync();
                        }

                       
                        var childs2 = await _todoRepo.GetAllAsync(t => t.GroupId == id, isDeleted: true);
                        foreach (var t in childs2)
                        {
                            if (t.IsDeleted)
                            {
                                t.IsDeleted = false;
                                _todoRepo.Update(t);
                            }
                        }
                        await _todoRepo.SaveChangesAsync();
                        break;
                    }
            }
        }

        // Kullanılmayan/boş kalanlar istersen tamamen kaldırılabilir:
        // public Task DeleteAsync(int Id) => throw new NotImplementedException();
        // public Task<User> GetByUsernameAsync(string username) => throw new NotImplementedException();
        // public async Task DeleteAsync(ToDoGroup group) { ... }  // kullanılmıyor
    }
}
