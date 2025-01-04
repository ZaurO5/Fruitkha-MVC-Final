using Business.Services.Abstract;
using Business.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoryController : Controller
{
	private readonly ICategoryService _categoryService;

	public CategoryController(ICategoryService categoryService)
    {
		_categoryService = categoryService;
	}

    #region Read

    [HttpGet]
	public async Task<IActionResult> Index()
	{
		var model = await _categoryService.GetAllAsync();
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
	public async Task<IActionResult> Create(CategoryCreateVM model)
	{
		var isSucceeded = await _categoryService.CreateAsync(model);
		if (isSucceeded) return RedirectToAction(nameof(Index));

		return View(model);	
	}

    #endregion

    #region Update

    [HttpGet]
	public async Task<IActionResult> Update(int id)
	{
		var model = await _categoryService.UpdateAsync(id);
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Update(int id, CategoryUpdateVM model)
	{
		var isSucceeded = await _categoryService.UpdateAsync(id, model);
		if (isSucceeded) return RedirectToAction(nameof(Index));

		return View(model);
	}

	#endregion

	#region Delete

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		var isSucceeded = await _categoryService.DeleteAsync(id);
		if (isSucceeded) return RedirectToAction(nameof(Index));	

		return NotFound();
	}

    #endregion
}
