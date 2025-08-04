using Business.Interfaces;
using Entities;
using FluentValidation;
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
        private readonly IValidator<ToDo> _toDoValidator;

        public ToDoController(IToDoService toDoService, IToDoGroupService toDoGroupService, IValidator<ToDo> toDoValidator)
        {
            _toDoService = toDoService;
            _toDoGroupService = toDoGroupService;
            _toDoValidator = toDoValidator;
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

            var todo = new ToDo
            {
                Title = title,
                GroupId = groupId,
                Deadline = deadline,
                CreatedAt = DateTime.Now,
                IsCompleted = false,
                UserId = userId
            };

            var validationResult = await _toDoValidator.ValidateAsync(todo);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                }

                ViewBag.PreviousTitle = title;
                ViewBag.PreviousDeadline = deadline;

                var group = await _toDoGroupService.GetByIdWithTasksAsync(groupId);
                return View("~/Views/ToDoGroup/Detail.cshtml", group);
            }

            await _toDoService.AddAsync(todo);
            return RedirectToAction("Detail", "ToDoGroup", new { id = groupId });
        }

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var todo = await _toDoService.GetByIdAsync(id);
            if (todo == null) return NotFound();

            return View(todo);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ToDo todo)
        {
            ToDo existingToDo = null;

            try
            {
                existingToDo = await _toDoService.GetByIdAsync(todo.Id);
                if (existingToDo == null) return NotFound();

                existingToDo.Title = todo.Title;
                existingToDo.Deadline = todo.Deadline;

                await _toDoService.UpdateAsync(existingToDo); // burada validasyon çalışır

                return RedirectToAction("Detail", "ToDoGroup", new { id = existingToDo.GroupId });
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var groupId = todo.GroupId.HasValue
                    ? todo.GroupId.Value
                    : (existingToDo?.GroupId ?? 0);

                var group = await _toDoGroupService.GetByIdWithTasksAsync(groupId);
                return View("~/Views/ToDoGroup/Detail.cshtml", group);
            }
        }



    }
}
