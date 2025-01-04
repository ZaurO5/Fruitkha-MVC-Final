using Business.ViewModels.Category;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface ICategoryService
{
	Task<CategoryIndexVM> GetAllAsync();
    Task<Category> GetAsync(int id);
    Task<bool> CreateAsync(CategoryCreateVM model);
    Task<CategoryUpdateVM> UpdateAsync(int id);
    Task<bool> UpdateAsync(int id, CategoryUpdateVM model);
    Task<bool> DeleteAsync(int id);

}
