using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Newtonsoft.Json;
using Starfleet.Ops.Domain.Framework;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Domain.Rules;
using Starfleet.Ops.Utility;

namespace Starfleet.Ops.Controllers
{
    public class GameViewModel
    {
        public GameState GameState { get; set; }

        public string BrowserGameState { get; set; }
        public string ServerGameState => JsonConvert.SerializeObject(GameState);
        public bool IsGameResuming { get; set; }
        public int NewDamage { get; set; }
        public Guid NewDamageTarget { get; set; }
        public List<string> BattleLog { get; set; } = new List<string>();
    }

   

    public class GameController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            GameViewModel vm;
            vm = new GameViewModel();
            vm.IsGameResuming = true;

            return View(vm);
        }

        [HttpPost]
        public IActionResult ResumeGame(GameViewModel vm)
        {
            vm.GameState = JsonConvert.DeserializeObject<GameState>(vm.BrowserGameState);
            vm.IsGameResuming = false;

            return View("Index",vm);
        }

        public class SimulationResult
        {
            public int TrackNumber { get; set; }
            public string AffectedUnitCode { get; set; }
        }

        public class MultiRoundDamageAllocationSimulator
        {
            List<string> previousVolleys = new List<string>();

            public MultiRoundDamageAllocationSimulator()
            {
            }

            public SimulationResult TakeDamage(Pawn pawnToAffect)
            {
                var result = new SimulationResult();

                var d1 = DiceRoller.Roll();
                var d2 = DiceRoller.Roll();

                var trackNumber = d1 + d2;
                result.TrackNumber = trackNumber;

               var track = GameRules.GetDamageAllocationTrack(trackNumber);

                var health = pawnToAffect.ComponentsRemaining;


                bool damageTaken = false;
                foreach (var unit in track.Units)
                {
                    if (!health.ContainsKey(unit.Code) || health[unit.Code] == 0)
                        continue;

                    if (unit.OncePerVolley && previousVolleys.Contains(unit.Id))
                        continue;

                    health[unit.Code] -= 1;
                    result.AffectedUnitCode = unit.Code;

                    previousVolleys.Add(unit.Code);
                    damageTaken = true;
                    break;
                }

                //if (!damageTaken)
                //    pawnToAffect.ExcessDamage++;

                return result;
            }
        }

        [HttpPost]
        public IActionResult Action(GameViewModel vm)
        {
            vm.GameState = JsonConvert.DeserializeObject<GameState>(vm.BrowserGameState);
            var pawnToAffect = vm.GameState.Pawns.Single(x => x.Id == vm.NewDamageTarget);
            var internalRoles = vm.NewDamage;


            vm.BattleLog = new List<string>();

            var sim =  new MultiRoundDamageAllocationSimulator();
            for (int i = 0; i < internalRoles; i++)
            {
                var result = sim.TakeDamage(pawnToAffect);
                
                vm.BattleLog.Add(result.TrackNumber + " - " + result.AffectedUnitCode);
            }

            return View("Index",vm);
        }

       
        
    }


}