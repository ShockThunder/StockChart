using BlazorBattles.Server.Data;
using BlazorBattles.Server.Services;
using BlazorBattles.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorBattles.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUtilityService _utilityService;

        public BattleController(DataContext context, IUtilityService utilityService)
        {
            _context = context;
            _utilityService = utilityService;
        }

        [HttpPost]
        public async Task<IActionResult> StartBattle([FromBody] int opponentId)
        {
            var attacker = await _utilityService.GetUser();
            var opponent = await _context.Users.FindAsync(opponentId);
            if (opponent == null || opponent.IsDeleted)
                return NotFound("Opponent not available.");

            var result = new BattleResult();
            result = await Fight(attacker, opponent, result);

            return Ok(result);
        }

        private async Task<BattleResult> Fight(User attacker, User opponent, BattleResult result)
        {
            var attackerArmy = await _context.UserUnits
                .Where(x => x.UserId == attacker.Id && x.HitPoints > 0)
                .Include(uu => uu.Unit)
                .ToListAsync();

            var opponentArmy = await _context.UserUnits
                .Where(x => x.UserId == opponent.Id && x.HitPoints > 0)
                .Include(uu => uu.Unit)
                .ToListAsync();

            var attackerDamageSum = 0;
            var opponentDamageSum = 0;

            int currentRound = 0;

            while(attackerArmy.Count > 0 && opponentArmy.Count > 0)
            {
                currentRound++;

                if (currentRound % 2 != 0)
                    attackerDamageSum += FightRound(attacker, opponent, attackerArmy, opponentArmy, result);
                else
                    opponentDamageSum += FightRound(opponent, attacker, opponentArmy, attackerArmy, result);
            }

            result.IsVictory = opponentArmy.Count == 0;
            result.RoundsFought = currentRound;

            if(result.RoundsFought > 0)
            {
                await FinishFight(attacker, opponent, result, attackerDamageSum, opponentDamageSum);
            }

            return result;
        }

        private async Task FinishFight(User attacker, User opponent, BattleResult result, int attackerDamageSum, int opponentDamageSum)
        {
            result.AttackerDamageSum = attackerDamageSum;
            result.OpponentDamageSum = opponentDamageSum;

            attacker.Battles++;
            opponent.Battles++;

            if (result.IsVictory)
            {
                attacker.Victories++;
                opponent.Defeats++;
                attacker.Bananas += opponentDamageSum;
                opponent.Bananas += attackerDamageSum * 10;
            }
            else
            {
                opponent.Victories++;
                attacker.Defeats++;
                opponent.Bananas += attackerDamageSum;
                attacker.Bananas += opponentDamageSum * 10;
            }

            StoreBattleHistory(attacker, opponent, result);

            await _context.SaveChangesAsync();
        }

        private void StoreBattleHistory(User attacker, User opponent, BattleResult result)
        {
            var battle = new Battle();
            battle.Attacker = attacker;
            battle.Opponent = opponent;
            battle.RoundsFought = result.RoundsFought;
            battle.WinnerDamage = result.IsVictory ? result.AttackerDamageSum : result.OpponentDamageSum;
            battle.Winner = result.IsVictory ? attacker : opponent;

            _context.Battles.Add(battle);
        }

        private int FightRound(User attacker, User opponent, List<UserUnit> attackerArmy, List<UserUnit> opponentArmy, BattleResult result)
        {
            var random = new Random();
            int randomAttackerIndex = random.Next(attackerArmy.Count);
            int randomOpponentIndex = random.Next(opponentArmy.Count);

            var randomAttacker = attackerArmy[randomAttackerIndex];
            var randomOpponent = opponentArmy[randomOpponentIndex];

            var damage =
                random.Next(randomAttacker.Unit.Attack) - random.Next(randomOpponent.Unit.Defense);

            if(damage < 0) damage = 0;

            if(damage <= randomOpponent.HitPoints)
            {
                randomOpponent.HitPoints -= damage;
                result.Log.Add(
                    $"{attacker.UserName}'s {randomAttacker.Unit.Title} attacks" +
                    $"{opponent.UserName}'s {randomOpponent.Unit.Title} with {damage} damage.");

                return damage;
            }
            else
            {
                damage = randomOpponent.HitPoints;
                randomOpponent.HitPoints = 0;

                opponentArmy.Remove(randomOpponent);

                result.Log.Add(
                    $"{attacker.UserName}'s {randomAttacker.Unit.Title} kills " +
                    $"{opponent.UserName}'s {randomOpponent.Unit.Title}!");

                return damage;

            }
        }
    }
}
