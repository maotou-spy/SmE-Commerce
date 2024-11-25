using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class CategoryService(ICategoryRepository categoryRepository, IHelperService helperService)
    : ICategoryService
{
    public async Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    ErrorCode = currentUser.ErrorCode,
                    Message = currentUser.Message
                };
            }

            // Check if name already exists
            var existedCate = await categoryRepository.GetCategoryByNameAsync(req.Name);
            if (existedCate is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NameAlreadyExists,
                    ErrorCode = ErrorCodes.NameAlreadyExists
                };
            }

            // Add category
            var category = new Category
            {
                Name = req.Name,
                CategoryImage = req.CategoryImage,
                Description = req.Description,
                Status = GeneralStatus.Active,
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now
            };

            var result = await categoryRepository.AddCategoryAsync(category);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = result.InternalErrorMessage,
                    ErrorCode = result.ErrorCode
                };
            }

            return new Return<bool>
            {
                IsSuccess = true,
                Message = result.Message,
                Data = true,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<Return<IEnumerable<GetCategoryResDto>>> GetCategoriesForCustomerAsync(string? name,
        int pageNumber, int pageSize)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<IEnumerable<GetCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = currentCustomer.ErrorCode,
                    Message = currentCustomer.Message
                };
            }

            var result = await categoryRepository.GetCategoriesAsync(name, UserStatus.Active, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return new Return<IEnumerable<GetCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                };
            }

            List<GetCategoryResDto>? categories = null;
            if (result.Data != null)
            {
                categories = result.Data.Select(category => new GetCategoryResDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.Name
                }).ToList();
            }

            return new Return<IEnumerable<GetCategoryResDto>>
            {
                Data = categories,
                IsSuccess = true,
                Message = result.Message,
                TotalRecord = result.TotalRecord,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<GetCategoryResDto>>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<Return<IEnumerable<Category>>> GetCategoriesForManagerAsync(string? name, int pageNumber,
        int pageSize)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<IEnumerable<Category>>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = currentCustomer.ErrorCode,
                    Message = currentCustomer.Message
                };
            }

            var result = await categoryRepository.GetCategoriesAsync(name, null, pageNumber, pageSize);
            if (!result.IsSuccess)
            {
                return new Return<IEnumerable<Category>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                };
            }

            return new Return<IEnumerable<Category>>
            {
                Data = result.Data,
                IsSuccess = true,
                Message = result.Message,
                TotalRecord = result.TotalRecord,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<Category>>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<Return<GetCategoryDetailResDto?>> GetCategoryDetailForCustomerAsync(Guid id)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<GetCategoryDetailResDto?>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = currentCustomer.ErrorCode,
                    Message = currentCustomer.Message
                };
            }

            var result = await categoryRepository.GetCategoryByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<GetCategoryDetailResDto?>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                };
            }

            if (result.Data.Status != GeneralStatus.Active)
                return new Return<GetCategoryDetailResDto?>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.NotForCustomer,
                    ErrorCode = ErrorCodes.NotForCustomer
                };

            var categories = new GetCategoryDetailResDto
            {
                CategoryId = result.Data!.CategoryId,
                CategoryName = result.Data.Name,
                CategoryImage = result.Data.CategoryImage,
                Description = result.Data.Description
            };

            return new Return<GetCategoryDetailResDto?>
            {
                Data = categories,
                IsSuccess = true,
                Message = result.Message,
                TotalRecord = result.TotalRecord,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<GetCategoryDetailResDto?>
            {
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                Data = null,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<Return<Category>> GetCategoryDetailForManagerAsync(Guid id)
    {
        try
        {
            var currentManager = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentManager.IsSuccess || currentManager.Data == null)
            {
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = currentManager.ErrorCode,
                    Message = currentManager.Message
                };
            }

            var result = await categoryRepository.GetCategoryByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                };
            }

            if (result.Data.Status == GeneralStatus.Deleted)
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.CategoryNotFound,
                    ErrorCode = ErrorCodes.CategoryNotFound
                };

            return new Return<Category>
            {
                Data = result.Data,
                IsSuccess = true,
                Message = result.Message,
                TotalRecord = result.TotalRecord,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<Category>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> UpdateCategoryDetailAsync(Guid id, AddCategoryReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = currentUser.Message,
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var oldCategory = await categoryRepository.GetCategoryByIdAsync(id);
            if (oldCategory.Data == null || !oldCategory.IsSuccess)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = oldCategory.Message,
                    ErrorCode = oldCategory.ErrorCode
                };
            }

            oldCategory.Data.Description = req.Description;
            oldCategory.Data.CategoryImage = req.CategoryImage;
            oldCategory.Data.Name = req.Name;
            oldCategory.Data.Status = oldCategory.Data.Status;
            oldCategory.Data.ModifiedAt = DateTime.Now;
            oldCategory.Data.ModifiedById = currentUser.Data.UserId;

            var result = await categoryRepository.UpdateCategoryAsync(oldCategory.Data);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                };

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> DeleteCategoryAsync(Guid id)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = currentUser.Message,
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var category = await categoryRepository.GetCategoryByIdAsync(id);
            if (category.Data == null || !category.IsSuccess)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = category.Message,
                    ErrorCode = category.ErrorCode
                };
            }

            if (category.Data.Status == GeneralStatus.Deleted)
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = ErrorMessage.CategoryNotFound,
                    ErrorCode = ErrorCodes.CategoryNotFound
                };

            category.Data.Status = GeneralStatus.Deleted;
            category.Data.ModifiedAt = DateTime.Now;
            category.Data.ModifiedById = currentUser.Data.UserId;

            var result = await categoryRepository.UpdateCategoryAsync(category.Data);
            if (result.Data == null || !result.IsSuccess)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                };
            }

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = result.Message,
                ErrorCode = result.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }
}