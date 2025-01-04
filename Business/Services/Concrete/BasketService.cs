using Business.Services.Abstract;
using Business.ViewModels.Basket;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class BasketService : IBasketService
{
	private readonly IBasketRepository _basketRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UserManager<User> _userManager;
	private readonly IActionContextAccessor _actionContextAccessor;
	private readonly IProductRepository _productRepository;
	private readonly IBasketProductRepository _basketProductRepository;

	public BasketService(IBasketRepository basketRepository,
						 IUnitOfWork unitOfWork,
						 UserManager<User> userManager,
						 IActionContextAccessor actionContextAccessor,
						 IProductRepository productRepository,
						 IBasketProductRepository basketProductRepository)
    {
		_basketRepository = basketRepository;
		_unitOfWork = unitOfWork;
		_userManager = userManager;
		_actionContextAccessor = actionContextAccessor;
		_productRepository = productRepository;
		_basketProductRepository = basketProductRepository;
	}


	public async Task<BasketIndexVM> GetAllAsync()
	{
		var checkUser = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User); ;
		if (checkUser is null) return null;

		var user = await _userManager.Users.Include(u => u.Basket).FirstOrDefaultAsync(u => u.Id == checkUser.Id);
		if (user?.Basket is null) return new BasketIndexVM();

		var model = new BasketIndexVM
		{
			BasketProducts = await _basketProductRepository.GetBasketProductsWithProducts(user.Basket.Id)
		};

		return model;
	}
		
	public async Task<(int statusCode, string description)> AddProductAsync(int productId)
	{
		var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
		if (user is null) return (401, "Product couldn't be added");

		var product = await _productRepository.GetAsync(productId);
		if (product is null) return (404, "Product couldn't be added");

		if (product.StockCount == 0) return (400, "Out of stock");

		var basket = await _basketRepository.GetBasketByUserId(user.Id);
		if (basket is null)
		{
			basket = new Basket
			{
				UserId = user.Id,
				CreatedAt = DateTime.Now
			};
		    await _basketRepository.CreateAsync(basket);
		}

		var basketProduct = await _basketProductRepository.GetByProductIdAndUserId(product.Id, user.Id);
		if (basketProduct is null)
		{
			basketProduct = new BasketProduct
			{
				Basket = basket,
				ProductId = productId,
				Count = 1,
				CreatedAt = DateTime.Now
			};

			await _basketProductRepository.CreateAsync(basketProduct);
		}
		else
		{
			if (basketProduct.Count == product.StockCount)
				return (400, "Out of stock");

			basketProduct.Count++;
			_basketProductRepository.Update(basketProduct);
		}


		await _unitOfWork.CommitAsync();
		return (200, "The product was successfully added to the basket");
	}

	public async Task<(int statusCode, string description)> UpdateCartAsync(List<BasketUpdateVM> updatedProducts)
	{
		var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
		if (user is null) return (401, "Product couldn't be added");

		var basket = await _basketRepository.GetBasketByUserId(user.Id);
		if (basket is null) return (404, "User's basket not found");

		foreach (var product in updatedProducts)
		{
			var basketProduct = await _basketProductRepository.GetByProductIdAndUserId(product.ProductId, user.Id);
			if (basketProduct is null) continue;

			var stockProduct = await _productRepository.GetAsync(product.ProductId);
			if (stockProduct is null) return (404, "Product not found");

			if (product.Count > stockProduct.StockCount) return (400, "Out of stock");

			if (product.Count <= 0) return (400, "There must be at least 1 product in basket");
			
			basketProduct.Count = product.Count;
			_basketProductRepository.Update(basketProduct);
		}

		await _unitOfWork.CommitAsync();
		return (200, "Basket updated");
	}

	public async Task<(int statusCode, string description)> DeleteAsync(int id)
	{
		var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
		if (user is null) return (401, "Product couldn't be added");

		var basketProduct = await _basketProductRepository.GetByProductIdAndUserId(id, user.Id);
		if (basketProduct is null) return (404, "Product not found");

		if (basketProduct.Basket.UserId != user.Id)
			return (400, "The product could not be deleted");

		_basketProductRepository.Delete(basketProduct);
		await _unitOfWork.CommitAsync();

		return (200, "Product removed");
	}
}
