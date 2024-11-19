using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices
{
    public class DiscountService(IDiscountRepository discountRepository, IHelperService helperService, IProductRepository productRepository) : IDiscountService
    {
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
                        Message = currentUser.Message,
                    };
                }

                // Check ten co bi trung hay khong
                var existedName = await discountRepository.GetDiscountByNameAsync(discount.DiscountName);
                if (existedName.IsSuccess && existedName.Data != null)
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
                            Message = ErrorMessage.InvalidPercentage,
                        };
                    }
                    if (discount.MaximumDiscount is <= 0)
                    {
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            Message = ErrorMessage.InvalidNumber,
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
                            Message = ErrorMessage.InvalidNumber,
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
                            Message = ErrorMessage.InvalidDate,
                        };
                    }

                    if (discount.FromDate < DateTime.Now)
                    {
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            Message = ErrorMessage.InvalidDate,
                        };
                    }
                }

                if(discount.MinimumOrderAmount is < 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidNumber,
                    };
                }

                if (discount.UsageLimit is < 0)
                {
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        Message = ErrorMessage.InvalidNumber,
                    };
                }

                if (discount is { MinQuantity: not null, MaxQuantity: not null } || discount.MinQuantity != null || discount.MaxQuantity != null)
                {
                    if (discount.MinQuantity > discount.MaxQuantity)
                    {
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            Message = ErrorMessage.InvalidInput,
                        };
                    }
                    if (discount.MinQuantity < 0 || discount.MaxQuantity < 0)
                    {
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            Message = ErrorMessage.InvalidNumber,
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
                            Message = ErrorMessage.NotAvailable,
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

                // Add products for discount
                if (discount.ProductIds.Count <= 0)
                    return new Return<bool>
                    {
                        Data = result.IsSuccess,
                        IsSuccess = result.IsSuccess,
                        Message = result.Message,
                        InternalErrorMessage = result.InternalErrorMessage
                    };

                result.Data.DiscountProducts = discount.ProductIds.Select(x => new DiscountProduct()
                {
                    DiscountId = result.Data.DiscountId,
                    ProductId = x
                }).ToList();

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
                    Message = SuccessMessage.Created,
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
    }
}
