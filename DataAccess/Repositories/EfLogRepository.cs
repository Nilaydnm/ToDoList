using DataAccess.Context;
using DataAccess.Interfaces;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class EfLogRepository :EfGenericRepository<LogEntry>, ILogRepository
    {
        private readonly AppDbContext _ctx;

        public EfLogRepository(AppDbContext context) : base(context)
        {
            _ctx = context;
        }


        public async Task<List<LogEntry>> GetLatestAsync(int take = 200)
        {
            return await _ctx.LogEntries.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Take(take)
                .ToListAsync();
        }
    }
}
