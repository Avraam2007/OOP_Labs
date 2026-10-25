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
        protected BaseDataRepository(string filePath) {
            FilePath = filePath;
        }

        public virtual void Load() {
            Items = JsonStorage.LoadFromFile<List<T>>(FilePath) ?? new List<T>();
        }

        public void Save() {
            JsonStorage.SaveToFile(FilePath, Items);
        }

        public virtual async Task LoadAsync() {
            Task<List<T>> loadTask = Task.Run(() => JsonStorage.LoadFromFile<List<T>>(FilePath));
            Items = await loadTask ?? new List<T>();
        }

        public async Task SaveAsync() {
            Task saveTask = Task.Run(() => JsonStorage.SaveToFile(FilePath, Items));
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
