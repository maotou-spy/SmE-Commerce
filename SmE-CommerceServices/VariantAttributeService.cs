using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.VariantAttribute;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ResponseDtos.VariantAttribute;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class VariantAttributeService(
    IVariantAttributeRepository variantRepository,
    IHelperService helperService
) : IVariantAttributeService
{
    public async Task<Return<List<ManagerVariantAttributeResDto>>> GetVariantAttributesAsync()
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<List<ManagerVariantAttributeResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var variantsResult = await variantRepository.GetVariantAttributes();
            if (!variantsResult.IsSuccess || variantsResult.Data == null)
                return new Return<List<ManagerVariantAttributeResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = variantsResult.StatusCode,
                    InternalErrorMessage = variantsResult.InternalErrorMessage,
                };

            var variants = variantsResult
                .Data.Select(attribute => new ManagerVariantAttributeResDto
                {
                    AttributeId = attribute.VariantId,
                    AttributeName = attribute.AttributeName,
                    AuditMetadata = new AuditMetadata
                    {
                        CreatedById = attribute.CreatedById,
                        CreatedAt = attribute.CreatedAt,
                        CreatedBy = attribute.CreatedBy?.FullName,
                        ModifiedById = attribute.ModifiedById,
                        ModifiedAt = attribute.ModifiedAt,
                        ModifiedBy = attribute.ModifiedBy?.FullName,
                    },
                })
                .ToList();

            return new Return<List<ManagerVariantAttributeResDto>>
            {
                Data = variants,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ManagerVariantAttributeResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> BulkCreateVariantAttributeAsync(List<VariantReqDto> variantReqs)
    {
        try
        {
            // Get the current user with the Manager role
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            // Remove duplicates within the input list
            variantReqs = variantReqs
                .GroupBy(req => req.AttributeName)
                .Select(group => group.First())
                .ToList();

            // Fetch existing attributes
            var existingVariantsResult = await variantRepository.GetVariantAttributes();
            if (!existingVariantsResult.IsSuccess || existingVariantsResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = existingVariantsResult.StatusCode,
                    InternalErrorMessage = existingVariantsResult.InternalErrorMessage,
                };

            // Check for duplicate attributes in the input list
            var duplicateVariants = variantReqs
                .Where(req =>
                    existingVariantsResult.Data.Any(existing =>
                        existing.AttributeName == req.AttributeName
                    )
                )
                .ToList();

            if (duplicateVariants.Count != 0)
                variantReqs = variantReqs
                    .Where(req =>
                        duplicateVariants.All(duplicate =>
                            duplicate.AttributeName != req.AttributeName
                        )
                    )
                    .ToList();

            // Map incoming DTOs to domain models
            var variantsToCreate = variantReqs
                .Select(req => new VariantAttribute
                {
                    AttributeName = req.AttributeName,
                    CreatedAt = DateTime.Now,
                    CreatedById = currentUser.Data.UserId,
                })
                .ToList();

            // Perform bulk creation
            var bulkCreateResult = await variantRepository.BulkCreateVariantAttribute(
                variantsToCreate
            );

            if (!bulkCreateResult.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = bulkCreateResult.StatusCode,
                    InternalErrorMessage = bulkCreateResult.InternalErrorMessage,
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                TotalRecord = bulkCreateResult.TotalRecord,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            // Handle unexpected exceptions
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> UpdateVariantAttributeAsync(
        Guid variantId,
        VariantReqDto variantReq
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

            var existingVariant = await variantRepository.GetVariantAttributeById(variantId);
            if (!existingVariant.IsSuccess || existingVariant.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeNotFound,
                    InternalErrorMessage = existingVariant.InternalErrorMessage,
                };
            // Check if the attribute name is the same
            if (existingVariant.Data.AttributeName == variantReq.AttributeName)
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                };

            // Check if the attribute name already exists
            var existingVariantsResult = await variantRepository.GetVariantAttributes();
            if (
                existingVariantsResult is { IsSuccess: true, Data: not null }
                && existingVariantsResult.Data.Any(x => x.AttributeName == variantReq.AttributeName)
            )
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeAlreadyExists,
                    InternalErrorMessage = existingVariantsResult.InternalErrorMessage,
                };

            existingVariant.Data.AttributeName = variantReq.AttributeName;
            existingVariant.Data.ModifiedAt = DateTime.Now;
            existingVariant.Data.ModifiedById = currentUser.Data.UserId;

            return await variantRepository.UpdateVariantAttribute(existingVariant.Data);
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

    public async Task<Return<bool>> DeleteVariantAttributeAsync(Guid variantId)
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

            var existingVariant = await variantRepository.GetVariantAttributeById(variantId);
            if (!existingVariant.IsSuccess || existingVariant.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeNotFound,
                    InternalErrorMessage = existingVariant.InternalErrorMessage,
                };

            // Check if the attribute is used in any product variant
            if (existingVariant.Data.ProductVariants.Count != 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeConflict,
                };

            return await variantRepository.DeleteVariantAttributes(existingVariant.Data);
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
