using ImdbListingExercise.Services.Contracts;

namespace ImdbListingExercise.Models.Protocol
{
    public class StatusResponse
    {
        public ScraperStatus Status { get; set; }
        public decimal Progress { get; set; }
        public string CurrentTask { get; set; }
        public int ActorsCount { get; set; }
    }
}