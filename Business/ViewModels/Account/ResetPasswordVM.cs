using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.Account;

public class ResetPasswordVM
{
    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm new password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Confirm password is wrong")]
    public string ConfirmNewPassword { get; set; }

    public string Email { get; set; }
    public string Token { get; set; }
}
