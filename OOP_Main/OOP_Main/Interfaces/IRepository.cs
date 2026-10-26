using System.Collections.Generic;

namespace OOP_Main.Interfaces {
    public interface IStorage<T> {
        T LoadFromFile(string filePath);

        void SaveToFile(string filePath, T data);
    }

    public interface IEntityStorage<U>: IStorage<List<U>> { }
}
