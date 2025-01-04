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

public class NewsRepository : BaseRepository<News>, INewsRepository
{
    private readonly AppDbContext _context;

    public NewsRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<News> GetNewsByTitle(string title)
    {
        return await _context.News.FirstOrDefaultAsync(x => x.Title == title);
    }

    public async Task<List<News>> GetLastThreeNews()
    {
        return await _context.News.OrderByDescending(p => p.CreatedAt).Take(3).ToListAsync();
    }
}
