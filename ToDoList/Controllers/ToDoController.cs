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
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            var todo = new ToDo
            {
                Title = title,
                GroupId = groupId,
                Deadline = deadline
            };

            var result = await _toDoService.CreateAsync(todo, userId);
            if (!result.Succeeded)
            {
                
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e);

                
                ViewBag.PreviousTitle = title;
                ViewBag.PreviousDeadline = deadline;

                var group = await _toDoGroupService.GetByIdWithTasksAsync(groupId);
                return View("~/Views/ToDoGroup/Detail.cshtml", group);
            }

            return RedirectToAction("Detail", "ToDoGroup", new { id = groupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, DeleteAction mode = DeleteAction.Soft)
        {
            // Hard delete sonrası kayıt uçabileceği için, redirect edebilmek adına GID’yi ÖNCE al
            var gid = await _toDoService.GetGroupIdByToDoIdAsync(id, isDeleted: true);

            await _toDoService.DeleteAsync(id, mode);

            if (gid is null || gid == 0)
                return RedirectToAction("Index", "ToDoGroup");

            return RedirectToAction("Detail", "ToDoGroup", new { id = gid });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var todo = await _toDoService.GetByIdAsync(id);
            if (todo == null) return NotFound();

            todo.IsCompleted = !todo.IsCompleted;
            todo.CompletedAt = todo.IsCompleted ? DateTime.Now : null;

            // TODO normal update metodu ile yapılacak ve buradaki business kodları kaldırılacak
            await _toDoService.UpdateWithoutValidationAsync(todo);

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

                await _toDoService.UpdateAsync(existingToDo); 

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
