using Business.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface IShopService
{
	Task<ShopIndexVM> GetAllAsync();
	Task<ProductDetailsVM> GetAsync(int id);
}
