using Blazored.LocalStorage;
using MemeIT.Client.Services;
using MemeIT.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MemeIT.Client.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient httpClient;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILocalStorageService localStorageService;

        public AuthenticationService(HttpClient httpClient,
                                     AuthenticationStateProvider authenticationStateProvider,
                                     ILocalStorageService localStorageService)
        {
            this.httpClient = httpClient;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorageService = localStorageService;
        }

        public async Task Login(LoginModel loginModel)
        {
            StringContent content = ApiService.PrepareStringContent(loginModel);

            using HttpResponseMessage responseMessage = await httpClient.PostAsync("api/authentication/login", content);

            if (responseMessage.IsSuccessStatusCode is false)
            {
                return;
            }

            JwtToken tkn = JsonSerializer.Deserialize<JwtToken>(
                await responseMessage.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            await localStorageService.SetItemAsync("authToken", tkn.Token);

            (authenticationStateProvider as AuthStateProvider)?.NotifyUserAuthentication(tkn.Token);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tkn.Token);
        }

        public async Task Logout()
        {
            await localStorageService.RemoveItemAsync("authToken");

            (authenticationStateProvider as AuthStateProvider)?.NotifyUserLogout();

            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<(bool, string)> Register(RegisterModel registerModel)
        {
            StringContent content = ApiService.PrepareStringContent(registerModel);

            using HttpResponseMessage responseMessage = await httpClient.PostAsync("api/authentication/register", content);

            Task<AuthResponse> response = responseMessage.Content.ReadFromJsonAsync<AuthResponse>()!;

            if (responseMessage.IsSuccessStatusCode is true)
            {
                await Login(new()
                {
                    UserName = registerModel.UserName,
                    Password = registerModel.Password
                });

                return ((await authenticationStateProvider.GetAuthenticationStateAsync()).User!.Identity!.IsAuthenticated, "");
            }
            else
            {
                return (false, (await response).Message);
            }
        }
    }
}
