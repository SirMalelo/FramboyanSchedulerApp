using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

// Custom AuthenticationStateProvider for Blazor WebAssembly
// Reads JWT from localStorage and provides authentication state to the app
public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    // JS runtime for accessing browser localStorage
    private readonly IJSRuntime _js;
    // Represents an unauthenticated user
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    // Constructor: injects JS runtime
    public ApiAuthenticationStateProvider(IJSRuntime js)
    {
        _js = js;
    }

    // Returns the current authentication state
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Try to get the token from localStorage
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(_anonymous);

        // Parse the JWT token
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jwt = null;
        try
        {
            jwt = handler.ReadJwtToken(token);
        }
        catch
        {
            // If parsing fails, treat as anonymous
            return new AuthenticationState(_anonymous);
        }

        // Create claims principal from JWT claims
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    // Call this when a user logs in to update the authentication state
    public void NotifyUserAuthentication(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        // Notifies Blazor that the authentication state has changed
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    // Call this when a user logs out to update the authentication state
    public void NotifyUserLogout()
    {
        // Notifies Blazor that the user is now anonymous
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
