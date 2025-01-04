using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class User : IdentityUser
{
	public string City { get; set; }
	public string Country { get; set; }
	public Basket Basket { get; set; }
}
