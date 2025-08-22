using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using System.Linq;

namespace ToDoList.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogService _log;
        public LogController(ILogService log) => _log = log;

        [HttpGet("/log")]
        public async Task<IActionResult> Index([FromQuery] int take = 200,
                                               [FromQuery] string? q = null,
                                               [FromQuery] bool onlyErrors = false)
        {
            var items = await _log.GetLatestAsync(take);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var ql = q.ToLowerInvariant();
                items = items.Where(x =>
                        (x.Url ?? "").ToLower().Contains(ql) ||
                        (x.ErrorMessage ?? "").ToLower().Contains(ql) ||
                        (x.Body ?? "").ToLower().Contains(ql) ||
                        (x.UserAgent ?? "").ToLower().Contains(ql))
                    .ToList();
            }

            if (onlyErrors)
                items = items.Where(x => x.IsError || (x.StatusCode.HasValue && x.StatusCode.Value >= 400)).ToList();

            return View(items); 
        }
    }
}
