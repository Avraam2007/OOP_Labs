using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NOptional;
using OOP_Main.FileIO;
using OOP_Main.Interfaces;

namespace OOP_Main.Repositories {
    public interface IIdentifiable<TId> {
        TId Id { get; }
    }
    public abstract class BaseDataRepository<T, TId> : IBridgeJSON 
        where T: class, IIdentifiable<TId>
        where TId: IEquatable<TId> 
        {
        public readonly string FilePath;
        public List<T> Items { get; protected set; } = new List<T>();

        private readonly IStorage<List<T>> _storage;

        protected BaseDataRepository(string filePath, IStorage<List<T>> storage = null) {
            FilePath = filePath;
            _storage = storage ?? new JsonStorage<List<T>>();
        }

        public virtual void Load() {
            Items = _storage.LoadFromFile(FilePath) ?? new List<T>();
        }

        public void Save() {
            _storage.SaveToFile(FilePath, Items);
        }

        public virtual async Task LoadAsync() {
            Task<List<T>> loadTask = Task.Run(() => _storage.LoadFromFile(FilePath));
            Items = await loadTask ?? new List<T>();
        }

        public async Task SaveAsync() {
            Task saveTask = Task.Run(() => _storage.SaveToFile(FilePath, Items));
            await saveTask;
        }

        protected virtual void Add(T obj) {
            Items.Add(obj);
            this.Save();
        }

        protected virtual bool Remove(TId id) {
            IOptional<T> itemToDelete = this.Get(id);
            if (itemToDelete.HasValue()) {
                Items.Remove(itemToDelete.GetValueOrElseThrow());
                return true;
            }
            return false;
        }

        protected abstract IOptional<T> Get(TId id);
    }
}
