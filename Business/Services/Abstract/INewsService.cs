using Business.ViewModels.News;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface INewsService
{
    Task<NewsIndexVM> GetAllAsync();
    Task<NewsDetailsVM> GetAsync(int id);
	Task<bool> CreateAsync(NewsCreateVM model);
    Task<NewsUpdateVM> UpdateAsync(int id);
    Task<bool> UpdateAsync(int id, NewsUpdateVM model);
    Task<bool> DeleteAsync(int id);   
}
