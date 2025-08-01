using DataAccess.Context;
using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories
{
    public class ToDoGroupRepository : EfGenericRepository<ToDoGroup>, IToDoGroupRepository
    {
        private readonly AppDbContext _context;

        public ToDoGroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        async Task IToDoGroupRepository.DeleteAsync(ToDoGroup group)
        {
            _context.ToDoGroups.Remove(group);
            await _context.SaveChangesAsync();
        }

       

        public async Task<ToDoGroup> GetByIdWithTasksAsync(int id)
        {
            return await _context.ToDoGroups
                .Include(g => g.ToDos)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<ToDoGroup>> GetGroupsWithTasksAsync()
        {
            return await _context.ToDoGroups
                .Include(g => g.ToDos)
                .ToListAsync();
        }

        public async Task<List<ToDoGroup>> GetByUserIdAsync(int userId)
        {
            return await _context.ToDoGroups
            .Where(g => g.Id == userId)
            .ToListAsync();
        }
        public async Task<List<ToDoGroup>> GetGroupsWithTasksByUserIdAsync(int userId)
        {
            return await _context.ToDoGroups
                .Include(g => g.ToDos)
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

    }
}
