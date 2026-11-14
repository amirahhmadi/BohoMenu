using BohoMenu.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BohoMenu.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index(
            int? categoryId,
            string? query)
        {
            var model = await _menuService.GetMenuPageAsync(
                categoryId,
                query
            );

            if (model == null)
                return NotFound("اطلاعات رستوران ثبت نشده است.");

            return View(model);
        }
    }
}
