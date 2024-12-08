using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.VariantAttribute;
using SmE_CommerceModels.ResponseDtos.VariantAttribute;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class VariantAttributeService(
    IVariantAttributeRepository attributeRepository,
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

            var attributesResult = await attributeRepository.GetVariantAttributes();
            if (!attributesResult.IsSuccess || attributesResult.Data == null)
                return new Return<List<ManagerVariantAttributeResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = attributesResult.StatusCode,
                    InternalErrorMessage = attributesResult.InternalErrorMessage,
                };

            var attributes = attributesResult
                .Data.Select(attribute => new ManagerVariantAttributeResDto
                {
                    AttributeId = attribute.VariantId,
                    AttributeName = attribute.AttributeName,
                    CreatedById = attribute.CreatedById,
                    CreatedAt = attribute.CreatedAt,
                    CreatedBy = attribute.CreatedBy?.FullName,
                    ModifiedById = attribute.ModifiedById,
                    ModifiedAt = attribute.ModifiedAt,
                    ModifiedBy = attribute.ModifiedBy?.FullName,
                })
                .ToList();

            return new Return<List<ManagerVariantAttributeResDto>>
            {
                Data = attributes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = attributes.Count,
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

    public async Task<Return<bool>> BulkCreateVariantAttributeAsync(
        List<AttributeReqDto> attributeReqs
    )
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
            attributeReqs = attributeReqs
                .GroupBy(req => req.AttributeName)
                .Select(group => group.First())
                .ToList();

            // Fetch existing attributes
            var existingAttributesResult = await attributeRepository.GetVariantAttributes();
            if (!existingAttributesResult.IsSuccess || existingAttributesResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = existingAttributesResult.StatusCode,
                    InternalErrorMessage = existingAttributesResult.InternalErrorMessage,
                };

            // Check for duplicate attributes in the input list
            var duplicateAttributes = attributeReqs
                .Where(req =>
                    existingAttributesResult.Data.Any(existing =>
                        existing.AttributeName == req.AttributeName
                    )
                )
                .ToList();

            if (duplicateAttributes.Count != 0)
                attributeReqs = attributeReqs
                    .Where(req =>
                        duplicateAttributes.All(duplicate =>
                            duplicate.AttributeName != req.AttributeName
                        )
                    )
                    .ToList();

            // Map incoming DTOs to domain models
            var attributesToCreate = attributeReqs
                .Select(req => new VariantAttribute
                {
                    AttributeName = req.AttributeName,
                    CreatedAt = DateTime.Now,
                    CreatedById = currentUser.Data.UserId,
                })
                .ToList();

            // Perform bulk creation
            var bulkCreateResult = await attributeRepository.BulkCreateVariantAttribute(
                attributesToCreate
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
        Guid attributeId,
        AttributeReqDto attributeReq
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

            var existingAttribute = await attributeRepository.GetVariantAttributeById(attributeId);
            if (!existingAttribute.IsSuccess || existingAttribute.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeNotFound,
                    InternalErrorMessage = existingAttribute.InternalErrorMessage,
                };
            // Check if the attribute name is the same
            if (existingAttribute.Data.AttributeName == attributeReq.AttributeName)
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                };

            // Check if the attribute name already exists
            var existingAttributes = await attributeRepository.GetVariantAttributes();
            if (
                existingAttributes is { IsSuccess: true, Data: not null }
                && existingAttributes.Data.Any(x => x.AttributeName == attributeReq.AttributeName)
            )
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeAlreadyExists,
                    InternalErrorMessage = existingAttributes.InternalErrorMessage,
                };

            existingAttribute.Data.AttributeName = attributeReq.AttributeName;
            existingAttribute.Data.ModifiedAt = DateTime.Now;
            existingAttribute.Data.ModifiedById = currentUser.Data.UserId;

            return await attributeRepository.UpdateVariantAttribute(existingAttribute.Data);
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

    public async Task<Return<bool>> DeleteVariantAttributeAsync(Guid attributeId)
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

            var existingAttribute = await attributeRepository.GetVariantAttributeById(attributeId);
            if (!existingAttribute.IsSuccess || existingAttribute.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantAttributeNotFound,
                    InternalErrorMessage = existingAttribute.InternalErrorMessage,
                };

            // Check if the attribute is used in any product variant
            if (existingAttribute.Data.ProductVariants.Count != 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.AttributeValueConflict,
                };

            return await attributeRepository.DeleteVariantAttributes(existingAttribute.Data);
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
