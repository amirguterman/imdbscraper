using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ImdbListingExercise.Models.Imdb;
using ImdbListingExercise.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace ImdbListingExercise.Services
{
    public class LocalImdbService : IImdbService
    {
        private readonly ILogger<LocalImdbService> logger;
        private readonly IStorageService<Actor> storageService;
        private readonly WebScraper scraper;
        public LocalImdbService(ILogger<LocalImdbService> logger, IStorageService<Actor> storageService, WebScraper scraper)
        {
            this.scraper = scraper;
            this.logger = logger;
            this.storageService = storageService;

            scraper.Set(ListActors());
            
            scraper
                .Subscribe(actor => storageService.Add(
                    Enumerable.Repeat<Actor>(actor, 1)));
        }

        public void Delete(string id)
        {
            this.storageService.Delete(id);
            this.scraper.Set(ListActors());
        }

        public List<Actor> ListActors() => this.storageService.List().ToList();

        public async Task StartSync()
        {
            var actors = await this.scraper.Run(ListActors());
            storageService.Add(actors);
        }

        public void StopSync()
        {
            this.scraper.StopSync();
        }

        public async Task Reset()
        {
            storageService.Reset();
            await StartSync();
        }
    }
}