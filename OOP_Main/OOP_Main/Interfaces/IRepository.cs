namespace OOP_Main.Interfaces {
    internal interface IRepository<T> {
        T LoadFromFile(string filePath);

        void SaveToFile(string filePath, T data);
    }
}
