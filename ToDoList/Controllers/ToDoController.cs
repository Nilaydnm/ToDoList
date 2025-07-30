using Business.Interfaces;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        private readonly IToDoService _toDoService;
        private readonly IToDoGroupService _toDoGroupService;

        public ToDoController(IToDoService toDoService, IToDoGroupService toDoGroupService)
        {
            _toDoService = toDoService;
            _toDoGroupService = toDoGroupService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var todos = await _toDoService.GetByUserIdAsync(userId);
            return View(todos);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string title, DateTime? deadline, int groupId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (string.IsNullOrWhiteSpace(title))
                return RedirectToAction("Detail", "ToDoGroup", new { id = groupId });

            var todo = new ToDo
            {
                Title = title,
                GroupId = groupId,
                Deadline = deadline,
                CreatedAt = DateTime.Now,
                IsCompleted = false,
                UserId = userId
            };

            await _toDoService.AddAsync(todo);
            return RedirectToAction("Detail", "ToDoGroup", new { id = groupId });
        }

        // Alt görev silme
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _toDoService.GetByIdAsync(id);
            if (todo == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (todo.UserId != userId)
                return Unauthorized();

            int groupId = todo.GroupId ?? 0;

            await _toDoService.DeleteAsync(todo);
            return RedirectToAction("Detail", "ToDoGroup", new { id = groupId });
        }

        // Checkbox ile tamamlandı/bekliyor işlemi
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var todo = await _toDoService.GetByIdAsync(id);
            if (todo == null)
                return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (todo.UserId != userId)
                return Unauthorized();

            todo.IsCompleted = !todo.IsCompleted;
            todo.CompletedAt = todo.IsCompleted ? DateTime.Now : null;

            await _toDoService.UpdateAsync(todo);
            return RedirectToAction("Detail", "ToDoGroup", new { id = todo.GroupId });
        }
    }
}
