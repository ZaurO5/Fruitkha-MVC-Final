using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.Shop;

public class ProductDetailsVM
{
	public string Photo { get; set; }
	public string Title { get; set; }
	public decimal Price { get; set; }
	public string Description { get; set; }
	public ICollection<ProductCategories> ProductCategories { get; set; }
}
