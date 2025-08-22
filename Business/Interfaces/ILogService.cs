using Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ILogService
    {
        Task AddAsync(LogCreateDto dto);
        Task<List<LogEntryDto>> GetLatestAsync(int take = 200);
    }
}
