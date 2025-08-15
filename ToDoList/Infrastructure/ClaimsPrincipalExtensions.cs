using System.Security.Claims;

namespace ToDoList.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var val = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : (int?)null;
        }
    }
}
