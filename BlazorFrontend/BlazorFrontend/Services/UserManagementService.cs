using BlazorFrontend.Models.Base;
using BlazorFrontend.Models.Dtos;
using System.Net.Http.Json;
namespace BlazorFrontend.Services;
public class UserManagementService
{
    private readonly HttpClient _http;
    public UserManagementService(HttpClient http) { _http = http; }
    public async Task<List<UserManagementDto>?> GetUsersAsync()
    {
        var r = await _http.GetFromJsonAsync<ApiResult<List<UserManagementDto>>>("api/v1/UserManagement/GetUsers");
        return r?.Data;
    }
}