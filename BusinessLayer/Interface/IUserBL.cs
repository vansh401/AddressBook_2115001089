using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        ResponseModel<string> RegisterUser(RegisterReq model);
        ResponseModel<string> LoginUser(LoginReq model);
        string GenerateResetToken(int userId, string email);
         UserEntity GetUserByEmail(string email);
        UserEntity ResetPassword(string token, ResetPasswordReq model);
        UserEntity GetUserById(int userId);
    }
}
