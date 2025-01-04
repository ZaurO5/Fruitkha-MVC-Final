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

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsWithCategoriesAsync()
    {
        return await _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).ToListAsync();
    }

    public async Task<Product> GetProductByTitleAsync(string title)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Title == title);
    }

    public async Task<Product> GetProductWithCategoriesAsync(int id)
    {
        return await _context.Products.Include(p => p.ProductCategories)
                                      .ThenInclude(pc => pc.Category)
                                      .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetLastThreeProducts()
    {
        return await _context.Products.OrderByDescending(p => p.CreatedAt).Take(3).ToListAsync();
    }
}
