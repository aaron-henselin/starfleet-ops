using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Domain.Rules;
using Starfleet.Ops.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Starfleet.Ops.Controllers
{
    public class GameStateSpecificationViewModel
    {
        public List<SelectListItem> AllShips { get; set; } = new List<SelectListItem>();
        public List<ShipSelection> SelectedShips { get; set; } = new List<ShipSelection>();
    }

    public class ShipSelection
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }


    public class GameStateSpecificationBuilderController : Controller
    {
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
        public IActionResult Index()
        {
            var vm = new GameStateSpecificationViewModel
            {
              AllShips  = CreateShipOptions().ToList(),
              SelectedShips = new List<ShipSelection>()
            };
           
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
        public IActionResult Index(GameStateSpecificationViewModel vm)
        {
            vm.AllShips = CreateShipOptions().ToList();
            RemoveEmptyEntriesAndEnsureLastOptionEmpty(vm);

            return View(vm);
        }

        [HttpPost]
        public IActionResult BeginGame(GameStateSpecificationViewModel vm)
        {
            var shipsToCreate = vm.SelectedShips.Where(x => !string.IsNullOrWhiteSpace(x.Code));

            var gs = new GameState();
            foreach (var shipToCreate in shipsToCreate)
            {
                var spec = GameRules.GetShipByCode(shipToCreate.Code);
                var pawn = Pawn.FromSpecification(spec);
                pawn.Name = shipToCreate.Name;
                gs.Pawns.Add(pawn);
            }
            

            return View(gs);
        }

    }
}
