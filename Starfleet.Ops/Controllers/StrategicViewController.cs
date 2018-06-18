using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Domain.Rules;
using Starfleet.Ops.Infrastructure;

namespace Starfleet.Ops.Controllers
{
    public class StrategicViewModel
    {
        public List<Pawn> Unassigned { get; set; }= new List<Pawn>();
        public List<FleetViewModel> AllFleets { get; set; } = new List<FleetViewModel>();
        public GameState GameState { get; set; }
        
    }

    public class CreateFleetViewModel
    {
        public string Name { get; set; }
    }
    

    public class FleetViewModel
    {
        public string Name { get; set; }
        public List<Pawn> Assigned { get; set; }= new List<Pawn>();
        public Guid Id { get; set; }
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
            strategicView.GameState = gs;

            foreach (var f in gs.Fleets)
            {
                var fleetVm = new FleetViewModel();
                fleetVm.Id = f.Id.Value;
                fleetVm.Name = f.Name;
                fleetVm.Assigned = gs.Pawns.Where(x => f.Id == x.FleetId).ToList();
                strategicView.AllFleets.Add(fleetVm);
            }

            foreach (var p in gs.Pawns)
            {
                var parentFleet = gs.Fleets.FirstOrDefault(x => x.Id == p.FleetId);
                if (parentFleet == null)
                    strategicView.Unassigned.Add(p);

            }

            return View(strategicView);
        }

        public IActionResult AssignToFleet(Guid id, Guid shipId, Guid fleetId)
        {
            var gs = gsRepo.GetById(id).Result;
            var pawn = gs.Pawns.First(x => x.Id == shipId);
            pawn.FleetId = fleetId;
            gsRepo.Save(gs);
            return RedirectToAction("Index", "StrategicView", new { id });
        }

        public IActionResult CreateFleet(Guid id, CreateFleetViewModel vm)
        {
            var gs = gsRepo.GetById(id).Result;
            gs.Fleets.Add(new Fleet
            {
                Name = vm.Name
            });
            gsRepo.Save(gs);
            return RedirectToAction("Index","StrategicView",new{id});
        }

        public IActionResult RepairFleet(Guid id, Guid fleetId)
        {
            var gs = gsRepo.GetById(id).Result;
            var pawns = gs.Pawns.Where(x => x.FleetId == fleetId).ToList();
            foreach (var pawn in pawns)
            {
                var spec = GameRules.GetShipByCode(pawn.SpecificationCode);
                pawn.Repair(spec);
            }
            gsRepo.Save(gs);
            return RedirectToAction("Index", "StrategicView", new { id });
        }
    }


}