using Business.Services.Abstract;
using Business.Utilities.Stripe;
using Business.ViewModels.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Controllers;

[Authorize]
public class BasketController : Controller
{
	private readonly IBasketService _basketService;
	private readonly StripeSettings _stripeSettings;


	public BasketController(IBasketService basketService,
							IOptions<StripeSettings> stripeSettings)
    {
		_basketService = basketService;
		_stripeSettings = stripeSettings.Value;
	}

	[HttpGet]
    public async Task<IActionResult> Index()
    {
		ViewBag.PublishableKey = _stripeSettings.PublishableKey;

		var model = await _basketService.GetAllAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(int productId)
    {
        var result = await _basketService.AddProductAsync(productId);
        switch (result.statusCode)
        {
            case 200:
                return Ok(result.description);
            case 400:
                return BadRequest(result.description);
            case 404:
                return NotFound(result.description);  
            default:
                return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart( [FromBody] List<BasketUpdateVM> updatedProducts)
    {
        var result = await _basketService.UpdateCartAsync(updatedProducts);
		switch (result.statusCode)
		{
			case 200:
				return Ok(result.description);
			case 400:
				return BadRequest(result.description);
			case 404:
				return NotFound(result.description);
			default:
				return NotFound();
		}
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _basketService.DeleteAsync(id);
		switch (result.statusCode)
		{
			case 200:
				return Ok(result.description);
			case 400:
				return BadRequest(result.description);
			case 404:
				return NotFound(result.description);
			default:
				return NotFound();
		}
	}
}
