using Microsoft.AspNetCore.Mvc;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookAPIController : ControllerBase
    {
        private readonly ILogger<AddressBookAPIController> _logger;
        public AddressBookAPIController(ILogger<AddressBookAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Address Book App Project";
        }
    }
}
