using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImdbListingExercise.Models.Imdb;

namespace ImdbListingExercise.Services.Contracts
{
    public interface IScraper<T> : IObservable<T> where T : class, new()
    {
        Task<ISet<Actor>> Run();
        ScraperStatus GetStatus();
        void StopSync();
        bool GetIsSyncing();
    }
    public enum ScraperStatus
    {
        Idle = 0,
        Sync = 1
    }
}