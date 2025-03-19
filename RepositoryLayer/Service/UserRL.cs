using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL:IUserRL
    {
        private readonly AppDbContext _context;
        public UserRL(AppDbContext context)
        {
            _context = context;
        }
        // checking for user exists with same email?
        public bool UserExists(string email) => _context.Users.Any(u => u.Email == email);

        // creating user with these fields..
        public UserEntity CreateUser(string username, string email, string passwordHash)
        {
            var user = new UserEntity { UserName = username, Email = email, PasswordHash = passwordHash };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        // extracting user by its email
        public UserEntity GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public bool UpdateUserPassword(string email, string newPasswordHash)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.PasswordHash = newPasswordHash;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
