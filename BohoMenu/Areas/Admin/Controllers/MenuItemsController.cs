
using BohoMenu.Core.DTOs.Requests.MenuItem;
using BohoMenu.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BohoMenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/MenuItems")]
    [Authorize(Roles = "Admin")]
    public class MenuItemsController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;

        public MenuItemsController(
            IMenuItemService menuItemService,
            ICategoryService categoryService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
        }

        // GET: /Admin/MenuItems
        [HttpGet("")]
        public async Task<IActionResult> Index(int? categoryId)
        {
            var items = await _menuItemService.GetAllAsync(categoryId);

            ViewBag.CategoryId = categoryId;

            if (categoryId.HasValue)
            {
                var category = await _categoryService.GetByIdAsync(categoryId.Value);

                if (category == null)
                    return NotFound();

                ViewBag.CategoryTitle = category.Title;
            }
            else
            {
                ViewBag.CategoryTitle = "همه دسته‌بندی‌ها";
            }

            return View("Index", items);
        }

        // GET: /Admin/MenuItems/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create(int categoryId)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);

            if (category == null)
                return NotFound();

            var request = new CreateMenuItemRequest
            {
                CategoryId = categoryId
            };

            ViewBag.CategoryTitle = category.Title;

            return View("Create", request);
        }

        // POST: /Admin/MenuItems/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int categoryId,
            CreateMenuItemRequest request)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);

            if (category == null)
                return NotFound();

            request.CategoryId = categoryId;

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryTitle = category.Title;

                return View("Create", request);
            }

            try
            {
                var result = await _menuItemService.CreateAsync(request);

                if (result == null)
                {
                    ModelState.AddModelError(
                        "",
                        "امکان افزودن آیتم وجود ندارد.");

                    ViewBag.CategoryTitle = category.Title;

                    return View("Create", request);
                }

                TempData["Success"] = "آیتم با موفقیت اضافه شد.";

                return RedirectToAction(nameof(Index), new { categoryId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(request.Image), ex.Message);

                ViewBag.CategoryTitle = category.Title;

                return View("Create", request);
            }
        }

        // GET: /Admin/MenuItems/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _menuItemService.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            var request = new UpdateMenuItemRequest
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                IsAvailable = item.IsAvailable,
                CategoryId = item.CategoryId
            };

            await LoadCategoriesAsync();

            return View("Edit", request);
        }

        // POST: /Admin/MenuItems/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateMenuItemRequest request)
        {
            if (id != request.Id)
                return BadRequest();

            var currentItem = await _menuItemService.GetByIdAsync(id);

            if (currentItem == null)
                return NotFound();

            // آدرس تصویر را از دیتابیس دریافت می‌کنیم
            request.ImageUrl = currentItem.ImageUrl;

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();

                return View("Edit", request);
            }

            try
            {
                var updated = await _menuItemService.UpdateAsync(request);

                if (!updated)
                {
                    ModelState.AddModelError(
                        nameof(request.CategoryId),
                        "ویرایش انجام نشد. دسته‌بندی انتخاب‌شده معتبر نیست.");

                    await LoadCategoriesAsync();

                    return View("Edit", request);
                }

                TempData["Success"] = "آیتم با موفقیت ویرایش شد.";

                return RedirectToAction(
                    nameof(Index),
                    new { categoryId = request.CategoryId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(request.Image), ex.Message);

                await LoadCategoriesAsync();

                return View("Edit", request);
            }
        }

        // POST: /Admin/MenuItems/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id,
            int? categoryId)
        {
            var deleted = await _menuItemService.DeleteAsync(id);

            if (!deleted)
            {
                TempData["Error"] = "آیتم موردنظر پیدا نشد.";

                return RedirectToAction(
                    nameof(Index),
                    new { categoryId });
            }

            TempData["Success"] = "آیتم با موفقیت حذف شد.";

            return RedirectToAction(
                nameof(Index),
                new { categoryId });
        }

        // بارگذاری دسته‌بندی‌ها برای فرم‌ها
        private async Task LoadCategoriesAsync()
        {
            ViewBag.Categories = await _menuItemService.GetCategoriesAsync();
        }
    }
}
