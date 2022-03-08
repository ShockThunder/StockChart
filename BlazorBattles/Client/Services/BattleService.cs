﻿using BlazorBattles.Shared;
using System.Net.Http.Json;

namespace BlazorBattles.Client.Services
{
    public class BattleService : IBattleService
    {
        private readonly HttpClient _httpClient;

        public BattleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public BattleResult LastBattle { get; set; } = new BattleResult();

        public async Task<BattleResult> StartBattle(int opponentId)
        {
            var result = await _httpClient.PostAsJsonAsync("api/battle", opponentId);

            LastBattle = await result.Content.ReadFromJsonAsync<BattleResult>();

            return LastBattle;
        }
    }
}