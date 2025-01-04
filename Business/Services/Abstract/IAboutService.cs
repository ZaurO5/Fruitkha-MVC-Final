using Business.ViewModels.About;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface IAboutService
{
    Task<AboutIndexVM> GetAllAsync();
}
