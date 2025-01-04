using Business.Services.Abstract;
using Business.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await _homeService.GetAllAsync();
        if (model is null) return BadRequest("There is not any product");
        return View(model);
    }
}
