using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DatabaseContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceRepositories
{
    public class ProductRepository(DefaultdbContext dbContext) : IProductRepository
    {
        public async Task<Return<Product>> AddProductAsync(Product product)
        {
            try
            {
                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();

                return new Return<Product>
                {
                    Data = product,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        
    }
}
