using Unidecode.NET;

namespace SmE_CommerceUtilities.Utils;

public abstract class VietnameseStringNormalizer
{
    public static string Normalize(string str = "")
    {
        return string.IsNullOrEmpty(str) ? str : str.Unidecode().ToLower();
    }
}
