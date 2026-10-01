using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    internal class UserData: IBridgeJSON {
        private const string UsersFilePath = "DataStorage/users.json";
        public List<User> Users { get; private set; } = new List<User>();
        public void Load() {
            Users = JsonStorage.LoadFromFile<List<User>>(UsersFilePath);
        }

        public void Save() {
            JsonStorage.SaveToFile(UsersFilePath, Users);
        }

        public void AddUser(User user) {
            Users.Add(user);
            JsonStorage.SaveToFile(UsersFilePath, Users);
        }

        public void AddUser(string name, string password, bool isAdmin = false) {
            int newUserId = Users.Count > 0 ? Users.Max(u => u.Id) : 0;
            newUserId++;
            User newUser = new User(newUserId, name, password, isAdmin);

            AddUser(newUser);
        }

        public bool DeleteUserById(int id) {
            User userToDelete = this.GetUserById(id);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
                JsonStorage.SaveToFile(UsersFilePath, Users);
                return true;
            }
            return false;
        }

        public bool DeleteUserByUsername(string username) {
            User userToDelete = this.GetUserByUsername(username);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
                JsonStorage.SaveToFile(UsersFilePath, Users);
                return true;
            }
            return false;
        }

        public User GetUserById(int id) {
            User foundUser = Users.Find((user) => user.Id == id);
            return foundUser;
        }

        public User GetUserByUsername(string username) {
            User foundUser = Users.Find((user) => user.Username == username);
            return foundUser;
        }
    }
}
