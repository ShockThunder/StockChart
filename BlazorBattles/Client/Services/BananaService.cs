using System.Net.Http.Json;

namespace BlazorBattles.Client.Services;

public class BananaService : IBananaService
{
    private readonly HttpClient _httpClient;

    public BananaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public event Action OnChange;
    public int Bananas { get; set; } = 1000;
    public void EatBananas(int amount)
    {
        Bananas -= amount;
        BananasChanged();
    }

    public void AddBananas(int amount)
    {
        Bananas += amount;
        BananasChanged();
    }

    private void BananasChanged() => OnChange.Invoke();

    public async Task GetBananas()
    {
        Bananas = await _httpClient.GetFromJsonAsync<int>("api/user/getbananas");
        BananasChanged();
    }
}