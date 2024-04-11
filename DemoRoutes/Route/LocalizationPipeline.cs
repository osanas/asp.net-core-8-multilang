using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

public static class LocalizationPipeline
{
    public static WebApplication UseRequestLocalizationWithSettings(this WebApplication app, RequestLocalizationOptions options)
    {
        app.UseRequestLocalization(options);
        return app;
    }
}
