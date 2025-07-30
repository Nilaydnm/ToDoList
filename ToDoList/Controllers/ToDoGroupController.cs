using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using Entities;

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

        public async Task<IActionResult> Index()
        {
            var groups = await _groupService.GetGroupsWithTasksAsync();
            return View(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                var newGroup = new ToDoGroup { Title = title };
                await _groupService.AddAsync(newGroup);
            }

            return RedirectToAction("Index");
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
    }
}
