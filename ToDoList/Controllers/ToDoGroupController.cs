using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using Entities;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Controllers
{
    public class ToDoGroupController : Controller
    {
        private readonly IToDoGroupService _groupService;

        public ToDoGroupController(IToDoGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var group = await _groupService.GetByIdWithTasksAsync(id);
            if (group == null) return NotFound();

            return View(group);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var groups = await _groupService.GetGroupsWithTasksAsync();
            return View(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            try
            {
                var group = new ToDoGroup { Title = title };
                await _groupService.AddAsync(group);
                return RedirectToAction("Index");
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var groups = await _groupService.GetGroupsWithTasksAsync();
                return View("Index", groups);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _groupService.GetByIdWithTasksAsync(id);
            if (group == null)
                return NotFound();

            await _groupService.DeleteAsync(group);
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

        // POST: ToDoGroup/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(ToDoGroup group)
        {
            try
            {
                await _groupService.UpdateAsync(group);
                return RedirectToAction("Index");
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                return View(group);
            }
        }



    }
}
