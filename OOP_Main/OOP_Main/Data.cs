using System.Collections.Generic;

namespace OOP_Main {
    public class Data {
        private List<User> users;
        private User currentUser;

        public List<User> Users { get { return users; } set { users = value; } }
        public User CurrentUser { get { return currentUser; } set { currentUser = value; } }

        public void AddUser(User user) {
            Users.Add(user);
        }
        public void DeleteUserById(string id) {
            User userToDelete = this.GetUserById(id);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
            }
        }

        public void DeleteUserByUsername(string username) {
            User userToDelete = this.GetUserByUsername(username);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
            }
        }

        public User GetUserById(string id) {
            User foundUser = Users.Find((user) => user.Id == id);
            return foundUser;
        }

        public User GetUserByUsername(string username) {
            User foundUser = Users.Find((user) => user.Username == username);
            return foundUser;
        }

        public Data() {
            users = new List<User>();

        }
    }
}
