using Business.ViewModels.TeamMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface ITeamMemberService
{
    Task<TeamMemberIndexVM> GetAllAsync();
    Task<bool> CreateAsync(TeamMemberCreateVM model);
    Task<TeamMemberUpdateVM> UpdateAsync(int id);
	Task<bool> UpdateAsync(int id, TeamMemberUpdateVM model);
    Task<bool> DeleteAsync(int id);

}
