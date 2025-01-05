using Business.Services.Abstract;
using Business.ViewModels.Category;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ModelStateDictionary _modelState;

    public CategoryService(ICategoryRepository categoryRepository,
                           IUnitOfWork unitOfWork,
                           IActionContextAccessor actionContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _modelState = actionContextAccessor.ActionContext.ModelState;
    }

    public async Task<CategoryIndexVM> GetAllAsync()
    {
        return new CategoryIndexVM
        {
            Categories = await _categoryRepository.GetAllAsync()
        };
    }

    public async Task<Category> GetAsync(int id)
    {
        return await _categoryRepository.GetAsync(id);
    }

    public async Task<bool> CreateAsync(CategoryCreateVM model)
    {
        if (!_modelState.IsValid) return false;

        var category = await _categoryRepository.GetByNameAsync(model.Name);
        if (category is not null)
        {
            _modelState.AddModelError("Name", "Category already exist");
            return false;
        }

        category = new Category
        {
            Name = model.Name,
            CreatedAt = DateTime.Now
        };

        await _categoryRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<CategoryUpdateVM> UpdateAsync(int id)
    {
        var category = await _categoryRepository.GetAsync(id);
        if (category is null) return null;


        var model = new CategoryUpdateVM
        {
            Name = category.Name
        };

        return model;
    }

    public async Task<bool> UpdateAsync(int id, CategoryUpdateVM model)
    {
        if (!_modelState.IsValid) return false;

        var category = await _categoryRepository.GetAsync(id);
        if (category is null) 
        {
            _modelState.AddModelError(string.Empty, "Category is Unavailable");
            return false;
        }

        var existCategory = await _categoryRepository.GetByNameAsync(model.Name);
        if (existCategory is not null && existCategory.Id != id)
        {
            _modelState.AddModelError("Name", "Category already exist");
            return false;
        }

        category.Name = model.Name;
        category.ModifiedAt = DateTime.Now;

        _categoryRepository.Update(category);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetAsync(id);
        if (category is null) return false;

        _categoryRepository.Delete(category);
        await _unitOfWork.CommitAsync();

        return true;
    }
}
