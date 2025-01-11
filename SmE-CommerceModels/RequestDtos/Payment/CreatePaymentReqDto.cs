using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Payment;

public class CreatePaymentReqDto
{
    [Required]
    public required Guid PaymentMethodId { get; set; }
    
    [Required]
    public required Guid OrderId { get; set; }
    
    [Required]
    public required decimal Amount { get; set; }
}