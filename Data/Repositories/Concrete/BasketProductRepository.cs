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

public class BasketProductRepository : BaseRepository<BasketProduct>, IBasketProductRepository
{
	private readonly AppDbContext _context;

	public BasketProductRepository(AppDbContext context) : base(context)
    {
		_context = context;
	}

	public async Task<List<BasketProduct>> GetBasketProductsWithProducts(int userBasketId)
	{
		return await _context.BasketProducts.Include(bp => bp.Product).Where(bp => bp.BasketId == userBasketId).ToListAsync();
	}

	public async Task<BasketProduct> GetByProductIdAndUserId(int productId, string userId)
	{
		return await _context.BasketProducts.FirstOrDefaultAsync(bp => bp.ProductId == productId && bp.Basket.UserId == userId);
	}
}
