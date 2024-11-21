using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices
{
    public class CategoryService(ICategoryRepository categoryRepository, IHelperService helperService) : ICategoryService
    {
        public async Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req)
        {
            try
            {
                // Validate user
                var currentUser = await helperService.GetCurrentUserWithRole(nameof(RoleEnum.Manager));
                if (!currentUser.IsSuccess || currentUser.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = currentUser.Message,
                    };
                }

                // Check if name already exists
                var existedCate = await categoryRepository.GetCategoryByNameAsync(req.Name);
                if (existedCate is { IsSuccess: true, Data: not null })
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.UserAlreadyExists,
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
                    CreatedAt = DateTime.Now,
                };

                var result = await categoryRepository.AddCategoryAsync(category);
                if (!result.IsSuccess || result.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InternalServerError,
                        InternalErrorMessage = result.InternalErrorMessage,
                    };
                }

                return new Return<bool>
                {
                    IsSuccess = true,
                    Message = SuccessMessage.Created,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                };
            }
        }

        public async Task<Return<IEnumerable<GetCategoryResDto>>> GetCategoriesForCustomerAsync(string? name, int pageNumber, int pageSize)
        {
            try
            {
                var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Customer);
                if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                {
                    return new Return<IEnumerable<GetCategoryResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
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
                        Message = result.Message
                    };
                }

                List<GetCategoryResDto>? categories = null;
                if (result.Data != null)
                {
                    categories = result.Data.Select(category => new GetCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.Name,
                    }).ToList();
                }

                return new Return<IEnumerable<GetCategoryResDto>>
                {
                    Data = categories,
                    IsSuccess = true,
                    Message = result.Message,
                    TotalRecord = result.TotalRecord
                };
            }
            catch (Exception ex)
            {
                return new Return<IEnumerable<GetCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex
                };
            }
        }

        public async Task<Return<IEnumerable<Category>>> GetCategoriesForManagerAsync(string? name, int pageNumber, int pageSize)
        {
            try
            {
                var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
                if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                {
                    return new Return<IEnumerable<Category>>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = currentCustomer.Message
                    };
                }

                var result = await categoryRepository.GetCategoriesAsync(name,null, pageNumber, pageSize);
                if (!result.IsSuccess)
                {
                    return new Return<IEnumerable<Category>>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = result.Message
                    };
                }

                return new Return<IEnumerable<Category>>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    Message = result.Message,
                    TotalRecord = result.TotalRecord
                };
            }
            catch (Exception ex)
            {
                return new Return<IEnumerable<Category>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex
                };
            }
        }

        public async Task<Return<GetCategoryDetailResDto?>> GetCategoryDetailForCustomerAsync(Guid id)
        {
            try
            {
                var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Customer);
                if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                {
                    return new Return<GetCategoryDetailResDto?>
                    {
                        Data = null,
                        IsSuccess = false,
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
                        Message = result.Message
                    };
                }
                
                if (result.Data.Status != GeneralStatus.Active)
                    return new Return<GetCategoryDetailResDto?>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = ErrorMessage.NotAvailable
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
                    TotalRecord = result.TotalRecord
                };
            }
            catch (Exception ex)
            {
                return new Return<GetCategoryDetailResDto?>
                {
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    Data = null
                };
            }
        }
        
        public async Task<Return<Category>> GetCategoryDetailForManagerAsync(Guid id)
        {
            try
            {
                var currentManager = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
                if (!currentManager.IsSuccess || currentManager.Data == null)
                {
                    return new Return<Category>
                    {
                        Data = null,
                        IsSuccess = false,
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
                        Message = result.Message
                    };
                }
                
                if (result.Data.Status == GeneralStatus.Deleted)
                    return new Return<Category>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = ErrorMessage.NotAvailable
                    };
                    
                return new Return<Category>
                {
                    Data = result.Data,
                    IsSuccess = true,
                    Message = result.Message,
                    TotalRecord = result.TotalRecord
                };
            }
            catch (Exception ex)
            {
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex
                };
            }
        }

        public async Task<Return<bool>> UpdateCategoryDetailAsync(Guid id, AddCategoryReqDto req)
        {
            try
            {
                var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
                if (!currentUser.IsSuccess || currentUser.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = currentUser.Message,
                    };
                }
                
                var oldCategory = await categoryRepository.GetCategoryByIdAsync(id);
                if (oldCategory.Data == null || !oldCategory.IsSuccess)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = currentUser.Message,
                    };
                }
                
                if (oldCategory.Data.Status != GeneralStatus.Active)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = ErrorMessage.NotAvailable
                    };

                oldCategory.Data.Description = req.Description;
                oldCategory.Data.CategoryImage = req.CategoryImage;
                oldCategory.Data.Name = req.Name;
                oldCategory.Data.Status = GeneralStatus.Active;
                oldCategory.Data.ModifiedAt = DateTime.Now;
                oldCategory.Data.ModifiedById = currentUser.Data.UserId;
                
                var result = await categoryRepository.UpdateCategoryAsync(oldCategory.Data);
                if (!result.IsSuccess)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = result.Message
                    };

                return new Return<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = SuccessMessage.Updated
                };
            }
            catch (Exception ex)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                };
            }
        }

        public async Task<Return<bool>> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
                if (!currentUser.IsSuccess || currentUser.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = currentUser.Message,
                    };
                }
                var category = await categoryRepository.GetCategoryByIdAsync(id);
                if (category.Data == null || !category.IsSuccess)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = currentUser.Message,
                    };
                }
                
                if (category.Data.Status == GeneralStatus.Deleted)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Data = false,
                        Message = ErrorMessage.NotAvailable
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
                        Message = currentUser.Message,
                    };
                }

                return new Return<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = SuccessMessage.Deleted
                };
            }
            catch (Exception ex)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                };
            }
        }
    }
}
