using System;

namespace OOP_Main {
    public class User {
        private readonly string _id;
        private string username;
        private readonly string password;
        private readonly bool isAdmin;

        public string Username { 
            get { return username; } 
            set {
                if (value.Trim() == "") {
                    throw new ArgumentException("Error: username is empty!");
                }
                username = value;
            } 
        }
        public string Id { get { return _id; } }
        public bool IsAdmin { get { return isAdmin; } }
        public string Password { 
            get { return password; }
            private set {
                if (value.Trim() == "") {
                    throw new ArgumentException("Error: password is empty!");
                }
                username = value;
            }
        }

        public User(string id, string username, string password, bool isAdmin = false) {
            this.username = username;
            this.password = password;
            this._id = id;
            this.isAdmin = isAdmin;
        }
    }
}
