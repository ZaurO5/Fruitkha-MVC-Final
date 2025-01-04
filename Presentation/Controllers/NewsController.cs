using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
public class NewsController : Controller
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _newsService.GetAllAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await _newsService.GetAsync(id);
        return View(model);
    }
}
