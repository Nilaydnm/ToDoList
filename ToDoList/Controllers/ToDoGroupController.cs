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
            // 1. Giriş yapan kullanıcının ID'sini alıyoruz (Claims üzerinden)
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            // 2. Sadece bu kullanıcıya ait grup ve görevleri çekiyoruz
            var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);

            return View(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string title)
        {
            try
            {
                // 1. Giriş yapan kullanıcının ID'sini al
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                    return Unauthorized();

                var userId = int.Parse(userIdStr);

                // 2. Title kontrolü: boşsa ModelState hatası ekle
                if (string.IsNullOrWhiteSpace(title))
                {
                    ModelState.AddModelError("Title", "Başlık boş olamaz.");
                }

                // 3. Eğer model hatalıysa aynı sayfaya hata mesajlarıyla dön
                if (!ModelState.IsValid)
                {
                    var groups = await _groupService.GetGroupsWithTasksByUserIdAsync(userId);
                    return View("Index", groups);
                }

                // 4. Grup nesnesini oluştur ve kaydet
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
                // FluentValidation hatalarını ModelState’e ekle
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
