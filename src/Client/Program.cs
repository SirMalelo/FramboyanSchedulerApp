using Microsoft.AspNetCore.Components.Authorization;
using Client.Auth;
using Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client; // App.razor namespace
using System.Net.Http;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

// Environment-aware HTTP client configuration
builder.Services.AddScoped(sp =>
{
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    var apiBaseUrl = "http://localhost:5117"; // Default fallback
    
    try
    {
        // Try to get from window.appConfig if available (production)
        var configBaseUrl = jsRuntime.InvokeAsync<string>("eval", "window.appConfig?.apiBaseUrl").Result;
        if (!string.IsNullOrEmpty(configBaseUrl) && !configBaseUrl.Contains("#{"))
        {
            apiBaseUrl = configBaseUrl;
        }
    }
    catch
    {
        // Fall back to default for development
    }
    
    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});

builder.Services.AddScoped<AuthenticatedHttpClient>();

await builder.Build().RunAsync();