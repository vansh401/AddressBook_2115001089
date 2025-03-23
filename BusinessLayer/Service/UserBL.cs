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
            var role = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;
            var user = _UserRepo.CreateUser(model.UserName, model.Email, hashedPassword,role);

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


        public string GenerateResetToken(int userId, string email)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Invalid userId or email for reset token generation.");
            }
            return _jwtTokenGenerator.GenerateResetToken(userId, email);
        }


        public UserEntity GetUserByEmail(string email)
        {
            return _UserRepo.GetUserByEmail(email);
        }

        public UserEntity ResetPassword(string token, ResetPasswordReq model)
        {
            int userId = _jwtTokenGenerator.ResetPassword(token, model);
            var user = _UserRepo.GetUserById(userId);
            if (user != null)
            {
                string hashedPassword = HashingPassword.HashPassword(model.NewPassword);
                user.PasswordHash = hashedPassword;
                if (_UserRepo.UpdateUserPassword(user.Email, user.PasswordHash))
                {
                    return user;
                }
            }
            return null;
        }
        public UserEntity GetUserById(int userId)
        {
            return _UserRepo.GetUserById(userId);
        }


    }
}
