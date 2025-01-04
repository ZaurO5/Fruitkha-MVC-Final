using Business.Services.Abstract;
using Business.ViewModels.TeamMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class TeamMemberController : Controller
{
    private readonly ITeamMemberService _teamMemberService;

    public TeamMemberController(ITeamMemberService teamMemberService)
    {
        _teamMemberService = teamMemberService;
    }

    #region Read

    public async Task<IActionResult> Index()
    {
        var model = await _teamMemberService.GetAllAsync();
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
    public async Task<IActionResult> Create(TeamMemberCreateVM model)
    {
        var IsSucceeded = await _teamMemberService.CreateAsync(model);
        if (IsSucceeded) return RedirectToAction(nameof(Index));

        return View(model);
    }

	#endregion

	#region Update
	
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var model = await _teamMemberService.UpdateAsync(id);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, TeamMemberUpdateVM model)
    {
        var IsSucceeded = await _teamMemberService.UpdateAsync(id, model);
        if (IsSucceeded) return RedirectToAction(nameof(Index));

        return View(model);
    }

    #endregion

    #region Delete

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var IsSucceeded = await _teamMemberService.DeleteAsync(id);
        if (IsSucceeded) return RedirectToAction(nameof(Index));

        return View();
    }

    #endregion
}
