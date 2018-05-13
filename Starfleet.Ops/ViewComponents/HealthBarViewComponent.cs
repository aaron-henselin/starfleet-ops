using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Starfleet.Ops.ViewComponents
{
    public class HealthBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int max, int current)
        {
            return View(new HealthBarViewModel { Max = max, Current = current });
        }
    }

    public class HealthBarViewModel
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }


}
