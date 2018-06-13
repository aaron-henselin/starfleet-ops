using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Starfleet.Ops.Domain.GameState;
using Starfleet.Ops.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Starfleet.Ops.Controllers
{
    public class MainMenuViewModel
    {
        public IEnumerable<GameState> OpenGames { get; set; }
    }

    public class ReconfigureViewModel
    {
        public string NewConnectionString { get; set; }
    }

    public class MainMenuController : Controller
    {
        private readonly IWritableOptions<CosmosDbConfiguration> _options;
        private GameStateRepository _gsRepo;


        public MainMenuController(IWritableOptions<CosmosDbConfiguration> options, GameStateRepository gsRepo)
        {
            _options = options;
            _gsRepo = gsRepo;

           
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(_options.Value.ConnectionString))
                return RedirectToAction("Reconfigure");
            
            var openGames = _gsRepo.GetAll().Result;
            var mm = new MainMenuViewModel {OpenGames = openGames};

            return View(mm);
        }

        [HttpGet]
        public IActionResult Reconfigure()
        {
            if (!string.IsNullOrEmpty(_options.Value.ConnectionString))
                return RedirectToAction("Index");

            return View("Reconfigure");
        }

        [HttpPost]
        public IActionResult Reconfigure(ReconfigureViewModel config)
        {
            if (!string.IsNullOrEmpty(_options.Value.ConnectionString))
                return RedirectToAction("Index");

            _options.Update(opt => {
                opt.ConnectionString = config.NewConnectionString;
            });

            return RedirectToAction("Index");
        }
    }
}
