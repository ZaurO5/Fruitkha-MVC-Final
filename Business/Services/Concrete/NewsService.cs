using Business.Services.Abstract;
using Business.ViewModels.News;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using IdentityProject.Utilities.File;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly ModelStateDictionary _modelState;

    public NewsService(INewsRepository newsRepository,
                       IUnitOfWork unitOfWork,
                       IFileService fileService,
                       IActionContextAccessor actionContextAccessor)
    {
        _newsRepository = newsRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _modelState = actionContextAccessor.ActionContext.ModelState;
    }

    public async Task<NewsIndexVM> GetAllAsync()
    {
        return new NewsIndexVM {
            News = await _newsRepository.GetAllAsync()
        };
    }

    public async Task<NewsDetailsVM> GetAsync(int id)
    {
        var news = await _newsRepository.GetAsync(id);
        if (news is not null) return new NewsDetailsVM
        {
            Title = news.Title,
            Body = news.Body,
            Photo = news.Photo,
            CreatedAt = news.CreatedAt,
        };

        return null;
    }

    public async Task<bool> CreateAsync(NewsCreateVM model)
    {
        if (!_modelState.IsValid) return false;

        var news = await _newsRepository.GetNewsByTitle(model.Title);
        if (news is not null)
        {
            _modelState.AddModelError("News", "News is Unavailable");
            return false;
        }

        if (!_fileService.IsImage(model.Photo.ContentType))
        {
            _modelState.AddModelError("Photo", "Incorrect format");
            return false;
        }

        if (!_fileService.IsTrueSize(model.Photo.Length))
        {
            _modelState.AddModelError("Photo", "Length must be less than 500 kb");
            return false;
        }

        var photoName = _fileService.Upload(model.Photo, "assets/img");

        news = new News
        {
            Title = model.Title,
            Body = model.Body,
            Photo = photoName,
            CreatedAt = DateTime.Now,
        };

        await _newsRepository.CreateAsync(news);
        await _unitOfWork.CommitAsync();

        return true;    
    }

    public async Task<NewsUpdateVM> UpdateAsync(int id)
    {
        var news = await _newsRepository.GetAsync(id);
        if (news is null) return null;

        var model = new NewsUpdateVM
        {
            Title = news.Title,
            Body = news.Body,
            PhotoName = news.Photo
        };

        return model;
    }

    public async Task<bool> UpdateAsync(int id, NewsUpdateVM model)
    {
        if (!_modelState.IsValid) return false;
        
        var news = await _newsRepository.GetAsync(id);
        if (news is null)
        {
            _modelState.AddModelError("News", "News Unavailable");
            return false;
        }

        var existNews = await _newsRepository.GetNewsByTitle(model.Title);
        if (existNews is not null && existNews.Id != id)
        {
            _modelState.AddModelError("Title", "News already exist");
            return false;
        }

        news.Title = model.Title;
        news.Body = model.Body;
        news.ModifiedAt = DateTime.Now;

        if (model.Photo is not null)
        {
            if (!_fileService.IsImage(model.Photo.ContentType))
            {
                _modelState.AddModelError("Photo", "Incorrect format");
                return false;
            }

            if (!_fileService.IsTrueSize(model.Photo.Length))
            {
                _modelState.AddModelError("Photo", "Length must be less than 500 kb");
                return false;
            }

            _fileService.Delete("assets/img", news.Photo);
            news.Photo = _fileService.Upload(model.Photo, "assets/img");
        }

        _newsRepository.Update(news);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var news = await _newsRepository.GetAsync(id);
        if (news is null) return false;

        _newsRepository.Delete(news);
        await _unitOfWork.CommitAsync();

        return true;
    }

}
