using Business.Services.Abstract;
using Business.Utilities.Stripe;
using Core.Constants;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class PaymentService : IPaymentService
{
	private readonly StripeSettings _stripeSettings;
	private readonly UserManager<User> _userManager;
	private readonly IActionContextAccessor _actionContextAccessor;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IOrderRepository _orderRepository;
	private readonly IOrderProductRepository _orderProductRepository;
	private readonly IUrlHelperFactory _urlHelperFactory;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IProductRepository _productRepository;
    private readonly IBasketRepository _basketRepository;

    public PaymentService(IOptions<StripeSettings> stripeSettings,
						  UserManager<User> userManager,
						  IActionContextAccessor actionContextAccessor,
						  IUnitOfWork unitOfWork,
						  IOrderRepository orderRepository,
						  IOrderProductRepository orderProductRepository,
						  IUrlHelperFactory urlHelperFactory,
						  IHttpContextAccessor httpContextAccessor,
						  IProductRepository productRepository,
						  IBasketRepository basketRepository)
	{
		_stripeSettings = stripeSettings.Value;
		_userManager = userManager;
		_actionContextAccessor = actionContextAccessor;
		_unitOfWork = unitOfWork;
		_orderRepository = orderRepository;
		_orderProductRepository = orderProductRepository;
		_urlHelperFactory = urlHelperFactory;
		_httpContextAccessor = httpContextAccessor;
		_productRepository = productRepository;
        _basketRepository = basketRepository;
    }

	public async Task<(int statusCode, string? description, string? id)> PayAsync()
	{
		var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
		if (user is null) return (404, "User not found or not authorized", null);

		user = await _userManager.Users.Include(u => u.Basket)
									   .ThenInclude(b => b.BasketProducts).ThenInclude(bp => bp.Product)
									   .FirstOrDefaultAsync(u => u.Id == user.Id);

		var order = new Order
		{
			Status = OrderStatus.Pending,
			CreatedAt = DateTime.Now,
			UserId = user.Id,
			PaymentToken = Guid.NewGuid()
		};

		await _orderRepository.CreateAsync(order);

		var httpContext = _httpContextAccessor.HttpContext;
		var urlHelper =  _urlHelperFactory.GetUrlHelper(new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));
		var scheme = _actionContextAccessor.ActionContext.HttpContext.Request.Scheme;

		var items = new List<SessionLineItemOptions>();

		foreach (var basketProduct in user.Basket.BasketProducts)
		{
			var orderProduct = new OrderProduct
			{
				Order = order,
				Price = basketProduct.Product.Price,
				Count = basketProduct.Count,
				ProductId = basketProduct.ProductId,
			};

			await _orderProductRepository.CreateAsync(orderProduct);

			var item = new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions()
				{
					UnitAmountDecimal = ((orderProduct.Price * Convert.ToDecimal(0.09) + orderProduct.Price) * 100),
					Currency = "USD",
					ProductData = new SessionLineItemPriceDataProductDataOptions()
					{
						Name = orderProduct.Product.Title
					}
				},
				
				Quantity = orderProduct.Count
			};

			items.Add(item);
		}

		await _unitOfWork.CommitAsync();

		var options = new SessionCreateOptions
		{
			PaymentMethodTypes = new List<string> { "card" },
			Mode = "payment",
			LineItems = items,
			SuccessUrl = urlHelper.Action("Success", "Payment", new { token = order.PaymentToken }, scheme),
			CancelUrl = urlHelper.Action("Cancel", "Payment", new { token = order.PaymentToken }, scheme),
		};

		try
		{
			var service = new SessionService();
			Session session = await service.CreateAsync(options);
			return (200, null, session.Id);
		}
		catch (StripeException e)
		{
			return (400, e.Message, null);
		}
	}

	public async Task<bool> PaySuccess(Guid token)
	{
		var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
		if (user is null) return false;

		user = await _userManager.Users.Include(u => u.Basket).FirstOrDefaultAsync(u => u.Id == user.Id);

		var order = await _orderRepository.GetOrderWithOrderProductsAsync(token, user.Id);

		if (order is null) return false;

		order.Status = OrderStatus.Success;
		foreach (var orderProduct in order.OrderProducts)
		{
			var product = await _productRepository.GetAsync(orderProduct.ProductId);
			if (product is not null)
				product.StockCount -= orderProduct.Count;
			
		    _productRepository.Update(product);
		}

		_basketRepository.Delete(user.Basket);
		await _unitOfWork.CommitAsync();

		return true;
	}

    public async Task<bool> PayCancel(Guid token)
    {
        var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
        if (user is null) return false;

        var order = await _orderRepository.GetOrderWithOrderProductsAsync(token, user.Id);

        if (order is null) return false;

        order.Status = OrderStatus.Failed;
		_orderRepository.Update(order);
        await _unitOfWork.CommitAsync();

        return true;
    }
}
