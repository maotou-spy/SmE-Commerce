using SlugifyNet;

namespace SmE_CommerceUtilities;

public static class SlugUtil
{
    public static string GenerateSlug(string phrase)
    {
        return phrase.GenerateSlug(locale: "vi");
    }
}
