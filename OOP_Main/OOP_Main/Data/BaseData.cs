using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NOptional;

namespace OOP_Main {
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
