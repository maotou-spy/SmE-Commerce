using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceServices.Interface
{
    public interface IProductService
    {
        Task<Return<Product>> AddProductAsync(Product product);
    }
}
