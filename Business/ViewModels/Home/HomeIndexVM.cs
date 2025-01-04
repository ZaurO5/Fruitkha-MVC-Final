using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.Home;

public class HomeIndexVM
{
    public List<Core.Entities.Product> Products { get; set; }
    public List<Core.Entities.News> News { get; set; }
}
