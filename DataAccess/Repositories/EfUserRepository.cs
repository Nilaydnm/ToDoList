using DataAccess;
using DataAccess.Context;
using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class EfUserRepository : EfGenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public EfUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<User>> GetByUserIdAsync(int userId, bool isDeleted = false)
        {
            return _context.Users
                .Where(u => u.Id == userId)
                .ToListAsync();
        }

        public async Task<User> GetByUsernameAsync(string username, bool isDeleted = false)
        {
            // firstor default ilk eşleşmeyi kabul ediyor aynı isimde biri kayıt olmasın bunu business katmanında kontrol et!
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
       
    }
}
