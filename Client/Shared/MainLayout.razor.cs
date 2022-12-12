using MemeIT.Client.Authentication;
using MemeIT.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MemeIT.Client.Shared
{
    public partial class MainLayout
    {
        private readonly LoginModel loginModel = new();
        private readonly RegisterModel registerModel = new();

        [Inject]
        private IAuthenticationService authenticationService { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private async void Login()
        {
            await authenticationService.Login(loginModel);

            if((await authenticationStateProvider.GetAuthenticationStateAsync())?.User?.Identity?.IsAuthenticated == true)
            {
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
        }

        private async void Register()
        {
            var response = await authenticationService.Register(registerModel);

            if(response.Item1 == true)
            {
                NavigationManager.NavigateTo("/", forceLoad: true);
            }
        }
    }
}
