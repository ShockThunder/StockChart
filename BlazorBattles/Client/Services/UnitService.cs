using BlazorBattles.Shared;
using Blazored.Toast.Services;

namespace BlazorBattles.Client.Services;

public class UnitService : IUnitService
{
    private readonly IToastService _toastService;
    public UnitService(IToastService toastService)
    {
        _toastService = toastService;
    }
    public IList<Unit> Units { get; set; } = new List<Unit>
    {
        new()
        {
            Id = 1,
            Title = "Knight",
            Attack = 10,
            Defense = 10,
            BananaCost = 100
        },
        new()
        {
            Id = 2,
            Title = "Archer",
            Attack = 15,
            Defense = 5,
            BananaCost = 150
        },
        new()
        {
            Id = 3,
            Title = "Mage",
            Attack = 20,
            Defense = 1,
            BananaCost = 200
        }
    };
    public IList<UserUnit> MyUnits { get; set; } = new List<UserUnit>();
    public void AddUnit(int unitId)
    {
        var unit = Units.First(unit => unit.Id == unitId);
        MyUnits.Add(new UserUnit(){UnitId = unit.Id, HitPoints = unit.HitPoints});
        _toastService.ShowSuccess($"Unit {unit.Title} has been built", "Unit built!");
    }
}