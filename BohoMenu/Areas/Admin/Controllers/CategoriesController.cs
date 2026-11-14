using BohoMenu.Core.DTOs.Requests.Category;
using BohoMenu.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BohoMenu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Categories")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Admin/Categories
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();

            return View("Index", categories);
        }

        // GET: /Admin/Categories/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("Create");
        }

        // POST: /Admin/Categories/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return View("Create", request);

            var categoryId = await _categoryService.CreateAsync(request);

            if (categoryId == null)
            {
                ModelState.AddModelError(
                    nameof(request.Title),
                    "دسته‌بندی با این عنوان قبلاً ثبت شده است.");

                return View("Create", request);
            }

            TempData["Success"] = "دسته‌بندی با موفقیت اضافه شد.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Categories/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            var request = new UpdateCategoryRequest
            {
                Id = category.Id,
                Title = category.Title
            };

            return View("Edit", request);
        }

        // POST: /Admin/Categories/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryRequest request)
        {
            if (id != request.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Edit", request);

            var updated = await _categoryService.UpdateAsync(request);

            if (!updated)
            {
                var category = await _categoryService.GetByIdAsync(id);

                if (category == null)
                    return NotFound();

                ModelState.AddModelError(
                    nameof(request.Title),
                    "دسته‌بندی دیگری با این عنوان وجود دارد.");

                return View("Edit", request);
            }

            TempData["Success"] = "دسته‌بندی با موفقیت ویرایش شد.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Categories/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);

            if (!deleted)
            {
                var category = await _categoryService.GetByIdAsync(id);

                if (category == null)
                    TempData["Error"] = "دسته‌بندی موردنظر پیدا نشد.";
                else
                    TempData["Error"] =
                        "این دسته‌بندی دارای محصول است و امکان حذف آن وجود ندارد.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "دسته‌بندی با موفقیت حذف شد.";

            return RedirectToAction(nameof(Index));
        }
    }
}
