using System;
using System.Collections.Generic;

namespace ImdbListingExercise.Services.Contracts
{
    public interface IStorageService<T> : IDisposable where T : class, new()
    {
        void Add(IEnumerable<T> items);
        void Delete(string id);
        void Reset();
        IEnumerable<T> List();
    }
}