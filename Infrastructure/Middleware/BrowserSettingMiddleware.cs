using Chatbot.Core.Constants;
using Chatbot.Core.Extensions;
using Chatbot.Core.Service;

namespace Chatbot.Infrastructure.Middleware;

public class BrowserSettingMiddleware
{
    private readonly RequestDelegate _next;

    public BrowserSettingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, UserSettingService userSettingProvider)
    {
        var offsert = context.Request.Headers.GetValueAsInt(Constants.WebConstants.TIME_ZONE_OFFSET);
        userSettingProvider.TimeZoneOffset = offsert.GetValueOrDefault();
        if(context.Request.Headers.TryGetValue(Constants.WebConstants.TIME_ZONE_KEY, out var value))
            userSettingProvider.TimeZone = value.ToString();
        await _next(context);
    }
}

