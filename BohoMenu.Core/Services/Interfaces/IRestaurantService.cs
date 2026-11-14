using BohoMenu.Core.DTOs.Requests.Restaurant;
using BohoMenu.Core.DTOs.Responses.Restaurant;

namespace BohoMenu.Core.Services.Interfaces;

public interface IRestaurantService
{
    Task<RestaurantDto?> GetAsync();

    Task<bool> UpdateAsync(UpdateRestaurantRequest request);
}