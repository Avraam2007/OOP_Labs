using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NOptional;
namespace OOP_Main 
    {
    public class UserData: BaseDataRepository<User, int> {
        public List<User> Users => Items;

        public UserData() : base("DataStorage/users.json") { }

        public void AddUser(User user) => Add(user);

        public void AddUser(string name, string password, bool isAdmin = false) {
            int newUserId = Users.Count > 1 ? Users.Max(u => u.Id) : 0;
            newUserId++;
            User newUser = new User(newUserId, name, password, 0, isAdmin);

            AddUser(newUser);
        }

        public bool DeleteUserById(int id) => Remove(id);

        public bool DeleteUserByUsername(string username) {
            IOptional<User> userToDelete = this.GetUserByUsername(username);
            if (userToDelete.HasValue()) {
                Users.Remove(userToDelete.GetValueOrElseThrow());
                Save();
                return true;
            }
            return false;
        }

        public override IOptional<User> Get(int id) {
            User foundUser = Users.Find((user) => user.Id == id);
            return Optional.OfNullable(foundUser);
        }

        public IOptional<User> GetUserById(int id) => this.Get(id);

        public IOptional<User> GetUserByUsername(string username) {
            User foundUser = Users.Find((user) => user.Username == username);
            return Optional.OfNullable(foundUser);
        }
    }
}
