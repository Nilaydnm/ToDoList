using Business.DTOs;
using Business.Interfaces;
using DataAccess.Interfaces;
using System;
using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Managers
{
    public class LogManager : ILogService
    {
        private readonly ILogRepository _repo;
        public LogManager(ILogRepository repo) { _repo = repo; }

        public async Task AddAsync(LogCreateDto dto)
        {
            var e = new LogEntry
            {
                UserId = dto.UserId,
                Method = dto.Method,
                Path = dto.Path,
                QueryString = dto.QueryString,
                Body = dto.Body,
                UserAgent = dto.UserAgent,
                RemoteIp = dto.RemoteIp,
                StatusCode = dto.StatusCode,
                IsError = dto.IsError,
                ErrorMessage = dto.ErrorMessage,
                StackTrace = dto.StackTrace,
                CreatedAtUtc = dto.CreatedAtUtc
            };
            await _repo.AddAsync(e);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<LogEntryDto>> GetLatestAsync(int take = 200)
        {
            var list = await _repo.GetLatestAsync(take);
            return list.Select(x => new LogEntryDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Method = x.Method,
                Url = x.Path + (string.IsNullOrEmpty(x.QueryString) ? "" : "?" + x.QueryString.TrimStart('?')),
                Body = x.Body,
                UserAgent = x.UserAgent,
                RemoteIp = x.RemoteIp,
                StatusCode = x.StatusCode,
                IsError = x.IsError,
                ErrorMessage = x.ErrorMessage,
                StackTrace = x.StackTrace,
                CreatedAtUtc = x.CreatedAtUtc
            }).ToList();
        }
    }
}
