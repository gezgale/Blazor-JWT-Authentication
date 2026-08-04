using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace BlazorFrontend.Auth
{
    public class UnauthorizedHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigation;
        private readonly ILocalStorageService _localStorage;

        public UnauthorizedHandler(NavigationManager navigation, ILocalStorageService localStorage)
        {
            _navigation = navigation;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorage.RemoveItemAsync("authToken");
                _navigation.NavigateTo("/login", true);
            }
            return response;
        }
    }
}