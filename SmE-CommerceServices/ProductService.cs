using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceServices
{
    public class ProductService(IProductService productService, IHelperService helperService) : IProductService
    {
        public Task<Return<Product>> AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
