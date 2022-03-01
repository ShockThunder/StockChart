using BlazorBattles.Server.Data;
using BlazorBattles.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorBattles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly DataContext _context;
        public UnitController(DataContext context)
        {
            _context = context;
        }
    
        [HttpGet]
        public async Task<IActionResult> GetUnits()
        {
            var units = await _context.Units.ToListAsync();
            return Ok(units);
        }

        [HttpPost]
        public async Task<IActionResult> AddUnit(Unit unit)
        {
            _context.Add(unit);
            await _context.SaveChangesAsync();
            return Ok(await _context.Units.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, Unit unit)
        {
            var dbUnit = await _context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if (dbUnit == null)
            {
                return NotFound();
            }

            dbUnit.Title = unit.Title;
            dbUnit.Attack = unit.Attack;
            dbUnit.BananaCost = unit.BananaCost;
            dbUnit.Defense = unit.Defense;
            dbUnit.HitPoints = unit.HitPoints;

            _context.Update(dbUnit);
            await _context.SaveChangesAsync();

            return Ok(dbUnit);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var unit = await _context.Units.FirstOrDefaultAsync(x => x.Id==id);
            if (unit == null)
                return NotFound();

            _context.Remove(unit);
            await _context.SaveChangesAsync();

            return Ok(await _context.Units.ToListAsync());
        }

    }
}
