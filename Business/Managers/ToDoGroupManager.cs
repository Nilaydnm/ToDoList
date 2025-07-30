using System;
using Business.Interfaces;
using DataAccess.Interfaces;
using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Managers
{
    public class ToDoGroupManager : IToDoGroupService
    {
        private readonly IToDoGroupRepository _toDoGroupRepository;

        public async Task<ToDoGroup> GetByIdWithTasksAsync(int id)
        {
            return await _toDoGroupRepository.GetByIdWithTasksAsync(id);
        }

        public ToDoGroupManager(IToDoGroupRepository toDoGroupRepository)
        {
            _toDoGroupRepository = toDoGroupRepository;
        }

        public async Task<List<ToDoGroup>> GetGroupsWithTasksAsync()
        {
            return await _toDoGroupRepository.GetGroupsWithTasksAsync();
        }

        public async Task AddAsync(ToDoGroup group)
        {
            await _toDoGroupRepository.AddAsync(group);
            await _toDoGroupRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(ToDoGroup group)
        {
            await _toDoGroupRepository.DeleteAsync(group);
            await _toDoGroupRepository.SaveChangesAsync();
        }
    }
}

