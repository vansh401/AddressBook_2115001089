using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        bool UserExists(string email);
        void CreateUser(string username,string email, string passwordHash);
        UserEntity GetUserByEmail(string email);
        void UpdateUserPassword(string email, string newPasswordHash);

    }
}
