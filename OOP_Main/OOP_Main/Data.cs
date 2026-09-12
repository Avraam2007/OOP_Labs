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
            User userToDelete = Users.Find((user) => user.Id == id);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
            }
        }

        public void DeleteUserByUsername(string username) {
            User userToDelete = Users.Find((user) => user.Username == username);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
            }
        }

        public Data() {

        }
    }
}
