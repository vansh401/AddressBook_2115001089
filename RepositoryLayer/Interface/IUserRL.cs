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
        UserEntity CreateUser(string username,string email, string passwordHash);
        UserEntity GetUserByEmail(string email);
        bool UpdateUserPassword(string email, string newPasswordHash);

    }
}
