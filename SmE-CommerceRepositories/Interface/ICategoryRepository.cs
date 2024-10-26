using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmE_CommerceRepositories.Interface
{
    public interface ICategoryRepository
    {
        Task<Return<Category>> AddCategoryAsync(Category category);
        Task<Return<Category>> GetCategoryByNameAsync(string name);
    }
}
