#region NameSpaces
using Blazored.LocalStorage;
using BlazorFrontend.Auth;
using BlazorFrontend.Models.Base;
using BlazorFrontend.Models.Dtos;
using BlazorFrontend.Models.Entities;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace BlazorFrontend.Services;
#endregion

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient,
        ILocalStorageService localStorageService,
        CustomAuthStateProvider authStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/User/Register", model);
        var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<AuthResponse>>();
        var result = apiResult?.Data;
        if (response.IsSuccessStatusCode && result?.Token is not null)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            _authStateProvider.NotifyUserAuthentication(result.Token);
        }
        return result;
    }

    public async Task<AuthResponse?> LoginAsync1(LoginRequest model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/User/Login", model);
        var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<AuthResponse>>();
        var result = apiResult?.Data;
        if (response.IsSuccessStatusCode && result?.Token is not null)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            _authStateProvider.NotifyUserAuthentication(result.Token);
        }
        return new AuthResponse() { Message = apiResult!.Message!, Success = apiResult!.IsSuccess, ExpiresAt = result!.ExpiresAt!, Token = result?.Token };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/User/Login", model);
            var apiResult = await response.Content.ReadFromJsonAsync<ApiResult<AuthResponse>>();
            var result = apiResult?.Data;
            if (response.IsSuccessStatusCode && result?.Token is not null)
            {
                await _localStorage.SetItemAsync("authToken", result.Token);
                _authStateProvider.NotifyUserAuthentication(result.Token);
            }
            return new AuthResponse
            {
                Success = apiResult?.IsSuccess ?? false,
                Message = apiResult?.Message ?? "",
                Token = result?.Token,
                ExpiresAt = result?.ExpiresAt
            };
        }
        catch (Exception)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "سرویس دهنده در دسترس نمی باشد.",
                Token = null,
                User = null
            };
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorageService.RemoveItemAsync("authToken");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _authStateProvider.NotifyUserLogout();
    }
}