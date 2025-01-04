using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.Category;

public class CategoryIndexVM
{
    public List<Core.Entities.Category> Categories { get; set; }
}
