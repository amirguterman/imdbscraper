using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ImdbListingExercise.Models.Imdb;
using ImdbListingExercise.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImdbListingExercise.Services;
using ImdbListingExercise.Models.Protocol;

namespace ImdbListingExercise.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ILogger<ActorsController> logger;
        private readonly IImdbService imdbService;
        private readonly WebScraper scraper;

        public ActorsController(ILogger<ActorsController> logger, IImdbService imdbService, WebScraper scraper)
        {
            this.logger = logger;
            this.imdbService = imdbService;
            this.scraper = scraper;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            try
            {
                var actors = this.imdbService.ListActors();
                return new JsonResult(actors);
            }
            catch(Exception e)
            {
                
                this.logger.LogError($"Could not get actors list", e);
                return StatusCode(500, Enumerable.Empty<Actor>());
            }
        }

        
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            try
            {
                this.imdbService.Delete(id);
                return StatusCode(204);
            }
            catch(Exception e)
            {
                this.logger.LogError($"Could not delete actor with id {id}", e);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            try
            {
                var status = new StatusResponse 
                { 
                    Status = this.scraper.SyncStatus,
                    Progress = this.scraper.SyncProgress,
                    CurrentTask = this.scraper.CurrentTask,
                    ActorsCount = this.scraper.StoredActorsCount
                };

                return new JsonResult(status);
            }
            catch(Exception e)
            {
                this.logger.LogError($"Could not get status", e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Reset()
        {
            try
            {
                this.imdbService.Reset();
                return StatusCode(204);
            }
            catch(Exception e)
            {
                this.logger.LogError($"Could not reset actors list", e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult StartSync()
        {
            try
            {
                this.imdbService.StartSync();
                return StatusCode(204);
            }
            catch(Exception e)
            {
                this.logger.LogError($"Could not reset actors list", e);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult StopSync()
        {
            try
            {
                this.imdbService.StopSync();
                return StatusCode(204);
            }
            catch(Exception e)
            {
                this.logger.LogError($"Could not reset actors list", e);
                return StatusCode(500);
            }
        }

    }
}