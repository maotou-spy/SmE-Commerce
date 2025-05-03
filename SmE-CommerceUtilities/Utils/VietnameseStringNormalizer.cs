using System.Globalization;
using System.Text;

namespace SmE_CommerceUtilities.Utils;

public class VietnameseStringNormalizer
{
    public static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (
            var c in from c in normalizedString
            let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
            where unicodeCategory != UnicodeCategory.NonSpacingMark
            select c
        )
            stringBuilder.Append(c);

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
    }
}
