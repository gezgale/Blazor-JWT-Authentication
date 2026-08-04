#region NameSpaces
using Blazored.LocalStorage;
using Blazored.Toast;
using BlazorFrontend;
using BlazorFrontend.Auth;
using BlazorFrontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddTransient<UnauthorizedHandler>();
builder.RootComponents.Add<BlazorFrontend.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:55281/";
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>()
.AddHttpMessageHandler<UnauthorizedHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("ApiClient"));
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddBlazoredToast();
await builder.Build().RunAsync();