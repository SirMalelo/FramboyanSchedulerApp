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
    // For now, use localhost for development
    // In production, this will be replaced by the build process
    var apiBaseUrl = "http://localhost:5117";
    
    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});

builder.Services.AddScoped<AuthenticatedHttpClient>();

await builder.Build().RunAsync();