using System.Collections.Generic;
using System.Threading.Tasks;
using ImdbListingExercise.Models.Imdb;

namespace ImdbListingExercise.Services.Contracts
{
    public interface IImdbService
    {
        Task StartSync();
        void StopSync();
        Task Reset();
        List<Actor> ListActors();
        void Delete(string id);
    }
}