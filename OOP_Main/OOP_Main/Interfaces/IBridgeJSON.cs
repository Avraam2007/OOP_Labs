using System.Threading.Tasks;

namespace OOP_Main.Interfaces {
    public interface IBridgeJSON {
        void Load();
        void Save();

        Task LoadAsync();

        Task SaveAsync();
    }
}
