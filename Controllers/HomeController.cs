using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ImdbListingExercise.Models;
using ImdbListingExercise.Models.Imdb;
using ImdbListingExercise.Services.Contracts;
using ImdbListingExercise.Services;

namespace ImdbListingExercise.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IImdbService imdbService;
        private readonly WebScraper scraper;

        public HomeController(ILogger<HomeController> logger, IImdbService imdbService, WebScraper scraper)
        {
            this.logger = logger;
            this.imdbService = imdbService;
            this.scraper = scraper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var actors = imdbService.ListActors().ToList();

            ViewData["Actors"] = actors;
            ViewData["IsSyncing"] = this.scraper.GetIsSyncing().ToString().ToLowerInvariant();
            ViewData["SyncStatus"] = this.scraper.SyncStatus.ToString().ToLowerInvariant();
            ViewData["SyncProgress"] = this.scraper.SyncProgress;
            ViewData["CurrentTask"] = this.scraper.CurrentTask;
            ViewData["ActorsCount"] = this.scraper.StoredActorsCount;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
