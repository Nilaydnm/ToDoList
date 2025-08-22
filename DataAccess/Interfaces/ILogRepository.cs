using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ILogRepository : IGenericRepository<LogEntry>
    {

        Task<List<LogEntry>> GetLatestAsync(int take = 200);
    }
}
