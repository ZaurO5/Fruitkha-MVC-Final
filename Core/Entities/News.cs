using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class News : BaseEntity
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Photo { get; set; }
}
