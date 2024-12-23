using System.Text.RegularExpressions;
using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ResponseDtos.Discount;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class DiscountService(
    IDiscountRepository discountRepository,
    IHelperService helperService,
    IProductRepository productRepository,
    IUserRepository userRepository) : IDiscountService
{
    #region Discount

    public async Task<Return<bool>> AddDiscountAsync(AddDiscountReqDto discount)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    StatusCode = currentUser.StatusCode
                };
            }

            // Check ten co bi trung hay khong
            var existedName = await discountRepository.GetDiscountByNameAsync(discount.DiscountName);
            if (existedName is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.NameAlreadyExists
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
                        StatusCode = ErrorCode.InvalidPercentage
                    };
                }

                if (discount.MaximumDiscount is <= 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidNumber
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
                        StatusCode = ErrorCode.InvalidNumber
                    };
                }
            }

            if (discount is { FromDate: not null, ToDate: not null } || discount.FromDate != null ||
                discount.ToDate != null)
            {
                if (discount.FromDate > discount.ToDate || discount.FromDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }

                if (discount.DiscountCodes != null && discount.DiscountCodes.Any(discountCode =>
                        discountCode.FromDate < discount.FromDate || discountCode.ToDate > discount.ToDate))
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }
            }
            
            // Check minimum order amount valid?
            if (discount.MinimumOrderAmount is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidNumber
                };
            }
            
            // Check maximum discount valid?
            if (discount.MaximumDiscount is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidNumber
                };
            }

            // Check usage limit valid?
            if (discount.UsageLimit is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidNumber
                };
            }

            if (discount is { MinQuantity: not null, MaxQuantity: not null } || discount.MinQuantity != null ||
                discount.MaxQuantity != null)
            {
                if (discount.MinQuantity > discount.MaxQuantity || discount.MinQuantity < 0 || discount.MaxQuantity < 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidNumber
                    };
                }
            }

            foreach (var productId in discount.ProductIds)
            {
                var productExists = await productRepository.GetProductByIdAsync(productId);
                if (productExists is { IsSuccess: false, Data: null } || productExists.Data?.Status == GeneralStatus.Inactive)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.DiscountNotFound
                    };
                }

                if (productExists.Data?.Status == GeneralStatus.Inactive)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound
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
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode
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
                    var existingCode =
                        await discountRepository.GetDiscountCodeByCodeAsync(discountCode.DiscountCode.ToUpper());
                    if (existingCode is { IsSuccess: true, Data: not null })
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = ErrorCode.DiscountCodeAlreadyExists
                        };
                }
            }

            // Add Discount Code
            if (discount.DiscountCodes != null && discount.DiscountCodes.Any())
            {
                if (discount.DiscountCodes.Any(discountCode =>
                        discountCode.FromDate < discount.FromDate || discountCode.ToDate > discount.ToDate))
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }

                result.Data.DiscountCodes = discount.DiscountCodes.Select(x => new DiscountCode
                {
                    DiscountId = result.Data.DiscountId,
                    Code = x.DiscountCode.ToUpper().Trim(),
                    UserId = x.UserId,
                    FromDate = x.FromDate ?? DateTime.Today,
                    ToDate = x.ToDate ?? result.Data.ToDate,
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
                    StatusCode = ErrorCode.Ok
                };
            var addProductsResult = await discountRepository.UpdateDiscountAsync(result.Data);
            if (!addProductsResult.IsSuccess || addProductsResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = addProductsResult.InternalErrorMessage,
                    StatusCode = addProductsResult.StatusCode
                };
            }

            transaction.Complete();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> UpdateDiscountAsync(Guid id, AddDiscountReqDto req)
    {
        using var transaction = new TransactionScope(TransactionScopeOption.Required, 
            new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }, 
            TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    StatusCode = currentUser.StatusCode
                };
            }
            
            var discount = await discountRepository.GetDiscountByIdForUpdateAsync(id);
            if (!discount.IsSuccess || discount.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = discount.StatusCode
                };
            }
            
            // Checking the name of discount
            var existedName = await discountRepository.GetDiscountByNameAsync(req.DiscountName);
            if (existedName is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.NameAlreadyExists
                };
            }
            
            // Check Date valid?
            if (req is { FromDate: not null, ToDate: not null } || req.FromDate != null ||
                req.ToDate != null)
            {
                if (req.FromDate > req.ToDate || req.FromDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }
            }
            
            // Check minimum order amount valid?
            if (req.MinimumOrderAmount is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidNumber
                };
            }
            
            // Check maximum discount valid?
            if (req.MaximumDiscount is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidNumber
                };
            }

            // Check usage limit valid?
            if (req.UsageLimit is < 0)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidNumber
                };
            }
            
            // Check min quantity and max quantity valid?
            if (req is { MinQuantity: not null, MaxQuantity: not null } || req.MinQuantity != null ||
                req.MaxQuantity != null)
            {
                if (req.MinQuantity > req.MaxQuantity || req.MinQuantity < 0 || req.MaxQuantity < 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidNumber
                    };
                }
            }
            
            // Check products valid?
            foreach (var productId in req.ProductIds)
            {
                var productExists = await productRepository.GetProductByIdAsync(productId);
                if (productExists is { IsSuccess: false, Data: null } || productExists.Data?.Status == GeneralStatus.Inactive)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound
                    };
                }
            }
            
            discount.Data.DiscountName = req.DiscountName;
            discount.Data.Description = req.Description;
            discount.Data.IsPercentage = discount.Data.IsPercentage;   // can not update this field
            discount.Data.DiscountValue = discount.Data.DiscountValue; // can not update this field
            discount.Data.MinimumOrderAmount = req.MinimumOrderAmount;
            discount.Data.MaximumDiscount = req.MaximumDiscount;
            discount.Data.FromDate = req.FromDate != discount.Data.FromDate ? req.FromDate : discount.Data.FromDate;
            discount.Data.ToDate = req.ToDate != discount.Data.ToDate ? req.ToDate : discount.Data.ToDate;
            discount.Data.Status = discount.Data.Status;               // can not update this field
            discount.Data.UsageLimit = req.UsageLimit;
            discount.Data.MinQuantity = req.MinQuantity;
            discount.Data.MaxQuantity = req.MaxQuantity;
            discount.Data.IsFirstOrder = req.IsFirstOrder;
            discount.Data.ModifiedAt = DateTime.Now;
            discount.Data.ModifiedById = currentUser.Data.UserId;
            
            var result = await discountRepository.UpdateDiscountAsync(discount.Data);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode
                };
            }
            
            // Kiểm tra xem có cần update không
            var needUpdate = false;
            
            // Add products for discount
            if (req.ProductIds.Count > 0)
            {
                result.Data.DiscountProducts = req.ProductIds.Select(x => new DiscountProduct
                {
                    DiscountId = result.Data.DiscountId,
                    ProductId = x
                }).ToList();
                needUpdate = true;
            }
            
            if (!needUpdate)
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok
                };
            var addProductsResult = await discountRepository.UpdateDiscountAsync(result.Data);
            if (!addProductsResult.IsSuccess || addProductsResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = addProductsResult.InternalErrorMessage,
                    StatusCode = addProductsResult.StatusCode
                };
            }
            
            transaction.Complete();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError
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
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    StatusCode = currentUser.StatusCode
                };
            }

            // Check if DiscountCode is valid
            if (req.DiscountCode.Length is < 4 or > 20 || !Regex.IsMatch(req.DiscountCode, "^[a-zA-Z0-9]+$"))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidDiscountCode
                };
            }

            // Check if DiscountId is valid
            var discount = await discountRepository.GetDiscountByIdAsync(id);
            if (!discount.IsSuccess || discount.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = discount.StatusCode
                };
            }

            // Check if UserId is valid
            if (req.UserId.HasValue)
            {
                var existedUser = await userRepository.GetUserByIdAsync(req.UserId.Value);
                if (!existedUser.IsSuccess || existedUser.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = existedUser.StatusCode
                    };
                }
            }

            if (req is { FromDate: not null, ToDate: not null } || req.FromDate != null || req.ToDate != null)
            {
                if (req.FromDate > req.ToDate)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }

                if (req.FromDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }

                if (req.FromDate < discount.Data.FromDate || req.ToDate > discount.Data.ToDate)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }
            }

            var existedCode = await discountRepository.GetDiscountCodeByCodeAsync(req.DiscountCode.ToUpper());
            if (existedCode is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.DiscountCodeAlreadyExists
                };
            }

            var newCode = new DiscountCode
            {
                DiscountId = id,
                UserId = req.UserId,
                Code = req.DiscountCode.ToUpper().Trim(),
                FromDate = req.FromDate ?? DateTime.Today,
                ToDate = req.ToDate ?? discount.Data.ToDate,
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
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode
                };
            }

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<GetDiscountCodeByCodeResDto>> GetDiscounCodeByCodeAsync(string code)
    {
        try
        {
            var result = await discountRepository.GetDiscountCodeByCodeAsync(code.ToUpper());
            if (!result.IsSuccess)
            {
                return new Return<GetDiscountCodeByCodeResDto>
                {
                    IsSuccess = false,
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode
                };
            }

            var res = result.Data != null
                ? new GetDiscountCodeByCodeResDto
                {
                    UserId = result.Data.UserId,
                    DiscountCode = result.Data.Code,
                    FromDate = result.Data.FromDate,
                    ToDate = result.Data.ToDate,
                    Status = result.Data.Status
                }
                : null;

            return new Return<GetDiscountCodeByCodeResDto>
            {
                Data = res,
                IsSuccess = true,
                TotalRecord = res != null ? 1 : 0,
                StatusCode = res != null ? ErrorCode.Ok : ErrorCode.DiscountNotFound
            };
        }
        catch (Exception e)
        {
            return new Return<GetDiscountCodeByCodeResDto>
            {
                Data = null,
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }

    public async Task<Return<bool>> UpdateDiscountCodeAsync(Guid codeId, AddDiscountCodeReqDto req)
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
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    StatusCode = currentUser.StatusCode
                };
            }
            
            // Check codeId is valid
            var discountCode = await discountRepository.GetDiscountCodeByIdForUpdateAsync(codeId);
            if (!discountCode.IsSuccess || discountCode.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = discountCode.StatusCode
                };
            }
            
            // Check if discountId is valid
            var discount = await discountRepository.GetDiscountByIdForUpdateAsync(discountCode.Data.DiscountId);
            if (!discount.IsSuccess || discount.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = discount.StatusCode
                };
            }
            
            // Check if DiscountCode is valid
            if (req.DiscountCode.Length is < 4 or > 20 || !Regex.IsMatch(req.DiscountCode, "^[a-zA-Z0-9]+$"))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidDiscountCode
                };
            }
            
            // Check if UserId is valid
            if (req.UserId.HasValue)
            {
                var existedUser = await userRepository.GetUserByIdAsync(req.UserId.Value);
                if (!existedUser.IsSuccess || existedUser.Data == null)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = existedUser.StatusCode
                    };
                }
            }
            
            // Check if FromDate and ToDate is valid
            if (req is { FromDate: not null, ToDate: not null } || req.FromDate != null || req.ToDate != null)
            {
                if (req.FromDate > req.ToDate)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }
                
                if (req.FromDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }
                
                if (req.FromDate < discount.Data.FromDate || req.ToDate > discount.Data.ToDate)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDate
                    };
                }
            }
            
            // Check if code is existed
            var existedCode = await discountRepository.GetDiscountCodeByCodeAsync(req.DiscountCode.ToUpper());
            if (existedCode is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.DiscountCodeAlreadyExists
                };
            }
            
            discountCode.Data.Code = req.DiscountCode.ToUpper().Trim();
            discountCode.Data.UserId = req.UserId;
            discountCode.Data.FromDate = req.FromDate ?? DateTime.Today; 
            discountCode.Data.ToDate = req.ToDate ?? discount.Data.ToDate;
            discountCode.Data.Status = discountCode.Data.Status;                // can not update this field   
            discountCode.Data.ModifiedAt = DateTime.Now;
            discountCode.Data.ModifiedById = currentUser.Data.UserId;
            
            var result = await discountRepository.UpdateDiscountCodeAsync(discountCode.Data);
            if (!result.IsSuccess || result.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode
                };
            }
            
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError
            };
        }
    }
    #endregion
}
