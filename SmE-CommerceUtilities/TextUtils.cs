using System.Globalization;
using System.Text;

namespace SmE_CommerceUtilities;

public class TextUtils
{
    // Remove diacritics from a string for search purposes
    public static string RemoveDiacritics(string input)
    {
        // 1. Normalize string to FormD
        var normalizedString = input.Normalize(NormalizationForm.FormD);

        // 2. Remove all diacritics
        var stringBuilder = new StringBuilder();
        foreach (
            var c in from c in normalizedString
            let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
            where unicodeCategory != UnicodeCategory.NonSpacingMark
            select c
        )
            stringBuilder.Append(c);

        // 3. Return the modified string
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
