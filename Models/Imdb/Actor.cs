using System;
using System.Collections.Generic;
using ImdbListingExercise.Models.Extensions;
using ImdbListingExercise.Models.Imdb.Enumerations;

namespace ImdbListingExercise.Models.Imdb
{
    public class Actor
    {
        public string Id { get; set; }
        public string PageUrl { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public DateTime? Born { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }

        public override bool Equals(object obj)
        {
            var second = obj as Actor;
            return second != null && this.Id == second.Id;
        }

        public override int GetHashCode() => this.Id?.GetStableHashCode() ?? default;
    }
}