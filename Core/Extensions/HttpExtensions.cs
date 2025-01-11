namespace Chatbot.Core.Extensions;

public static class HttpExtensions
{
    public static int? GetValueAsInt(this IHeaderDictionary httpHeader, string key)
    {
        return httpHeader.TryGetValue(key, out var value) &&
             int.TryParse(value.ToString(), out var parsed) ? parsed : null;
    }
}

