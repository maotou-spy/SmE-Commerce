using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceServices
{
    public class ProductService(IProductRepository productRepository, IHelperService helperService) : IProductService
    {
        public async Task<Return<bool>> AddProductAsync(AddProductReqDto req)
        {
            throw new NotImplementedException();
        }
    }
}
