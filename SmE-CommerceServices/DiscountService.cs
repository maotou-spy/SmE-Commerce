using System.Text.RegularExpressions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class DiscountService(IDiscountRepository discountRepository, IHelperService helperService, IProductRepository productRepository, IUserRepository userRepository) : IDiscountService
{
    #region Discount
    public async Task<Return<bool>> AddDiscountAsync(AddDiscountReqDto discount)
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
                    Message = currentUser.Message
                };
            }

            // Check ten co bi trung hay khong
            var existedName = await discountRepository.GetDiscountByNameAsync(discount.DiscountName);
            if (existedName is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.Duplicated
                };
            }

            // Check if DiscountValue is valid based on IsPercentage
            if (discount.IsPercentage)
            {
                if (discount.DiscountValue is < 0 or > 100)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidPercentage
                    };
                }
                if (discount.MaximumDiscount is <= 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidNumber
                    };
                }
            }
            else
            {
                if (discount.DiscountValue <= 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidNumber
                    };
                }
            }

            if(discount is { FromDate: not null, ToDate: not null } || discount.FromDate != null || discount.ToDate != null)
            {
                if (discount.FromDate > discount.ToDate)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidDate
                    };
                }

                if (discount.FromDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidDate
                    };
                }
            }

            if(discount.MinimumOrderAmount is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidNumber
                };
            }

            if (discount.UsageLimit is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidNumber
                };
            }

            if (discount is { MinQuantity: not null, MaxQuantity: not null } || discount.MinQuantity != null || discount.MaxQuantity != null)
            {
                if (discount.MinQuantity > discount.MaxQuantity)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidInput
                    };
                }
                if (discount.MinQuantity < 0 || discount.MaxQuantity < 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidNumber
                    };
                }
            }

            foreach (var productId in discount.ProductIds)
            {
                var productExists = await productRepository.GetProductByIdAsync(productId);
                if (productExists is { IsSuccess: false, Data: null })
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.NotFound
                    };
                }

                if (productExists.Data?.Status == GeneralStatus.Inactive)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.NotAvailable
                    };
                }
            }

            var discountModel = new Discount
            {
                DiscountName = discount.DiscountName,
                Description = discount.Description,
                IsPercentage = discount.IsPercentage,
                DiscountValue = discount.DiscountValue,
                MinimumOrderAmount = discount.MinimumOrderAmount,
                MaximumDiscount = discount.MaximumDiscount,
                FromDate = discount.FromDate ?? DateTime.Today,
                ToDate = discount.ToDate ?? DateTime.MaxValue,
                Status = discount.Status != GeneralStatus.Inactive
                    ? GeneralStatus.Active
                    : GeneralStatus.Inactive,
                UsageLimit = discount.UsageLimit,
                UsedCount = 0,
                MinQuantity = discount.MinQuantity,
                MaxQuantity = discount.MaxQuantity,
                IsFirstOrder = discount.IsFirstOrder,
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now
            };

            var result = await discountRepository.AddDiscountAsync(discountModel);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = result.Message,
                    InternalErrorMessage = result.InternalErrorMessage
                };
            }
            
            // Kiểm tra xem có cần update không
            var needUpdate = false;
            
            // Add products for discount
            if (discount.ProductIds.Count > 0)
            {
                result.Data.DiscountProducts = discount.ProductIds.Select(x => new DiscountProduct
                {
                    DiscountId = result.Data.DiscountId,
                    ProductId = x
                }).ToList();
                needUpdate = true;
            }
            
            if (discount.DiscountCodes != null && discount.DiscountCodes.Any())
            {
                foreach (var discountCode in discount.DiscountCodes)
                {
                    var existingCode = await discountRepository.GetDiscountCodeByCodeAsync(discountCode.DiscountCode.ToUpper());
                    if (existingCode is { IsSuccess: true, Data: not null }) 
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            Message = ErrorMessage.Duplicated
                        };
                }
            }
            
            // Add Discount Code
            if (discount.DiscountCodes != null && discount.DiscountCodes.Any())
            {
                result.Data.DiscountCodes = discount.DiscountCodes.Select(x => new DiscountCode
                {
                    DiscountId = result.Data.DiscountId,
                    Code = x.DiscountCode.ToUpper().Trim(),
                    UserId = x.UserId,
                    FromDate = x.FromDate ?? DateTime.Today,
                    ToDate = x.ToDate ?? DateTime.MaxValue,
                    Status = x.Status != DiscountCodeStatus.Inactive
                        ? DiscountCodeStatus.Active
                        : DiscountCodeStatus.Inactive,
                    CreatedAt = DateTime.Now,
                    CreateById = currentUser.Data.UserId
                }).ToList();
                needUpdate = true;
            }

            if (!needUpdate)
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    Message = SuccessMessage.Created
                };
            var addProductsResult = await discountRepository.UpdateDiscountAsync(result.Data);
            if (!addProductsResult.IsSuccess || addProductsResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = addProductsResult.Message,
                    InternalErrorMessage = addProductsResult.InternalErrorMessage
                };
            }

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = SuccessMessage.Created
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }
    #endregion

    #region DiscountCode
    public async Task<Return<bool>> AddDiscountCodeAsync(Guid id, AddDiscountCodeReqDto req)
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
                    Message = currentUser.Message
                };
            }
                
            // Check if DiscountCode is valid
            if (req.DiscountCode.Length is < 4 or > 20 || !Regex.IsMatch(req.DiscountCode, "^[a-zA-Z0-9]+$"))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidInput
                };
            }
                
            // Check if DiscountId is valid
            var discount = await discountRepository.GetDiscountByIdAsync(id);
            if (!discount.IsSuccess || discount.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = discount.Message
                };
            }
                
            // Check if UserId is valid
            if(req.UserId.HasValue)
            {
                var existedUser = await userRepository.GetUserByIdAsync(req.UserId.Value);
                if (!existedUser.IsSuccess || existedUser.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = existedUser.Message
                    };
                }
            }
                
            if(req is { FromDate: not null, ToDate: not null }|| req.FromDate != null || req.ToDate != null)
            {
                if (req.FromDate > req.ToDate)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidDate
                    };
                }

                if (req.FromDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidDate
                    };
                }
            }
            
            var existedCode = await discountRepository.GetDiscountCodeByCodeAsync(req.DiscountCode.ToUpper());
            if (existedCode is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.Duplicated
                };
            }

            var newCode = new DiscountCode
            {
                DiscountId = id,
                UserId = req.UserId,
                Code = req.DiscountCode.ToUpper().Trim(),
                FromDate = req.FromDate ?? DateTime.Today,
                ToDate = req.ToDate ?? DateTime.MaxValue,
                Status = req.Status != DiscountCodeStatus.Inactive
                    ? DiscountCodeStatus.Active
                    : DiscountCodeStatus.Inactive,
                CreatedAt = DateTime.Now,
                CreateById = currentUser.Data.UserId
            };
                
            var result = await discountRepository.AddDiscountCodeAsync(newCode);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = result.Message,
                    InternalErrorMessage = result.InternalErrorMessage
                };
            }
                
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = SuccessMessage.Created
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }
    
    public async Task<Return<DiscountCode>> GetDiscounCodeByCodeAsync(string code)
    {
        try
        {
            var discountCode = await discountRepository.GetDiscountCodeByCodeAsync(code);
            if (!discountCode.IsSuccess || discountCode.Data == null)
            {
                return new Return<DiscountCode>
                {
                    IsSuccess = false,
                    Message = discountCode.Message
                };
            }

            return new Return<DiscountCode>
            {
                Data = discountCode.Data,
                IsSuccess = true,
                Message = SuccessMessage.Found
            };
        }
        catch (Exception e)
        {
            return new Return<DiscountCode>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }
    #endregion
}