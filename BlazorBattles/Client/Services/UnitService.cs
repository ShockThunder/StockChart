﻿using BlazorBattles.Shared;
using Blazored.Toast.Services;
using System.Net;
using System.Net.Http.Json;

namespace BlazorBattles.Client.Services;

public class UnitService : IUnitService
{
    private readonly IToastService _toastService;
    private readonly HttpClient _httpClient;
    private readonly IBananaService _bananaService;

    public UnitService(IToastService toastService, HttpClient httpClient, IBananaService bananaService)
    {
        _toastService = toastService;
        _httpClient = httpClient;
        _bananaService = bananaService;
    }
    public IList<Unit> Units { get; set; } = new List<Unit>();
    public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();
    public async Task AddUnit(int unitId)
    {
        var unit = Units.First(unit => unit.Id == unitId);

        var result = await _httpClient.PostAsJsonAsync<int>("api/userunit", unitId);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            _toastService.ShowError(await result.Content.ReadAsStringAsync());
        }
        else
        {
            await _bananaService.GetBananas();
            _toastService.ShowSuccess($"Unit {unit.Title} has been built", "Unit built!");
        }
    }

    public async Task LoadUnitsAsync()
    {
        if(Units.Count == 0)
        {
            Units = await _httpClient.GetFromJsonAsync<IList<Unit>>("api/Unit");
        }
    }

    public async Task LoadUserUnitsAsync()
    {
        MyUnits = await _httpClient.GetFromJsonAsync<IList<UserUnit>>("api/userunit");
    }
}