using BlazorBattles.Shared;

namespace BlazorBattles.Client.Services
{
    public interface ILeaderboardService
    {
        IList<UserStatistics> Leaderboard { get; set; }
        Task GetLeaderboard();
    }
}
