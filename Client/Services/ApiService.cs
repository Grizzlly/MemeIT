using System.Text.Json;
using System.Text;

namespace MemeIT.Client.Services
{
    public class ApiService
    {
        public static StringContent PrepareStringContent(object obj)
        {
            return new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }
    }
}
