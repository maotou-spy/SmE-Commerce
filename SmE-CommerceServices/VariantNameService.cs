using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
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
                        ModifiedById = variantName.ModifiedById,
                        ModifiedAt = variantName.ModifiedAt,
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

    public async Task<Return<bool>> BulkVariantNameAsync(List<string> req)
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

            var variants = req.Select(variant => new VariantName
                {
                    Name = variant,
                    CreatedAt = DateTime.Now,
                    CreatedById = currentUser.Data.Username,
                })
                .ToList();

            return await variantRepository.BulkCreateVariantNameAsync(variants);
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
            existingVariant.Data.ModifiedById = currentUser.Data.Username;

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

    public async Task<Return<bool>> DeleteVariantNameAsync(Guid nameId)
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

            var existingVariant = await variantRepository.GetVariantNameByIdAsync(nameId);
            if (!existingVariant.IsSuccess || existingVariant.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantNameNotFound,
                    InternalErrorMessage = existingVariant.InternalErrorMessage,
                };

            // // Check if the attribute is used in any product variant
            if (existingVariant.Data.VariantAttributes.Count != 0)
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
