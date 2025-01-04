using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.News;

public class NewsUpdateVM
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Body is required")]
    public string Body { get; set; }

    public string? PhotoName { get; set; }

    public IFormFile? Photo { get; set; }
}
