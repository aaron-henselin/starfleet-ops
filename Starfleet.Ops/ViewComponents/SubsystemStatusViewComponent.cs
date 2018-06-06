using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Domain.Rules;

namespace Starfleet.Ops.ViewComponents
{


    public class ComponentStatusViewModel
    {
        public string ComponentDisplayName { get; set; }
        public int MaxHealth { get; set; }
        public int RemainingHealth { get; set; }
    }

    public class SubsystemStatusViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Pawn pawn, string subsystem)
        {
            List<ComponentStatusViewModel> vms = new List<ComponentStatusViewModel>();
            var componentInfo = GameRules.GetComponents().ToDictionary(x => x.Code);
            var spec = GameRules.GetShipByCode(pawn.SpecificationCode);
            var componentsInSubsystem = spec.Components.Where(x => componentInfo[x.Key].ComponentType == subsystem);
           
            foreach (var component in componentsInSubsystem)
            {
                var componentCode = component.Key;

                if (spec.Components[componentCode] == 0)
                    continue;

                var status =
                new ComponentStatusViewModel
                {
                    ComponentDisplayName = componentInfo[componentCode].Code,
                    MaxHealth = spec.Components[componentCode],
                    RemainingHealth = pawn.ComponentsRemaining[componentCode]

                };

                vms.Add(status);
            }
            //@foreach(var w in weapons)
            //{
            //    < h5 > @GameRules.GetComponentByCode(w.Key).Code(@GameRules.GetComponentByCode(w.Key).Name) </ h5 >

            //        var remaining = pawn.ComponentsRemaining[w.Key];

            //    @await Component.InvokeAsync("HealthBar", new { max = w.Value, current = remaining })
            //}

            return View(vms);
        }
    }
}
