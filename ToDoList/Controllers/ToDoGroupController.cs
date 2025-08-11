using Business.Interfaces;
using Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    public class ToDoGroupController : Controller
    {
        private readonly IToDoGroupService _groupService;
        private readonly IValidator<ToDoGroup> _groupValidator;
        public ToDoGroupController(IToDoGroupService groupService, IValidator<ToDoGroup> groupValidator)
        {
            _groupService = groupService;
            _groupValidator = groupValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var group = await _groupService.GetByIdWithTasksAsync(id);
            if (group == null) return NotFound();
            // kullanıcı kontrolü yapılsın !
            return View(group);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);

            return View(groups);
        }

        [HttpGet]
        public async Task<IActionResult> DetailFromSession()
        {
            var groupId = HttpContext.Session.GetInt32("CurrentGroupId");

            if (groupId == null)
                return RedirectToAction("Index");

            var group = await _groupService.GetByIdWithTasksAsync(groupId.Value);
            if (group == null) return NotFound();

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

           

            return View("Detail", group);
        }
        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            try
            {
                
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized();

                var userId = int.Parse(userIdStr);

                if (string.IsNullOrWhiteSpace(title))
                {
                    ModelState.AddModelError("Title", "Başlık boş olamaz.");
                }

                
                if (!ModelState.IsValid)
                {
                    var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);
                    return View("Index", groups);
                }

                var group = new ToDoGroup
                {
                    Title = title,
                    UserId = userId
                };

                await _groupService.AddAsync(group);
                return RedirectToAction("Index");
            }
            catch (FluentValidation.ValidationException ex)
            {
             
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);
                return View("Index", groups);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, DeleteAction mode = DeleteAction.Soft)
        {
            await _groupService.DeleteAsync(id, mode);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _groupService.GetByIdAsync(id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ToDoGroup group)
        {
            try
            {
                var validationResult = await _groupValidator.ValidateAsync(group);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }

                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);
                    return View("Index", groups);
                }

                var existingGroup = await _groupService.GetByIdAsync(group.Id);
                if (existingGroup == null) return NotFound();

                existingGroup.Title = group.Title;

                await _groupService.UpdateAsync(existingGroup);

                return RedirectToAction("Index");
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);
                return View("Index", groups);
            }
        }



        



    }
}
