using Business.Services.Abstract;
using Business.ViewModels.Account;
using Core.Entities;
using Data.UnitOfWork;
using Fruitkh.Utilities.EmailHandler.Abstract;
using Fruitkh.Utilities.EmailHandler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ModelStateDictionary _modelState;

    public AccountService(UserManager<User> userManager,
                          SignInManager<User> signInManager,
                          IEmailService emailService,
                          IActionContextAccessor actionContextAccessor,
                          IUnitOfWork unitOfWork,
                          IUrlHelperFactory urlHelperFactory,
                          IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _urlHelperFactory = urlHelperFactory;
        _httpContextAccessor = httpContextAccessor;
        _modelState = actionContextAccessor.ActionContext.ModelState;
    }

    public async Task<bool> RegisterAsync(AccountRegisterVM model)
    {
        if (!_modelState.IsValid) return false;

        var user = new User
        {
            Email = model.Email,
            UserName = model.Email,
            Country = model.Country,
            City = model.City,
            PhoneNumber = model.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                _modelState.AddModelError(string.Empty, error.Description);

            return false;
        }


        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var httpContext = _httpContextAccessor.HttpContext;
        var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));
        var url = urlHelper.Action(nameof(ConfirmEmailAsync), "Account", new { token, email = user.Email }, httpContext.Request.Scheme);
        _emailService.SendMessage(new Message(new List<string> { user.Email }, "Email Confirmation", url));

        return true;
    }
     
    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return false;

        return true;
    }


    public async Task<(bool IsSucceeded, string? returnUrl)> LoginAsync(AccountLoginVM model)
    {
        if (!_modelState.IsValid) return (false, null);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            _modelState.AddModelError(string.Empty, "Email or Password is wrong");
            return (false, null);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (!result.Succeeded)
        {
            _modelState.AddModelError(string.Empty, "Email or Password is wrong");
            return (false, null);
        }

        return (true, model.ReturnUrl);
    }

    public async Task<bool> ForgetPasswordAsync(ForgetPasswordVM model)
    {
        if (!_modelState.IsValid) return false;

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            _modelState.AddModelError("Email", "User not found");
            return false;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var httpContext = _httpContextAccessor.HttpContext;
        var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));
        var url = urlHelper.Action(nameof(ResetPassword), "Account", new { token, user.Email }, httpContext.Request.Scheme);
        _emailService.SendMessage(new Message(new List<string> { user.Email }, "Forget Password?", url));

        return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordVM model)
    {
        if (!_modelState.IsValid) return false;

        var user = await _userManager.FindByNameAsync(model.Email);
        if (user is null)
        {
            _modelState.AddModelError("Password", "It was not possible to update the password");
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                _modelState.AddModelError(string.Empty, error.Description);

            return false;
        }

        return true;
    }
}
