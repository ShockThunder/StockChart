using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorBattles.Client
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        public CustomAuthProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        private readonly ILocalStorageService _localStorageService;
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = new AuthenticationState(new ClaimsPrincipal());
            if (await _localStorageService.GetItemAsync<bool>("isAuthenticated"))
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "Vasya")
                }, "test auth type");

                var user = new ClaimsPrincipal(identity);
                state = new AuthenticationState(user);
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
            
        }
    }
}
