using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Infrastructure;

namespace Starfleet.Ops.Controllers
{
    public class StrategicViewModel
    {
        public List<Pawn> Unassigned { get; set; }= new List<Pawn>();
        public List<FleetViewModel> AllFleets { get; set; } = new List<FleetViewModel>();
        public GameState GameState { get; set; }
        
    }

    public class FleetViewModel
    {
        public string Name { get; set; }
        public List<Pawn> Assigned { get; set; }= new List<Pawn>();
    }

    public class StrategicViewController : Controller
    {
        private readonly GameStateRepository gsRepo;

        public StrategicViewController(GameStateRepository gsRepo)
        {
            this.gsRepo = gsRepo;
        }

        public IActionResult Index(Guid id)
        {
      
            var gs = gsRepo.GetById(id).Result;
            var strategicView = new StrategicViewModel();

            foreach (var f in gs.Fleets)
            {
                var fleetVm = new FleetViewModel();
                fleetVm.Name = f.Name;
                fleetVm.Assigned = gs.Pawns.Where(x => f.Id == x.FleetId).ToList();
            }

            foreach (var p in gs.Pawns)
            {
                var parentFleet = gs.Fleets.FirstOrDefault(x => x.Id == p.Id);
                if (parentFleet == null)
                    strategicView.Unassigned.Add(p);

            }

            return View(strategicView);
        }
    }
}