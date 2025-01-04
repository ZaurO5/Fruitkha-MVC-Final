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

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
	private readonly AppDbContext _context;

	public CategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
	}

    public async Task<Category> GetByNameAsync(string name)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    } 
}
