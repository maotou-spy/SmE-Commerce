using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category.Manager;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class CategoryService(ICategoryRepository categoryRepository, IHelperService helperService)
    : ICategoryService
{
    public async Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(
                nameof(RoleEnum.Manager)
            );
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool> { IsSuccess = false, StatusCode = currentUser.StatusCode };

            // Check if name already exists
            var existedCate = await categoryRepository.GetCategoryByNameAsync(req.Name);
            if (existedCate is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.NameAlreadyExists
                };

            // Add category
            var category = new Category
            {
                Name = req.Name,
                Description = req.Description,
                Status = GeneralStatus.Active,
                Slug = SlugUtil.GenerateSlug(req.Name).Trim(),
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now
            };

            var result = await categoryRepository.AddCategoryAsync(category);
            if (!result.IsSuccess || result.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = result.StatusCode
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<IEnumerable<GetCategoryResDto>>> GetCategoriesForCustomerAsync(string? name, int pageNumber, int pageSize)
    {
        try
        {
            var result = await categoryRepository.GetCategoriesAsync( name, GeneralStatus.Active, pageNumber, pageSize);
            if (!result.IsSuccess)
                return new Return<IEnumerable<GetCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };
            
            List<GetCategoryResDto>? categories = null;
            if (result.Data != null)
                categories = result
                    .Data.Select(category => new GetCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,
                        Description = category.Description,
                        Slug = category.Slug
                    })
                    .ToList();

            return new Return<IEnumerable<GetCategoryResDto>>
            {
                Data = categories,
                IsSuccess = true,
                TotalRecord = result.TotalRecord,
                StatusCode = result.StatusCode
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<GetCategoryResDto>>
            {
                Data = null,
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<IEnumerable<ManagerGetCategoryResDto>>> GetCategoriesForManagerAsync(
        string? name,
        int pageNumber,
        int pageSize
    )
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<IEnumerable<ManagerGetCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode
                };

            var result = await categoryRepository.GetCategoriesAsync(name, null, pageNumber, pageSize);
            if (!result.IsSuccess)
                return new Return<IEnumerable<ManagerGetCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            var categories = result.Data;
                return new Return<IEnumerable<ManagerGetCategoryResDto>>
                {
                    Data = categories!.Select(category => new ManagerGetCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Name,
                        Description = category.Description,
                        Status = category.Status,
                        Slug = category.Slug,
                        AuditMetadata = new AuditMetadata
                        {
                            CreatedById = category.CreateById,
                            CreatedAt = category.CreatedAt,
                            CreatedBy = category.CreateBy?.FullName,
                            ModifiedById = category.ModifiedById,
                            ModifiedAt = category.ModifiedAt,
                            ModifiedBy = category.ModifiedBy?.FullName
                        }
                    }),
                    IsSuccess = true,
                    TotalRecord = result.TotalRecord,
                    StatusCode = result.StatusCode
                };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<ManagerGetCategoryResDto>>
            {
                Data = null,
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> UpdateCategoryDetailAsync(Guid id, AddCategoryReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = currentUser.StatusCode
                };

            var oldCategory = await categoryRepository.GetCategoryByIdForUpdateAsync(id);
            if (oldCategory.Data == null || !oldCategory.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = oldCategory.StatusCode
                };
            
            // Check if name already exists
            var existedCate = await categoryRepository.GetCategoryByNameAsync(req.Name);
            if (existedCate is { IsSuccess: true, Data: not null } && existedCate.Data.CategoryId != id)
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.NameAlreadyExists
                };

            oldCategory.Data.Description = req.Description;
            oldCategory.Data.Name = req.Name;
            oldCategory.Data.Status = oldCategory.Data.Status;
            oldCategory.Data.Slug = SlugUtil.GenerateSlug(req.Name).Trim();
            oldCategory.Data.ModifiedAt = DateTime.Now;
            oldCategory.Data.ModifiedById = currentUser.Data.UserId;

            var result = await categoryRepository.UpdateCategoryAsync(oldCategory.Data);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = result.StatusCode
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> DeleteCategoryAsync(Guid id)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = currentUser.StatusCode
                };

            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category.Data == null || !category.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = category.StatusCode
                };

            // Check if the category still has products
            var products = await categoryRepository.GetProductsByCategoryIdAsync(id);
            if (products is { IsSuccess: true, Data: not null } && products.Data.Any())
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = ErrorCode.CategoryHasProducts
                };

            if (category.Data.Status == GeneralStatus.Deleted)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = ErrorCode.CategoryNotFound
                };

            category.Data.Status = GeneralStatus.Deleted;
            category.Data.ModifiedAt = DateTime.Now;
            category.Data.ModifiedById = currentUser.Data.UserId;

            var result = await categoryRepository.UpdateCategoryAsync(category.Data);
            if (result.Data == null || !result.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = result.StatusCode
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> UpdateCategoryStatusAsync(Guid id)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = currentUser.StatusCode
                };

            var category = await categoryRepository.GetCategoryByIdForUpdateAsync(id);
            if (category.Data == null || !category.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = category.StatusCode
                };
            
            category.Data.Status = category.Data.Status == GeneralStatus.Active
                ? GeneralStatus.Inactive
                : GeneralStatus.Active;
            category.Data.ModifiedAt = DateTime.Now;
            category.Data.ModifiedById = currentUser.Data.UserId;

            var result = await categoryRepository.UpdateCategoryAsync(category.Data);
            if (result.Data == null || !result.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = result.StatusCode
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }
}
