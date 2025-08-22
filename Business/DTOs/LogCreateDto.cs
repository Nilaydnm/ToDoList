using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class LogCreateDto
    {
        public int? UserId { get; set; }
        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public string QueryString { get; set; } = "";
        public string? Body { get; set; }
        public string? UserAgent { get; set; }
        public string? RemoteIp { get; set; }
        public int? StatusCode { get; set; }
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public class LogEntryDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Method { get; set; } = "";
        public string Url { get; set; } = ""; 
        public string? Body { get; set; }
        public string? UserAgent { get; set; }
        public string? RemoteIp { get; set; }
        public int? StatusCode { get; set; }
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
