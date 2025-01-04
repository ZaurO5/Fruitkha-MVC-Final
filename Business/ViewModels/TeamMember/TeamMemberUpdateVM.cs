using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.TeamMember;

public class TeamMemberUpdateVM
{
	[Required(ErrorMessage = "Name is required")]
	public string Name { get; set; }

	[Required(ErrorMessage = "Surname is required")]
	public string Surname { get; set; }

	[Required(ErrorMessage = "Position is required")]
	public string Position { get; set; }

    public string? PhotoName { get; set; }

    public IFormFile? Photo { get; set; }
}
