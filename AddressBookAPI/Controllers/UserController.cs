    using BusinessLayer.Interface;
    using Microsoft.AspNetCore.Mvc;
    using ModelLayer.Model;

    namespace AddressBookAPI.Controllers
    {
        [ApiController]
        [Route("api/auth")]
        public class UserController:ControllerBase
        {
            private readonly IUserBL _userService;
            private readonly ILogger<UserController> _logger;
            private readonly IEmailService _emailService;
            public UserController(ILogger<UserController> logger,IUserBL userService, IEmailService emailService)
            {
                _logger = logger;
                _userService = userService;
                _emailService = emailService;
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
            _logger.LogInformation("Forgot Password request received with email: {Email}", model.Email);
            var response = new ResponseModel<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return BadRequest(new { success = false, message = "Email is required" });
                }

                var user = _userService.GetUserByEmail(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("User not found with email: {Email}", model.Email);
                    return BadRequest(new { success = false, message = "User not found" });
                }

                if (string.IsNullOrWhiteSpace(user.Email) || user.Id == 0)
                {
                    _logger.LogError("Invalid user data. UserId or Email is missing.");
                    return BadRequest(new { success = false, message = "Invalid user data" });
                }

                string token = _userService.GenerateResetToken(user.Id, user.Email);
                _emailService.SendResetEmail(user.Email, token);

                response.Success = true;
                response.Message = "Reset password link sent to email";
                response.Data = token;
                return Ok(response);
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
        }
    }
