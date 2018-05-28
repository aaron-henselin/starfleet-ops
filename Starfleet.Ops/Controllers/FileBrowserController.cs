using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starfleet.Ops.Utility;

namespace Starfleet.Ops.Controllers
{
    public class FileBrowserController : Controller
    {
        public IActionResult Index()
        {




            return View();
        }
    }
}
