using System.Security.Claims;
using System.Text;
using System.Text.Json;               
using Business.DTOs;
using Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ToDoList.Middleware         
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, ILogService logService)
        {
            int? userId = TryGetUserId(context.User);
            var req = context.Request;

            string method = req.Method;
            string path = req.Path.HasValue ? req.Path.Value! : "";
            string query = req.QueryString.HasValue ? req.QueryString.Value!.TrimStart('?') : "";
            string? userAgent = req.Headers["User-Agent"].ToString();
            string? remoteIp = context.Connection.RemoteIpAddress?.ToString();

            string? bodyText = await TryReadBodyAsync(req);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var dto = new LogCreateDto
                {
                    UserId = userId,
                    Method = method,
                    Path = path,
                    QueryString = query,
                    Body = bodyText,
                    UserAgent = userAgent,
                    RemoteIp = remoteIp,
                    StatusCode = 500,
                    IsError = true,
                    ErrorMessage = ex.Message,
                    StackTrace = ex.ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                };

                try { await logService.AddAsync(dto); } catch { }
                throw; 
            }

            if (context.Response.StatusCode >= 400)
            {
                var dto = new LogCreateDto
                {
                    UserId = userId,
                    Method = method,
                    Path = path,
                    QueryString = query,
                    Body = bodyText,
                    UserAgent = userAgent,
                    RemoteIp = remoteIp,
                    StatusCode = context.Response.StatusCode,
                    IsError = context.Response.StatusCode >= 500,
                    ErrorMessage = $"HTTP {context.Response.StatusCode}",
                    StackTrace = null,
                    CreatedAtUtc = DateTime.UtcNow
                };
                try { await logService.AddAsync(dto); } catch { }
            }

            if (context.Items.TryGetValue("Audit_LoginFailed", out var audit))
            {
                string message = context.Items.TryGetValue("Audit_Message", out var m)
                                 ? m?.ToString() ?? "Login failed"
                                 : "Login failed";

                string bodyJson = JsonSerializer.Serialize(audit);

                var dto = new LogCreateDto
                {
                    UserId = TryGetUserId(context.User), 
                    Method = method,
                    Path = path,
                    QueryString = query,
                    Body = bodyJson,                    
                    UserAgent = userAgent,
                    RemoteIp = remoteIp,
                    StatusCode = context.Response.StatusCode, 
                    IsError = false,                     
                    ErrorMessage = message,
                    StackTrace = null,
                    CreatedAtUtc = DateTime.UtcNow
                };
                try { await logService.AddAsync(dto); } catch { }
            }
        }


        private static int? TryGetUserId(ClaimsPrincipal user)
        {
            var idStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user?.FindFirst("sub")?.Value
                        ?? user?.FindFirst("UserId")?.Value;
            return int.TryParse(idStr, out var id) ? id : (int?)null;
        }

        private static async Task<string?> TryReadBodyAsync(HttpRequest request)
        {
            if (request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) ||
                request.Method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                return null;

            if (request.ContentLength.HasValue && request.ContentLength.Value > 1024 * 128)
                return $"<body too large: {request.ContentLength.Value} bytes>";

            request.EnableBuffering();

            try
            {
                if (request.HasFormContentType)
                {
                    var dict = new Dictionary<string, string>();
                    foreach (var kv in request.Form)
                    {
                        var key = kv.Key;
                        var val = kv.Value.ToString();

                        if (key.Equals("password", StringComparison.OrdinalIgnoreCase) ||
                            key.Equals("pass", StringComparison.OrdinalIgnoreCase) ||
                            key.Equals("pwd", StringComparison.OrdinalIgnoreCase) ||
                            key.Equals("sifre", StringComparison.OrdinalIgnoreCase))
                        {
                            val = "***";
                        }

                        dict[key] = val;
                    }
                    return string.Join("&", dict.Select(k => $"{k.Key}={k.Value}"));
                }

                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    body = body
                        .Replace("\"password\":\"", "\"password\":\"***", StringComparison.OrdinalIgnoreCase)
                        .Replace("\"pwd\":\"", "\"pwd\":\"***", StringComparison.OrdinalIgnoreCase);
                }

                return string.IsNullOrWhiteSpace(body) ? null : body;
            }
            catch
            {
                return "<body read error>";
            }
        }
    }
}
