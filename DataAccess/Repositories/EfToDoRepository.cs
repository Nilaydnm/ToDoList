using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using DataAccess.Interfaces;
using Entities;

namespace DataAccess.Repositories
{
    public class EfToDoRepository : EfGenericRepository<ToDo>, IToDoRepository
    {
        private readonly AppDbContext _context;

        public EfToDoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public Task UpdateAsync(ToDo todo)
        {
            _context.ToDos.Update(todo);
            return Task.CompletedTask;
        }

        public Task UpdateWithoutValidationAsync(ToDo todo)
        {
            _context.ToDos.Update(todo);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ToDo todo)
        {
            _context.ToDos.Remove(todo);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();

        public Task<bool> GroupExistsAsync(int groupId)
            => _context.ToDoGroups.AnyAsync(g => g.Id == groupId);

        public Task<List<ToDo>> GetByUserIdAsync(int userId)
            => _context.ToDos.Where(t => t.UserId == userId).ToListAsync();

        public Task<List<ToDo>> GetToDosByUserIdAsync(int userId)
            => _context.ToDos.Where(t => t.UserId == userId).ToListAsync();
    }
}
