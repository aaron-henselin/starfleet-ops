using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Domain.Rules;
using Starfleet.Ops.Infrastructure;
using Starfleet.Ops.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Starfleet.Ops.Controllers
{
    public class GameStateSpecificationViewModel
    {
        public List<SelectListItem> AllShips { get; set; } = new List<SelectListItem>();
        public List<ShipSelection> SelectedShips { get; set; } = new List<ShipSelection>();
        public string GameName { get; set; }
        public Guid? Id { get; internal set; }
    }

    public class ShipSelection
    {
        public ShipSelection()
        { }

        public ShipSelection(Pawn p)
        {
            ShipId = p.Id;
            Code = p.SpecificationCode;
            Name = p.Name;
        }

        public Guid? ShipId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }


    public class GameStateSpecificationBuilderController : Controller
    {
        private readonly GameStateRepository _gsRepo;

        public GameStateSpecificationBuilderController(GameStateRepository gsRepo)
        {
            _gsRepo = gsRepo;
        }

        private IEnumerable<SelectListItem> CreateShipOptions()
        {
            var blankOption = new SelectListItem{Text="-- Select Ship --",Value=string.Empty};
            var baseOptions = GameRules.GetShips().Select(CreateShipOption).ToList();

            return new[] {blankOption}.Concat(baseOptions);
        }

        private SelectListItem CreateShipOption(ShipSpecification x)
        {
            return new SelectListItem
            {
                Text = x.Faction + " - " + x.Name,
                Value = x.Code,
            };
        }

        [HttpGet]
        public IActionResult Index(Guid? id)
        {
            var vm = new GameStateSpecificationViewModel
            {
                Id = id,
                AllShips = CreateShipOptions().ToList(),
                SelectedShips = new List<ShipSelection>()
            };

            GameState existingGameState;
            if (id != null)
            {
                existingGameState = _gsRepo.GetById(id.Value).Result;
                foreach (var p in existingGameState.Pawns)
                    vm.SelectedShips.Add(new ShipSelection(p));
            }

            RemoveEmptyEntriesAndEnsureLastOptionEmpty(vm);

            return View(vm);
        }

        private static void RemoveEmptyEntriesAndEnsureLastOptionEmpty(GameStateSpecificationViewModel vm)
        {
            vm.SelectedShips.RemoveAll(x => string.IsNullOrWhiteSpace(x.Code));
            if (!vm.SelectedShips.Any() || !string.IsNullOrWhiteSpace(vm.SelectedShips.Last().Code))
                vm.SelectedShips.Add(new ShipSelection());
        }

        [HttpPost]
        public IActionResult Index(Guid? id,GameStateSpecificationViewModel vm)
        {
            vm.Id = id;
            vm.AllShips = CreateShipOptions().ToList();
            RemoveEmptyEntriesAndEnsureLastOptionEmpty(vm);

            return View(vm);
        }

        [HttpPost]
        public IActionResult BeginGame(Guid? id,GameStateSpecificationViewModel vm)
        {
            var shipsToCreate = vm.SelectedShips.Where(x => !string.IsNullOrWhiteSpace(x.Code));

            var gs = new GameState
            {
                Id = id ?? Guid.NewGuid()
            };
            foreach (var shipToCreate in shipsToCreate)
            {
                var spec = GameRules.GetShipByCode(shipToCreate.Code);
                var pawn = Pawn.FromSpecification(spec);
                pawn.GameStateId = gs.Id.Value;
                pawn.Name = shipToCreate.Name;
                gs.Pawns.Add(pawn);
            }

        
            _gsRepo.Save(gs);

            return View(gs);
        }

    }
}
