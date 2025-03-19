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
        public UserController(IUserBL userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] RegisterReq model)
        {
            var response = _userService.RegisterUser(model);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);

        }

        [HttpPost]
        [Route("login")]

        public IActionResult Login([FromBody] LoginReq model)
        {
            var response=_userService.LoginUser(model);
            if (!response.Success)
            {
                return Unauthorized(response);
            }
            return Ok(response);
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
