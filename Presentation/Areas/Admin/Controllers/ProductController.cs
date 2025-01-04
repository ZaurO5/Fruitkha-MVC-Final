using Business.Services.Abstract;
using Business.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }


    #region Read

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await _productService.GetAllAsync();
        return View(model);
    }

    #endregion

    #region Create

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _productService.CreateAsync();
        return View(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM model)
    {
        var isSucceeded = await _productService.CreateAsync(model);
        if (isSucceeded) return RedirectToAction(nameof(Index));

        var result = await _categoryService.GetAllAsync();

        model.Categories = result.Categories.Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Id.ToString()
        }).ToList();

        return View(model);
    }

    #endregion

    #region Update

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var model = await _productService.UpdateAsync(id);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, ProductUpdateVM model)
    {
        var isSucceeded = await _productService.UpdateAsync(id, model);
        if (isSucceeded) return RedirectToAction(nameof(Index));

        var result = await _categoryService.GetAllAsync();

        model.Categories = result.Categories.Select(c => new SelectListItem
        {
            Text = c.Name,
            Value = c.Id.ToString()
        }).ToList();

        return View(model); 
    }

    #endregion

    #region Delete

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var isSucceeded = await _productService.DeleteAsync(id);
        if (isSucceeded) return RedirectToAction(nameof(Index));

        return View();
    }

    #endregion
}
