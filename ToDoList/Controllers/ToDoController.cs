using Business.Interfaces;
using Business.Results;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Infrastructure; 

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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var todos = await _toDoService.GetByUserIdAsync(uid.Value);
            return View(todos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, DeleteAction mode = DeleteAction.Soft)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var todo = await _toDoService.GetByIdAsync(id, isDeleted: true);
            if (todo is null) return NotFound();
            if (todo.UserId != uid.Value) return Forbid();

            var gid = await _toDoService.GetGroupIdByToDoIdAsync(id, isDeleted: true);

            await _toDoService.DeleteAsync(id, mode); 

            if (gid is null || gid == 0)
                return RedirectToAction("Index", "ToDoGroup");

            return RedirectToAction("Detail", "ToDoGroup", new { id = gid });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string title, DateTime? deadline, int groupId)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var group = await _toDoGroupService.GetByIdWithTasksAsync(groupId);
            if (group is null) return NotFound();//servis 
            if (group.UserId != uid.Value) return Forbid();//servis

            var result = await _toDoService.CreateAsync(
                new ToDo { Title = title?.Trim(), GroupId = groupId, Deadline = deadline },
                uid.Value);

            if (!result.Succeeded)
            {
                if (result is OperationResultWithValidation OperationResultWithValidation && OperationResultWithValidation.ValidationErrors is not null)
                    ModelState.AddValidationErrors(OperationResultWithValidation.ValidationErrors);
                else
                    ModelState.AddErrors(result.Errors);

                ViewBag.PreviousTitle = title;
                ViewBag.PreviousDeadline = deadline;

                return View("~/Views/ToDoGroup/Detail.cshtml", group);
            }

            return RedirectToAction("Detail", "ToDoGroup", new { id = groupId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var result = await _toDoService.ToggleCompleteAsync(id, uid.Value);

            if (!result.Succeeded)
            {
                ModelState.AddErrors(result.Errors);

                var gid = await _toDoService.GetGroupIdByToDoIdAsync(id, isDeleted: true);
                if (gid is not null && gid != 0)
                {
                    var group = await _toDoGroupService.GetByIdWithTasksAsync(gid.Value);
                    return View("~/Views/ToDoGroup/Detail.cshtml", group);
                }
                return RedirectToAction("Index", "ToDoGroup");
            }

            return RedirectToAction("Detail", "ToDoGroup", new { id = result.Data });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var todo = await _toDoService.GetByIdAsync(id);
            if (todo == null) return NotFound();
            if (todo.UserId != uid.Value) return Forbid();

            return View(todo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ToDo input)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var result = await _toDoService.UpdateValidatedAsync(input, uid.Value);

            if (!result.Succeeded)
            {
                if (result is OperationResultWithValidation ov && ov.ValidationErrors is not null)
                    ModelState.AddValidationErrors(ov.ValidationErrors);
                else
                    ModelState.AddErrors(result.Errors);

                var gid = input.GroupId ?? 0;
                if (gid != 0)
                {
                    var group = await _toDoGroupService.GetByIdWithTasksAsync(gid);

                    var stats = await _toDoGroupService.GetGroupStatsByUserIdAsync(uid.Value);
                    ViewBag.GroupStats = stats.ToDictionary(s => s.GroupId, s => s);

                    return View("~/Views/ToDoGroup/Detail.cshtml", group);
                }

                return RedirectToAction("Index", "ToDoGroup");
            }

            return RedirectToAction("Detail", "ToDoGroup", new { id = input.GroupId });
        }



    }
}
