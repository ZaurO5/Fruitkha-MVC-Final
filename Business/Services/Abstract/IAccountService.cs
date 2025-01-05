using Business.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface IAccountService
{
    Task<bool> ConfirmEmail(string email, string token);
    Task<bool> RegisterAsync(AccountRegisterVM model);
    Task<(bool IsSucceeded, string? returnUrl)> LoginAsync(AccountLoginVM model);
    Task<bool> ForgetPasswordAsync(ForgetPasswordVM model);
    Task<bool> ResetPassword(ResetPasswordVM model);
}
