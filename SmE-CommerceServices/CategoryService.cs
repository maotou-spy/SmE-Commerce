using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
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
                    CategoryImageHash = HashUtil.Hash(req.CategoryImage),
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
    }
}
