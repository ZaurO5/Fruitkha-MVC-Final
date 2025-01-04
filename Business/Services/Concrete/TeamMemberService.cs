using Business.Services.Abstract;
using Business.ViewModels.TeamMember;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.Repositories.Concrete;
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

public class TeamMemberService : ITeamMemberService
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly ModelStateDictionary _modelState;

    public TeamMemberService(ITeamMemberRepository teamMemberRepository,
                             IUnitOfWork unitOfWork,
                             IActionContextAccessor actionContextAccessor,
                             IFileService fileService)
    {
        _teamMemberRepository = teamMemberRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _modelState = actionContextAccessor.ActionContext.ModelState;
    }

    public async Task<TeamMemberIndexVM> GetAllAsync()
    {
        return new TeamMemberIndexVM
        {
            TeamMembers = await _teamMemberRepository.GetAllAsync()
        };
    }

    public async Task<bool> CreateAsync(TeamMemberCreateVM model)
    {
        if (!_modelState.IsValid) return false;

        if (!_fileService.IsImage(model.Photo.ContentType))
        {
            _modelState.AddModelError("Photo", "The image is not in the correct format");
            return false;
        }

        if (!_fileService.IsTrueSize(model.Photo.Length))
        {
            _modelState.AddModelError("Photo", "Length must be less than 500 kb");
            return false;
        }

        var photoName = _fileService.Upload(model.Photo, "assets/img");

        var teamMember = new TeamMember
        {
            Name = model.Name,
            Surname = model.Surname,
            Position = model.Position,
            Photo = photoName,
            CreatedAt = DateTime.Now
        };

        await _teamMemberRepository.CreateAsync(teamMember);
        await _unitOfWork.CommitAsync();

        return true;
    }

	public async Task<TeamMemberUpdateVM> UpdateAsync(int id)
	{
        var teamMember = await _teamMemberRepository.GetAsync(id);
        if (teamMember is null) return null;

        var model = new TeamMemberUpdateVM
        {
            Name = teamMember.Name,
            Surname = teamMember.Surname,
            Position = teamMember.Position,
            PhotoName = teamMember.Photo
        };

        return model;
	}

	public async Task<bool> UpdateAsync(int id, TeamMemberUpdateVM model)
	{
        if (!_modelState.IsValid) return false;

		var teamMember = await _teamMemberRepository.GetAsync(id);
		if (teamMember is null)
		{
			_modelState.AddModelError(string.Empty, "This product is not available");
			return false;
		}

        teamMember.Name = model.Name;
        teamMember.Surname = model.Surname;
        teamMember.Position = model.Position;
        teamMember.ModifiedAt = DateTime.Now;

		if (model.Photo is not null)
		{
			if (!_fileService.IsImage(model.Photo.ContentType))
			{
				_modelState.AddModelError("Photo", "The image is not in the correct format");
				return false;
			}

			if (!_fileService.IsTrueSize(model.Photo.Length))
			{
				_modelState.AddModelError("Photo", "Length must be less than 500 kb");
				return false;
			}

			_fileService.Delete("assets/img", teamMember.Photo);
			teamMember.Photo = _fileService.Upload(model.Photo, "assets/img");
		}

        _teamMemberRepository.Update(teamMember);
        await _unitOfWork.CommitAsync();

        return true;
	}

    public async Task<bool> DeleteAsync(int id)
    {
        var news = await _teamMemberRepository.GetAsync(id);
        if (news is null) return false;

        _teamMemberRepository.Delete(news);
        await _unitOfWork.CommitAsync();

        return true;
    }
}
