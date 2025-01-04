using Business.Services.Abstract;
using Business.ViewModels.About;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class AboutService : IAboutService
{
    private readonly ITeamMemberRepository _teamMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AboutService(ITeamMemberRepository teamMemberRepository,
                        IUnitOfWork unitOfWork)
    {
        _teamMemberRepository = teamMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AboutIndexVM> GetAllAsync()
    {
        return new AboutIndexVM
        {
            TeamMembers = await _teamMemberRepository.GetAllAsync()
        };
    }
}
