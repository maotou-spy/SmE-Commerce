using SmE_CommerceModels.Models;
using Unidecode.NET;

namespace SmE_CommerceUtilities.Utils;

public abstract class StringUtils
{
    public static string SimplifyText(string str = "")
    {
        return string.IsNullOrEmpty(str) ? str : str.Unidecode().ToLower();
    }

    public static string CreateFullAddressString(Address? address)
    {
        return address == null
            ? string.Empty
            : string.Join(
                ", ",
                new[] { address.Address1, address.Ward, address.District, address.City }
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(part => part.Trim())
            );
    }
}
