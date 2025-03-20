using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;
using RepositoryLayer.PasswordHashing;

namespace BusinessLayer.Service
{
    public class UserBL:IUserBL
    {
        private readonly IUserRL _UserRepo;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        public UserBL(IUserRL UserRepo,JwtTokenGenerator jwtTokenGenerator)
        {
            _UserRepo = UserRepo;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public ResponseModel<string> RegisterUser(RegisterReq model)
        {
            if (_UserRepo.UserExists(model.Email))
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Email already exists"
                };

            string hashedPassword = HashingPassword.HashPassword(model.Password);
            var user = _UserRepo.CreateUser(model.UserName, model.Email, hashedPassword);

            var response = new ResponseModel<string>
            {
                Success = true,
                Message = "User registered successfully",
                Data = $"User Name: {user.UserName}     Email: {user.Email}"
            };
            return response;
        }

        public ResponseModel<string> LoginUser(LoginReq model)
        {
            var user = _UserRepo.GetUserByEmail(model.Email);
            if (user == null || !HashingPassword.VerifyPassword(model.Password, user.PasswordHash))
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            string token = _jwtTokenGenerator.GenerateToken(user);
            return new ResponseModel<string>
            {
                Success = true,
                Message = "Login successful",
                Data = token
            };
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
