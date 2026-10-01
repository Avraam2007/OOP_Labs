using System;

namespace OOP_Main {
    public class User {
        private readonly int _id;
        private string username;
        private string password;
        private readonly bool isAdmin;

        public string Username {
            get { return username; }
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("Error: username is empty!");
                }
                username = value;
            }
        }
        public int Id { get { return _id; } }
        public bool IsAdmin { get { return isAdmin; } }
        public string Password {
            get { return password; }
            private set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("Error: password is empty!");
                }
                password = value;
            }
        }

        public User(int id, string username, string password, bool isAdmin = false) {
            this.Username = username;
            this.Password = password;
            this._id = id;
            this.isAdmin = isAdmin;
        }

        public override bool Equals(object obj) {
            if (obj is User other) return this.Id == other.Id;
            return false;
        }
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => $"[User] {Username} (Admin: {IsAdmin})";
    }
}
