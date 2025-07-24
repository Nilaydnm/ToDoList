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

        public EfToDoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ToDo>> GetToDosByUserIdAsync(int userId)
        {
            return await _context.ToDos
                .Where(todo => todo.UserId == userId)
                .ToListAsync();
        }
    }
}
