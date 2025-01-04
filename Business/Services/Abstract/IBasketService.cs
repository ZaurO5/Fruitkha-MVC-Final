using Business.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface IBasketService
{
	Task<BasketIndexVM> GetAllAsync();
	Task<(int statusCode, string description)> AddProductAsync(int productId);
	Task<(int statusCode, string description)> UpdateCartAsync(List<BasketUpdateVM> updatedProducts);
	Task<(int statusCode, string description)> DeleteAsync(int id);
}
