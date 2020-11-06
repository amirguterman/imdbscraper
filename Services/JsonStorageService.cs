using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using ImdbListingExercise.Models.Imdb;
using ImdbListingExercise.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace ImdbListingExercise.Services
{
    public class JsonStorageService : IStorageService<Actor>
    {
        private string DB_FILE_NAME => "db.json";
        private string DB_REMOTE_URL => "https://www.imdb.com/list/ls052283250/";

        private Subject<FileOperation> dbAccessStream = new Subject<FileOperation>();
        private HashSet<Actor> db = new HashSet<Actor>();
        private bool disposedValue;
        private IDisposable managedFileAccessSub;
        private readonly ILogger<JsonStorageService> logger;

        private enum FileOperation
        {
            Load = 0,
            Save = 1
        }

        public JsonStorageService(ILogger<JsonStorageService> logger)
        {
            //Reduce I/O operations to disk by sampling requests of each type every 10 ms
            this.managedFileAccessSub = 
            
            dbAccessStream
                .Where(o => o == FileOperation.Load)
                .Sample(TimeSpan.FromMilliseconds(10))
                .Do(o => LoadFromFile())
                .Merge(
            
            dbAccessStream
                .Where(o => o == FileOperation.Save)
                .Sample(TimeSpan.FromMilliseconds(10))
                .Do(o => SaveToFile()))
            
            //load actors from file on initialization
            

            .Subscribe();

            LoadFromFile();
            this.logger = logger;
        }

        private void LoadFromFile()
        {
            var fileName = DB_FILE_NAME;

            //if db was not yet created, return empty list of actors (sync operation not requestred yet)
            if (!File.Exists(fileName)) return;

            try
            {
                // read file into a string and deserialize JSON to a type
                var actors = JsonSerializer.Deserialize<IEnumerable<Actor>>(File.ReadAllText(fileName));

                //store all the saved actors in the hashset
                actors.Select(db.Add).ToList();
            }
            catch (IOException ioe)
            {
                logger.LogError("The file could not be accessed", ioe);
                throw ioe;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
        }

        public void Add(IEnumerable<Actor> actors)
        {
            //add actors to in-memory hashset except the actors that already exist
            var success = actors.Except(db)
                //add to actor to hashset
                .Select(actor => db.Add(actor))
                .ToList()
                .All(added => true);
                
            if (!success)
                throw new Exception("There was a problem loading one of the actors");

            //signal to flush actors to file
            dbAccessStream.OnNext(FileOperation.Save);
        }

        private void SaveToFile()
        {
            var fileName = DB_FILE_NAME;

            try
            {
                //get actors from in-memory dictionary
                var actors = List();

                // serialize JSON to a string and then write string to a file
                var json = JsonSerializer.Serialize<IEnumerable<Actor>>(actors);
                File.WriteAllText(fileName, json);
            }
            catch (IOException ioe)
            {
                logger.LogError("The file could not be accessed", ioe);
                throw ioe;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
        }

        public void Delete(string id)
        {
            var actor = db.FirstOrDefault(a => a.Id == id);

            //remove actor from in-memory dictionary
            if (actor == null || !db.Remove(actor))
                throw new Exception($"Could not find an actor with id {id}");

            //signal to flush actors to file
            dbAccessStream.OnNext(FileOperation.Save);
        }

        public void Reset()
        {
            //remove actor from in-memory dictionary
            db.Clear();
            
            //signal to flush actors to file
            dbAccessStream.OnNext(FileOperation.Save);
        }

        //public void Add(IEnumerable<Actor> items) => Add(items);
        
        public IEnumerable<Actor> List() => db;
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.managedFileAccessSub.Dispose();
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}