using BlazorBattles.Shared;

namespace BlazorBattles.Client.Services
{
    public interface IBattleService
    {
        Task<BattleResult> StartBattle(int opponentId);
    }
}
