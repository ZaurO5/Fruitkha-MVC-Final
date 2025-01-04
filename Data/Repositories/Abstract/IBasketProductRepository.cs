using Core.Entities;
using Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Abstract;

public interface IBasketProductRepository : IBaseRepository<BasketProduct>
{
	Task<BasketProduct> GetByProductIdAndUserId(int productId, string userId);
	Task<List<BasketProduct>> GetBasketProductsWithProducts(int userBasketId);
}
