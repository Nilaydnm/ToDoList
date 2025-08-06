using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DataAccess.Context;
using DataAccess.Interfaces;
using Entities;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class EfToDoRepository : EfGenericRepository<ToDo>, IToDoRepository
    {
        private readonly AppDbContext _context;

        public async Task UpdateAsync(ToDo todo)
        {
            _context.ToDos.Update(todo);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateWithoutValidationAsync(ToDo todo)
        {
            _context.ToDos.Update(todo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ToDo todo)
        {
            _context.ToDos.Remove(todo);
        }
        public async Task<bool> GroupExistsAsync(int groupId)
        {
            return await _context.ToDoGroups.AnyAsync(g => g.Id == groupId);
        }

        public EfToDoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<ToDo>> GetByUserIdAsync(int userId)
        {
            return await _context.ToDos
                                 .Where(t => t.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<List<ToDo>> GetToDosByUserIdAsync(int userId)
        {
            return await _context.ToDos
                .Where(todo => todo.UserId == userId)
                .ToListAsync();
        }
    }
}
