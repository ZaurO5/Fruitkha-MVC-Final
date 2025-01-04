using Business.Services.Abstract;
using Business.ViewModels.News;
using Business.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NewsController : Controller
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    #region Read

    [HttpGet]
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

	#endregion

	#region Create

	[HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewsCreateVM model)
    {
        var isSucceeded = await _newsService.CreateAsync(model);
        if (isSucceeded) return RedirectToAction(nameof(Index));

        return View(model);
    }

    #endregion

    #region Update
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var model = await _newsService.UpdateAsync(id);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int id, NewsUpdateVM model)
    {
        var isSucceeded = await _newsService.UpdateAsync(id, model);
        if (isSucceeded) return RedirectToAction(nameof(Index));

        return View(model);
    }

    #endregion

    #region Delete

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var isSucceeded = await _newsService.DeleteAsync(id);
        if (isSucceeded) return RedirectToAction(nameof(Index));

        return View();
    }

    #endregion
}
