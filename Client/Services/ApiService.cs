using System.Text.Json;
using System.Text;
using MemeIT.Shared.Models;
using System.Net.Http.Json;
using static MudBlazor.Colors;

namespace MemeIT.Client.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient httpClient;

        public static StringContent PrepareStringContent(object obj)
        {
            return new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        public ApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<MemeDto>> GetMemes()
        {
            List<MemeDto>? memes = null;

            using HttpResponseMessage respone = await httpClient.GetAsync("api/memes");

            if(respone.IsSuccessStatusCode)
            {
                memes = (await respone.Content.ReadFromJsonAsync<IEnumerable<MemeDto>>())?.ToList();
            }

            return memes ?? new List<MemeDto>();
        }

        public async Task<MemeDto?> GetMeme(Guid id)
        {
            MemeDto? meme = null;

            using HttpResponseMessage respone = await httpClient.GetAsync($"api/memes/{id}");

            if (respone.IsSuccessStatusCode)
            {
                meme = await respone.Content.ReadFromJsonAsync<MemeDto?>();
            }

            return meme;
        }

        public async Task<bool> PostMeme(MemeDto meme)
        {
            using HttpResponseMessage respone = await httpClient.PostAsync($"api/memes", PrepareStringContent(meme));

            return respone.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMeme(Guid memeid)
        {
            using HttpResponseMessage respone = await httpClient.DeleteAsync($"api/memes/{memeid}");

            return respone.IsSuccessStatusCode;
        }
    }
}
