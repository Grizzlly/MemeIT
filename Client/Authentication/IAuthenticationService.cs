using MemeIT.Shared.Models.Authentication;
using System.Threading.Tasks;

namespace MemeIT.Client.Authentication
{
    public interface IAuthenticationService
    {
        Task Login(LoginModel loginModel);
        Task Logout();

        Task<(bool, string)> Register(RegisterModel registerModel);
    }
}