using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorBattles.Client
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Vasya")
            }, "test auth type");

            var user = new ClaimsPrincipal(identity);
            //return Task.FromResult(new AuthenticationState(user));
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        }
    }
}
