using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.Category;

public class CategoryCreateVM
{
    [Required(ErrorMessage = "Title required")]
    [MinLength(2, ErrorMessage = "Min length must be at least 2 characters")]
    public string Name { get; set; }
}
