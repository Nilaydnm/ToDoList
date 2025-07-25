using Business.Interfaces;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class ToDoController : Controller
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var todos = await _toDoService.GetByUserIdAsync(userId);
            return View(todos);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var todo = new ToDo
            {
                Title = title,
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            await _toDoService.AddAsync(todo);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var todo = await _toDoService.GetByIdAsync(id);

            if (todo == null || todo.UserId != userId)
                return Unauthorized();

            await _toDoService.DeleteAsync(todo);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var todo = await _toDoService.GetByIdAsync(id);

            if (todo == null || todo.UserId != userId)
                return Unauthorized();

            todo.IsCompleted = !todo.IsCompleted;
            todo.CompletedAt = todo.IsCompleted ? DateTime.Now : null;

            await _toDoService.UpdateAsync(todo);
            return RedirectToAction("Index");
        }
    }
}
