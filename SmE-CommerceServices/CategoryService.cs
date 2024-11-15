using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;
using SmE_CommerceModels.ResponseDtos.Category;

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
                    Status = req.Status ?? GeneralStatus.Active,
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
                    Message = SuccessfulMessage.Created,
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

                var result = await categoryRepository.GetCategories(name, UserStatus.Active, pageNumber, pageSize);
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

                var result = await categoryRepository.GetCategories(name,null, pageNumber, pageSize);
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

                var result = await categoryRepository.GetCategoryByIdAsync(id, GeneralStatus.Active);
                if (!result.IsSuccess & result.Data == null)
                {
                    return new Return<GetCategoryDetailResDto?>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = result.Message
                    };
                }
                
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
                var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
                if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                {
                    return new Return<Category>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = currentCustomer.Message
                    };
                }

                var result = await categoryRepository.GetCategoryByIdAsync(id, null);
                if (!result.IsSuccess)
                {
                    return new Return<Category>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = result.Message
                    };
                }
                    
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
    }
}
