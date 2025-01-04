using Business.Services.Abstract;
using Business.ViewModels.Shop;
using Data.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class ShopService : IShopService
{
	private readonly IProductRepository _productRepository;

	public ShopService(IProductRepository productRepository)
    {
		_productRepository = productRepository;
	}

	public async Task<ShopIndexVM> GetAllAsync()
	{
		return new ShopIndexVM
		{
			Products = await _productRepository.GetAllProductsWithCategoriesAsync()
		};
	}

	public async Task<ProductDetailsVM> GetAsync(int id)
	{
		var product = await _productRepository.GetProductWithCategoriesAsync(id);
		if (product is null) return null;

		var model = new ProductDetailsVM
		{
			Photo = product.Photo,
			Title = product.Title,
			Price = product.Price,
			Description = product.Description,
			ProductCategories = product.ProductCategories
		};

		return model;
	}
}
