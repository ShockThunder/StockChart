using BlazorBattles.Shared;
using System.Net.Http.Json;

namespace BlazorBattles.Client.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly HttpClient _httpClient;

        public LeaderboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IList<UserStatistics> Leaderboard { get; set; }

        public async Task GetLeaderboard()
        {
            Leaderboard = await _httpClient.GetFromJsonAsync<IList<UserStatistics>>("api/user/leaderboard");
        }
    }
}
