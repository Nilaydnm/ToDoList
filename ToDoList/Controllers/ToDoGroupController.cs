using Business.Interfaces;
using Business.Results;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Infrastructure; 

namespace ToDoList.Controllers
{
    [Authorize]
    public class ToDoGroupController : Controller
    {
        private readonly IToDoGroupService _groupService;
        private readonly IToDoService _toDoService;

        public ToDoGroupController(IToDoGroupService groupService, IToDoService toDoService)
        {
            _groupService = groupService;
            _toDoService = toDoService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Detail(int id, string status = "all")
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var group = await _groupService.GetByIdWithTasksAsync(id);
            if (group == null) return NotFound();
            if (group.UserId != uid) return Forbid();

            ViewBag.Status = (status ?? "all").ToLowerInvariant();
            return View(group);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string status = "all")
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(uid.Value);
            ViewBag.Status = (status ?? "all").ToLowerInvariant();
            return View(groups);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string title)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var result = await _groupService.CreateAsync(new ToDoGroup { Title = title?.Trim() }, uid.Value);

            if (!result.Succeeded)
            {
                if (result is OperationResultWithValidation ov && ov.ValidationErrors is not null)
                    ModelState.AddValidationErrors(ov.ValidationErrors);
                else
                    ModelState.AddErrors(result.Errors);

                ViewBag.PreviousTitle = title;
                var groups = await _groupService.GetByIdAsync(uid.Value);
                return View("Index", groups);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, DeleteAction mode = DeleteAction.Soft)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var g = await _groupService.GetByIdAsync(id, isDeleted: true);
            if (g is null) return NotFound();
            if (g.UserId != uid.Value) return Forbid();

            await _groupService.DeleteAsync(id, mode);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ToDoGroup input)
        {
            var uid = User.GetUserId();
            if (uid is null) return Unauthorized();

            var result = await _groupService.UpdateValidatedAsync(input, uid.Value);

            if (!result.Succeeded)
            {
                if (result is OperationResultWithValidation ov && ov.ValidationErrors is not null)
                    ModelState.AddValidationErrors(ov.ValidationErrors);
                else
                    ModelState.AddErrors(result.Errors);

                var groups = await _groupService.GetByIdAsync(uid.Value);
                ViewBag.PreviousTitle = input.Title;
                return View("Index", groups);
            }

            return RedirectToAction("Index");
        }
    }
}
