using System;
using System.Collections.Generic;
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
        ResponseModel<string> ForgotPassword(ForgotPasswordReq model);
        ResponseModel<string> ResetPassword(ResetPasswordReq model);
    }
}
