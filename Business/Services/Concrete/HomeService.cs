using Business.Services.Abstract;
using Business.ViewModels.Home;
using Business.ViewModels.Shop;
using Data.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class HomeService : IHomeService
{
    private readonly IProductRepository _productRepository;
    private readonly INewsRepository _newsRepository;

    public HomeService(IProductRepository productRepository,
                       INewsRepository newsRepository)
    {
        _productRepository = productRepository;
        _newsRepository = newsRepository;
    }

    public async Task<HomeIndexVM> GetAllAsync()
    {
        return new HomeIndexVM
        {
            Products = await _productRepository.GetLastThreeProducts(),
            News = await _newsRepository.GetLastThreeNews()
        };
    }
}
