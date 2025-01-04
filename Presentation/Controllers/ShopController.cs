using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
public class ShopController : Controller
{
	private readonly IShopService _shopService;

	public ShopController(IShopService shopService)
    {
		_shopService = shopService;
	}

    public async Task<IActionResult> Index()
    {
        var model = await _shopService.GetAllAsync();
		return View(model);
    }

	public async Task<IActionResult> Details(int id)
	{
		var model = await _shopService.GetAsync(id);
		return View(model);
	}
}
