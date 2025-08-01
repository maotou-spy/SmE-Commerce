﻿using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface ICategoryRepository
{
    Task<Return<Category>> AddCategoryAsync(Category category);
    Task<Return<Category>> GetCategoryByNameAsync(string name);

    Task<Return<IEnumerable<Category>>> GetCategoriesAsync(
        string? name,
        string? status,
        int? pageNumber,
        int? pageSize
    );

    Task<Return<List<Category>>> GetCategoriesByIdsAsync(List<Guid> ids);

    Task<Return<Category>> GetCategoryByIdAsync(Guid id);
    Task<Return<Category>> UpdateCategoryAsync(Category category);
    Task<Return<IEnumerable<Category>>> GetProductsByCategoryIdAsync(Guid id);
    Task<Return<Category>> GetCategoryByIdForUpdateAsync(Guid categoryId);
    Task<Return<List<Category>>> GetCategoriesByProductIdAsync(Guid productId);
    
    Task<Return<IEnumerable<Category>>> GetCategoriesForCustomerAsync();
}
