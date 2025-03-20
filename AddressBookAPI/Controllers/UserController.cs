using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController:ControllerBase
    {
        private readonly IUserBL _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger,IUserBL userService)
        {
            _logger = logger;
            _userService = userService;
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
        public IActionResult ForgotPassword([FromBody]ForgotPasswordReq model)
        {
            var response= _userService.ForgotPassword(model);
            return Ok(response);
        }

        [HttpPost]
        [Route("resetpassword")]
        public IActionResult ResetPassword([FromBody]ResetPasswordReq model)
        {
            var response=_userService.ResetPassword(model);
            return Ok(response);
        }
    }
}
