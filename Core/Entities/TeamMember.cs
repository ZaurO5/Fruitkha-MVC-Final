using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class TeamMember : BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Position { get; set; }
    public string Photo { get; set; }
}
