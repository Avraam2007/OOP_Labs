using Newtonsoft.Json;
using OOP_Main.Entities;
using OOP_Main.States;
using System;

namespace OOP_Main {
    public class User {
        private readonly int _id;
        private string username;
        private string password;
        public readonly bool isAdmin;
        private Account _account;

        private readonly UserState _roleState;

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
        public string Role { get { return _roleState.GetRoleName(); } }
        public string Password {
            get { return password; }
            private set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("Error: password is empty!");
                }
                password = value;
            }
        }

        public Account Account {
            get { return _account; }
            set {
                _account = value;
            }
        }
        public User() { }

        [JsonConstructor]
        public User(int id, string username, string password, int startSum = 0, bool isAdmin = false) {
            this.Username = username;
            this.Password = password;
            this._id = id;
            this.isAdmin = isAdmin;
            if (id == 0) {
                _roleState = new GuestState();
            }
            else if (isAdmin) {
                _roleState = new AdminState();
            }
            else {
                _roleState = new LoggedInState();
            }
            this._account = new Account(startSum);
        }

        public override bool Equals(object obj) {
            if (obj is User other) return this.Id == other.Id;
            return false;
        }
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => $"|User #{Id}| {Username} ({this.Role})";

        public void TakeMoney(double amount) => _account.Take(amount);

        public void TopUpAccount(double amount) => _account.Add(amount);

        public bool CanBuy() => _roleState.CanBuyProducts();

        public bool IsGuest() => this.Role == "Guest";

        public bool IsAdmin() => this.Role == "Administrator";

        public void RegisterAccountHandler(AccountHandler del) => _account.RegisterHandler(del);
    }
}
