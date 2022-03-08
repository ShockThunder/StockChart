using BlazorBattles.Server.Data;
using BlazorBattles.Server.Services;
using BlazorBattles.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorBattles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilityService _utilityService;

        public UserController(DataContext context, IUtilityService utilityService)
        {
            _context = context;
            _utilityService = utilityService;
        }

        [HttpGet("getbananas")]
        public async Task<IActionResult> GetBananas()
        {
            var user = await _utilityService.GetUser();
            return Ok(user.Bananas);
        }

        [HttpPut("addbananas")]
        public async Task<IActionResult> AddBananas([FromBody] int bananas)
        {
            var user = await _utilityService.GetUser();
            user.Bananas += bananas;

            await _context.SaveChangesAsync();
            return Ok(user.Bananas);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var users = await _context.Users.Where(x => !x.IsDeleted && x.IsConfirmed).ToListAsync();

            users = users
                .OrderByDescending(x => x.Victories)
                .ThenBy(x => x.Defeats)
                .ThenBy(x => x.DateCreated)
                .ToList();

            var rank = 1;
            var response = users.Select(x => new UserStatistics
            {
                Rank = rank++,
                UserId = x.Id,
                Username = x.UserName,
                Battles = x.Battles,
                Victories = x.Victories,
                Defeats = x.Defeats
            });

            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var user = await _utilityService.GetUser();
            var battles = await _context.Battles.Where(x => x.AttackerId == user.Id || x.OpponentId == user.Id)
                .Include(b => b.Attacker)
                .Include(b => b.Opponent)
                .Include(b => b.Winner)
                .ToListAsync();

            var history = battles.Select(battle => new BattleHistoryEntry
            {
                AttackerId = battle.AttackerId,
                OpponentId = battle.OpponentId,
                BattleId = battle.Id,
                YouWon = battle.WinnerId == user.Id,
                AttackerName = battle.Attacker.UserName,
                OpponentName = battle.Opponent.UserName,
                RoundsFought = battle.RoundsFought,
                WinnerDamageDealt = battle.WinnerDamage,
                BattleDate = battle.BattleDate
            });

            return Ok(history.OrderByDescending(h => h.BattleDate));
        }
    }
}
