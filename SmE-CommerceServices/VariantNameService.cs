using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.VariantName;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ResponseDtos.VariantName;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class VariantNameService(
    IVariantNameRepository variantRepository,
    IHelperService helperService
) : IVariantNameService
{
    public async Task<Return<List<ManagerVariantNameResDto>>> GetVariantNamesAsync()
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<List<ManagerVariantNameResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var variantsResult = await variantRepository.GetVariantNamesAsync();
            if (!variantsResult.IsSuccess || variantsResult.Data == null)
                return new Return<List<ManagerVariantNameResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = variantsResult.StatusCode,
                    InternalErrorMessage = variantsResult.InternalErrorMessage,
                };

            var variants = variantsResult
                .Data.Select(variantName => new ManagerVariantNameResDto
                {
                    AttributeId = variantName.VariantNameId,
                    AttributeName = variantName.Name,
                    AuditMetadata = new AuditMetadata
                    {
                        CreatedById = variantName.CreatedById,
                        CreatedAt = variantName.CreatedAt,
                        CreatedBy = variantName.CreatedBy?.FullName,
                        ModifiedById = variantName.ModifiedById,
                        ModifiedAt = variantName.ModifiedAt,
                        ModifiedBy = variantName.ModifiedBy?.FullName,
                    },
                })
                .ToList();

            return new Return<List<ManagerVariantNameResDto>>
            {
                Data = variants,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ManagerVariantNameResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> UpdateVariantNameAsync(
        Guid variantNameId,
        VariantNameReqDto variantNameReq
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var existingVariant = await variantRepository.GetVariantNameByIdAsync(variantNameId);
            if (!existingVariant.IsSuccess || existingVariant.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantNameNotFound,
                    InternalErrorMessage = existingVariant.InternalErrorMessage,
                };
            // Check if the attribute name is the same
            if (existingVariant.Data.Name == variantNameReq.VariantName)
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                };

            // Check if the attribute name already exists
            var existingVariantsResult = await variantRepository.GetVariantNamesAsync();
            if (
                existingVariantsResult is { IsSuccess: true, Data: not null }
                && existingVariantsResult.Data.Any(x => x.Name == variantNameReq.VariantName)
            )
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantNameAlreadyExists,
                    InternalErrorMessage = existingVariantsResult.InternalErrorMessage,
                };

            existingVariant.Data.Name = variantNameReq.VariantName;
            existingVariant.Data.ModifiedAt = DateTime.Now;
            existingVariant.Data.ModifiedById = currentUser.Data.UserId;

            return await variantRepository.UpdateVariantNameAsync(existingVariant.Data);
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> DeleteVariantNameAsync(Guid variantId)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var existingVariant = await variantRepository.GetVariantNameByIdAsync(variantId);
            if (!existingVariant.IsSuccess || existingVariant.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantNameNotFound,
                    InternalErrorMessage = existingVariant.InternalErrorMessage,
                };

            // Check if the attribute is used in any product variant
            if (existingVariant.Data.ProductVariants.Count != 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantNameConflict,
                };

            return await variantRepository.DeleteVariantNamesAsync(existingVariant.Data);
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }
}
