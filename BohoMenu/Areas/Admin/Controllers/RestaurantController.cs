using BohoMenu.Core.DTOs.Requests.Restaurant;
using BohoMenu.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BohoMenu.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Restaurant")]
[Authorize(Roles = "Admin")]
public class RestaurantController : Controller
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet("Settings")]
    public async Task<IActionResult> Settings()
    {
        var restaurant = await _restaurantService.GetAsync();

        if (restaurant == null)
            return NotFound();

        var model = new UpdateRestaurantRequest
        {
            RestaurantName = restaurant.RestaurantName,
            Tagline = restaurant.Tagline,
            LogoUrl = restaurant.LogoUrl
        };

        return View("Settings", model);
    }

    [HttpPost("Settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(UpdateRestaurantRequest request)
    {
        if (!ModelState.IsValid)
        {
            var currentRestaurant = await _restaurantService.GetAsync();

            request.LogoUrl = currentRestaurant?.LogoUrl;

            return View("Settings", request);
        }

        try
        {
            var result = await _restaurantService.UpdateAsync(request);

            if (!result)
                return NotFound();

            TempData["Success"] = "اطلاعات کافه با موفقیت ذخیره شد.";

            return RedirectToAction(nameof(Settings));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(request.Logo), ex.Message);

            var currentRestaurant = await _restaurantService.GetAsync();

            request.LogoUrl = currentRestaurant?.LogoUrl;

            return View("Settings", request);
        }
    }
}
