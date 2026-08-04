using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorFrontend.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);
            if (IsTokenExpired(token))
            {
                await _localStorageService.RemoveItemAsync("authToken");
                return new AuthenticationState(_anonymous);
            }
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        //public override async Task<AuthenticationState> GetAuthenticationState1Async()
        //{
        //    var token = await _localStorageService.GetItemAsync<string>("authToken");
        //    if (string.IsNullOrWhiteSpace(token))
        //        return new AuthenticationState(_anonymous);

        //    var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        //    return new AuthenticationState(new ClaimsPrincipal(identity));
        //}

        //public void NotifyUserAuthentication(string token)
        //{
        //    var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        //    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
        //}

        public void NotifyUserAuthentication(string token)
        {
            if (IsTokenExpired(token))
            {
                NotifyUserLogout();
                return;
            }
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }

        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo <= DateTime.UtcNow;
        }
    }
}