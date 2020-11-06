using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using ImdbListingExercise.Models.Imdb;
using ImdbListingExercise.Models.Imdb.Enumerations;
using ImdbListingExercise.Services.Contracts;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace ImdbListingExercise.Services
{
    public class WebScraper : IScraper<Actor>
    {
        public enum Status
        {
            Idle = 0,
            Sync = 1
        }
        
        public static readonly string ACTORS_LIST_URL = @"https://www.imdb.com/list/ls052283250/";
        public static readonly string ACTOR_PAGE_URL = @"https://www.imdb.com{0}";
        public static readonly string FEMALE_ACTORS_SEARCH_URL = @"https://www.imdb.com/search/name/?name={0}&gender=female";

        public ScraperStatus SyncStatus { get; private set; }
        public decimal SyncProgress => TotalActorsCount == default ? 0 : 100 * (1 - (decimal)PendingActorsCount / TotalActorsCount);
        public int TotalActorsCount { get; private set; }
        public int StoredActorsCount => actors.Count;
        public int PendingActorsCount => actorsPending.Count;
        public string CurrentTask { get; private set; }
        
        private Subject<Actor> onLoaded = new Subject<Actor>();
        private Subject<object> onStopSync = new Subject<object>();
        private static object syncRoot = new object();
        private readonly ILogger<WebScraper> logger;
        readonly IBrowsingContext browsingContext;
        readonly HashSet<Actor> actors = new HashSet<Actor>();
        readonly HashSet<Actor> actorsPending = new HashSet<Actor>();
        public WebScraper(ILogger<WebScraper> logger, IBrowsingContext browsingContext)
        {
            this.logger = logger;
            this.browsingContext = browsingContext;
        }

        public void Set(List<Actor> existing)
        {
            //set existing actors list to the excluded list
            actors.Clear();

            existing
                .ToList()
                .Select(actors.Add)
                .ToList();

        }

        public async Task<ISet<Actor>> Run() => await Run(Enumerable.Empty<Actor>().ToList());
        public async Task<ISet<Actor>> Run(List<Actor> exclude)
        {
            var eventId = new EventId();
            try {
                var actorsListHtml = await DownloadHtmlDocument();
                var availableActors = ParseActorResultsPage(actorsListHtml);
                
                //set available actors count to calculate sync progress
                TotalActorsCount = availableActors.Count;
                
                //set existing actors list to the excluded list
                Set(exclude);
                
                //set the pending actors to all available actors from imdb except the existing
                actorsPending.Clear();

                availableActors
                    .Except(actors)
                    .Select(actorsPending.Add)
                    .ToList();
                

                SyncStatus = ScraperStatus.Sync;
                foreach (var actor in actorsPending.ToList())
                {
                    CurrentTask = actor.FullName;
                    eventId = new EventId(StoredActorsCount, $"{actor.Id} ({actor.FullName})");
                    //checking if sync was stopped manually
                    if (SyncStatus != ScraperStatus.Sync) break;

                    await UpdateActorGender(actor);
                    await UpdateActorDetails(actor);
                    
                    actorsPending.Remove(actor);
                    actors.Add(actor);
                    onLoaded.OnNext(actor);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(eventId, e, "Sync failed");
            }
            finally
            {
                StopSync();
            }
            

            //identify and update genders of all actors using imdb advanced search url
            //searching only with male filter in gender field,
            //since we know the actor already exists in imdb then if the 
            //male gender filter returns 0 matches, we know it must be female.
          
            return actors;
        }

        async Task UpdateActorDetails(Actor actor)
        {
            var eventId = new EventId(StoredActorsCount, $"{actor.Id} ({actor.FullName})");

            try
            {
                var searchUrl = string.Format(ACTOR_PAGE_URL, actor.PageUrl);
                var dateString = "";

                var doc = await this.browsingContext.OpenAsync(searchUrl);
                dateString = doc
                    .QuerySelector("div#name-born-info.txt-block")
                    .QuerySelector("time")
                    .GetAttribute("datetime");
                
                var born = DateTime.ParseExact(dateString, "yyyy-M-d", new CultureInfo("en-US"));
                actor.Born = born;
                
            }
            catch (System.Exception e)
            {
                this.logger.LogError(eventId, e, "Details sync failed");
                StopSync();
            }
        }

        async Task UpdateActorGender(Actor actor)
        {
            var eventId = new EventId(StoredActorsCount, $"{actor.Id} ({actor.FullName})");

            try
            {
                var searchUrl = string.Format(FEMALE_ACTORS_SEARCH_URL, actor.FullName.Replace(" ", "+"));
                var doc = await this.browsingContext.OpenAsync(searchUrl);
                var isFemale = ParseActorResultsPage(doc).Any(a => a.Id == actor.Id);
                actor.Gender = isFemale ? Gender.Female : Gender.Male;
            }
            catch (System.Exception e)
            {
                this.logger.LogError(eventId, e, "Gender sync failed");
                StopSync();
            }

            //TODO: handle cases where multiple matches returned
        }

        async Task<IDocument> DownloadHtmlDocument()
        {
            var eventId = new EventId(StoredActorsCount, $"Download Html Document");
            try
            {
                var html = await this.browsingContext.OpenAsync(ACTORS_LIST_URL);
                return html;
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        static ISet<Actor> ParseActorResultsPage(IDocument doc) =>
            ParseActorResults(SelectActorResults(doc));

        static IEnumerable<IElement> SelectActorResults(IDocument doc) =>
            doc.QuerySelectorAll(".mode-detail").Where(predicateForListLiActor);

        static Func<IElement, bool> predicateForListLiActor = li 
            => li
                .QuerySelector("img")?
                .GetAttribute("alt")?
                .Trim() != null;

        static ISet<Actor> ParseActorResults(IEnumerable<IElement> elems) =>
            elems.Aggregate(new HashSet<Actor>(), (actorsList, li) =>
            {
                var link = li.QuerySelector(".lister-item-image").QuerySelector("a").GetAttribute("href");

                //extract id from various forms of url: (eg: /name/aaa or /name/aaa/?bbb)
                var id = Regex.Match(link, @"(?<=\/name\/)(.*)(?=\/)|(?<=\/name\/)(.[^\/]*)").Groups[0].Value;
                
                var url = $"/name/{id}";

                var fullName = li
                    .QuerySelector(".lister-item-header")?
                    .QuerySelector("a")?
                    .Text()
                    .Trim();

                var imageUrl = li
                    .QuerySelector("img")?
                    .GetAttribute("src");

                var role = li.QuerySelector(".text-muted.text-small")?.InnerHtml.Replace("\\n", "").Trim().Split(' ').FirstOrDefault();

                //if it is a cloned match or a different role (such as "Art Department") skip match
                if (fullName == null || role == null || imageUrl.Contains("/nopicture/")) return actorsList;

                var actor = new Actor
                {
                    Id = id,
                    PageUrl = url,
                    FullName = fullName,
                    ImageUrl = imageUrl,
                    Role = role
                };

                actorsList.Add(actor);
                
                return actorsList;
            });


        public IDisposable Subscribe(IObserver<Actor> observer)
        {
            return Observable

                //protect the main stream of loaded actors from being closed by the observers
                .Defer(() => onLoaded).Subscribe(observer);
        }

        public bool GetIsSyncing() => SyncStatus == ScraperStatus.Sync;

        public void StopSync()
        {
            SyncStatus = ScraperStatus.Idle;
        }

        public ScraperStatus GetStatus() => SyncStatus;
    }
}