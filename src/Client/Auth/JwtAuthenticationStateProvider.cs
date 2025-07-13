using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Client.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        public JwtAuthenticationStateProvider(IJSRuntime js)
        {
            _js = js;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            var identity = new ClaimsIdentity();
            if (!string.IsNullOrWhiteSpace(token))
            {
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken? jwt = null;
                try { jwt = handler.ReadJwtToken(token); } catch { }
                if (jwt != null && jwt.ValidTo > DateTime.UtcNow)
                {
                    identity = new ClaimsIdentity(jwt.Claims, "jwt");
                }
            }
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyAuthenticationStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
