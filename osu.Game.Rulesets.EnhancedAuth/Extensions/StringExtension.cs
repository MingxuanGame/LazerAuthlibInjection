namespace osu.Game.Rulesets.EnhancedAuth.Extensions;

public static class StringExtension
{
    public static string RemoveSuffix(this string text, string suffix)
    {
        return text.EndsWith(suffix) ? text[..^suffix.Length] : text;
    }

    public static string AddHttpsProtocol(this string url)
    {
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            return "https://" + url;
        return url;
    }
}
