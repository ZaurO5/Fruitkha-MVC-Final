using Core.Constants;
using Core.Entities;
using Data.Contexts;
using Data.Repositories.Abstract;
using Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Concrete;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
	private readonly AppDbContext _context;

	public OrderRepository(AppDbContext context) : base(context)
    {
		_context = context;
	}

	public async Task<Order> GetOrderWithOrderProductsAsync(Guid token, string userId)
	{
		return await _context.Orders.Include(o => o.OrderProducts)
							  .FirstOrDefaultAsync(o => o.PaymentToken == token && o.Status == OrderStatus.Pending && o.UserId == userId);
	}
}
