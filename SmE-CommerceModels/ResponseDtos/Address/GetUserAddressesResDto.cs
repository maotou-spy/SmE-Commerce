namespace SmE_CommerceModels.ResponseDtos.Address;

public class GetUserAddressesResDto
{
    public Guid AddressId { get; set; }
    public string ReceiverName { get; set; } = null!;
    public string ReceiverPhone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Ward { get; set; } = null!;
    public string District { get; set; } = null!;
    public string City { get; set; } = null!;
    public bool IsDefault { get; set; } = false;
}
