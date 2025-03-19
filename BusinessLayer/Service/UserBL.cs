﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL:IUserBL
    {
        private readonly IUserRL _UserRepo;
        public UserBL(IUserRL UserRepo)
        {
            _UserRepo = UserRepo;
        }
        public ResponseModel<UserEntity> RegisterUser(RegisterReq model)
        {
            if (_UserRepo.UserExists(model.Email))
                return new ResponseModel<UserEntity>
                {
                    Success = false,
                    Message = "Email already exists"
                };

            string hashedPassword = model.Password;
            var user = _UserRepo.CreateUser(model.UserName, model.Email, hashedPassword);

            var response = new ResponseModel<UserEntity>
            {
                Success = true,
                Message = "User registered successfully",
                Data = user
            };
            return response;
        }

        public ResponseModel<string> LoginUser(LoginReq model)
        {
            var user=_UserRepo.GetUserByEmail(model.Email);
            if(user==null || model.Password != user.PasswordHash)
            {
                return new ResponseModel<string> { Success = false, Message = "Invalid Credentials" };
            }
            return new ResponseModel<string> { Success = true, Message = "Login Succesfull"};
        }

        public ResponseModel<string> ForgotPassword(ForgotPasswordReq model)
        {
            // logic of reset password link 
            return new ResponseModel<string> { Success = true, Message = "Password Reset Link Sent" };
        }
        

        public ResponseModel<string> ResetPassword(ResetPasswordReq model)
        {
            string hashedPassword = model.NewPassword;
            _UserRepo.UpdateUserPassword(model.Email,hashedPassword);

            return new ResponseModel<string> { Success = true, Message = "Password Reset Successfully" };
        }


    }
}
