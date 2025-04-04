﻿using System.Security.Claims;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Middleware.RabbitMQ.Interface;
using Middleware.RabbitMQ.Service;
using ModelLayer.Model;

namespace AddressBookAPI.Controllers
{
    [ApiController]
        [Route("api/auth")]
        public class UserController:ControllerBase
        {
            private readonly IUserBL _userService;
            private readonly ILogger<UserController> _logger;
            private readonly IRabbitMQProducer _rabbitMQProducer;
            public UserController(ILogger<UserController> logger,IUserBL userService,IRabbitMQProducer rabbitMQProducer)
            {
                _logger = logger;
                _userService = userService;
                _rabbitMQProducer = rabbitMQProducer;
            }

            [HttpPost]
            [Route("register")]
            public IActionResult Register([FromBody] RegisterReq model)
            {
                try
                {
                    var response = _userService.RegisterUser(model);
                    if (!response.Success)
                    {
                        _logger.LogWarning("User registration failed: Email already exists.");
                        return BadRequest(response);
                    }
                    _logger.LogInformation("User Registered successfully");
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while registering user.");
                    return StatusCode(500, new { Success = false, Message = "Internal Server Error", Error = ex.Message });
                }

            }

            [HttpPost]
            [Route("login")]

            public IActionResult Login([FromBody] LoginReq model)
            {
                try
                {
                    _logger.LogInformation("Login attemp for user: {0}", model.Email);
                    var response = _userService.LoginUser(model);
                    if (!response.Success)
                    {
                        _logger.LogWarning("Invalid login attempt for user: {0}", model.Email);
                        return Unauthorized(response);
                    }
                    _logger.LogInformation("User {0} logged in successfully.", model.Email);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login failed.");
                    return BadRequest(new { Success = false, Message = "Login failed.", Error = ex.Message });
                }
            }

        

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordReq model)
        {
            _logger.LogInformation("ForgotPassword request received with email: {Email}", model.Email);
            var response = new ResponseModel<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return BadRequest(new { success = false, message = "Email is required" });
                }
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null)
                {
                    string token = _userService.GenerateResetToken(user.Id, user.Email);
                    _rabbitMQProducer.PublishMessage(user.Email, token);
                    response.Success = true;
                    response.Message = "Reset password link sent to email";
                    return Ok(response);
                }
                response.Success = false;
                response.Message = "User not found";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process Failed");
                return BadRequest(new { Success = false, Message = "Process Failed", Error = ex.Message });
            }
        }


            [HttpPost]
            [Route("resetpassword")]
            public IActionResult ResetPassword([FromQuery] string token, [FromBody] ResetPasswordReq model)
            {
                _logger.LogInformation("Resetting Password...");
                var response = new ResponseModel<string>();
                try
                {
                    var user = _userService.ResetPassword(token, model);
                    if (user != null)
                    {
                        response.Success = true;
                        response.Message = "Password reset successful";
                        return Ok(response);
                    }
                    response.Success = false;
                    response.Message = "Invalid or expired token";
                    return BadRequest(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Process Failed");
                    return BadRequest(new { Success = false, Message = "Process Failed", Error = ex.Message });
                }
            }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            try
            {
                _logger.LogInformation("Fetching User Profile...");
                var userId = User.FindFirstValue("userId");
                var response = new ResponseModel<string>();
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Invalid user token.";
                    return Unauthorized(response);
                }

                var user = _userService.GetUserById(int.Parse(userId));
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return NotFound(response);
                }
                response.Success = true;
                response.Message = "User found";
                response.Data = $"id: {user.Id}, UserName: {user.UserName}, Email: {user.Email}, Role: {user.Role}";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

    }
    }
