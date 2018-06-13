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
using Starfleet.Ops.Infrastructure;
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


    }

    public class BattleAction
    {
        public Guid PawnId { get; set; }

        public int NumRolls { get; set; }

        public string BrowserGameState { get; set; }
        public GameState GameState { get; internal set; }
    }

    public class GameController : Controller
    {
        private readonly GameStateRepository _gsRepo;

        public GameController(GameStateRepository gsRepo)
        {
            _gsRepo = gsRepo;
        }
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    GameViewModel vm;
        //    vm = new GameViewModel();
        //    vm.IsGameResuming = true;

        //    return View(vm);
        //}


        public IActionResult ResumeGame(Guid id)
        {
          
            var gs = _gsRepo.GetById(id);

            var vm = new GameViewModel
            {
                GameState = gs.Result,
                IsGameResuming = false
            };

            return View("Index", vm);
        }

        public class SimulationResult
        {
            public bool Allocated { get; set; }
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

                result.Allocated = damageTaken;
                return result;
            }
        }

        [HttpPost]
        public PartialViewResult Action(Guid id,BattleAction vm)
        {
          
            var gs = _gsRepo.GetById(id).Result;

           
            var pawnToAffect = gs.Pawns.Single(x => x.Id == vm.PawnId);
            var internalRoles = vm.NumRolls;


            var localLog= new List<string>();
            localLog.Add($"== Simulating {vm.NumRolls} Internals ==");
            var sim =  new MultiRoundDamageAllocationSimulator();
            for (int i = 0; i < internalRoles; i++)
            {
                var result = sim.TakeDamage(pawnToAffect);
                localLog.Add(result.TrackNumber + " - " + result.AffectedUnitCode);
            }

            pawnToAffect.BattleLog.InsertRange(0,localLog);

            _gsRepo.Save(gs);

            vm.GameState = gs;
            this.ViewBag.PostActionGameState = JsonConvert.SerializeObject(vm.GameState);

            return PartialView("_Pawn",pawnToAffect);
        }

       
        
    }


}