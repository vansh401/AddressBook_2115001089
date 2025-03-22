using System.Security.Claims;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Service;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("api/addressbook")]
    public class AddressBookAPIController : ControllerBase
    {
        private readonly ILogger<AddressBookAPIController> _logger;
        private readonly IAddressBookService _addressBookService;
        private readonly IRedisCacheService _redisCacheService;
        public AddressBookAPIController(ILogger<AddressBookAPIController> logger, IAddressBookService addressBookService, IRedisCacheService redisCacheService)
        {
            _logger = logger;
            _addressBookService = addressBookService;
            _redisCacheService = redisCacheService;
        }

        private int GetUserIdFromToken()
        {
            var userId = User.FindFirstValue("userId");
            if(userId == null)
            {
                _logger.LogWarning("Invalid Or missing UserId token");
                throw new UnauthorizedAccessException(userId);
            }
            return int.Parse(userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation("Checking cache for contacts");

                // Check Redis Cache
                var cacheKey = $"contacts_user_{userId}";
                var cachedcontacts = await _redisCacheService.GetCachedData<List<AddressBookEntity>>(cacheKey);

                if (cachedcontacts != null)
                {
                    _logger.LogInformation("Returning greetings from cache.");
                    return Ok(new ResponseModel<List<AddressBookEntity>>
                    {
                        Success = true,
                        Message = $"Cache hit for user {userId} contacts",
                        Data = cachedcontacts
                    });
                }
                _logger.LogInformation("Fetching all contacts from Database...");
                var contacts = _addressBookService.GetAllContact(userId);
                if (contacts == null || contacts.Count == 0)
                {
                    _logger.LogWarning("No Contacts found.");
                    return NotFound(new ResponseModel<List<AddressBookEntity>>
                    {
                        Success = false,
                        Message = "No contacts found."
                    });
                }
                _logger.LogInformation($"Caching data for key: contacts_user_{userId}");
                await _redisCacheService.SetCachedData($"contacts_user_{userId}", contacts, TimeSpan.FromMinutes(20));


                var response = new ResponseModel<List<AddressBookEntity>>
                {
                    Success = true,
                    Message = "Contacts Fetched successfully!",
                    Data = contacts
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllContacts: {ex.Message}");
                return StatusCode(500, new ResponseModel<List<AddressBookEntity>>
                {
                    Success = false,
                    Message = "Internal Server Error"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation($"Fetching Contact for UserID: {userId}, ContactID: {id}");
                var cacheKey = $"contact_{id}";
                var cachedContact = await _redisCacheService.GetCachedData<AddressBookEntity>(cacheKey);
                if (cachedContact != null)
                {
                    if (cachedContact.UserId != userId)
                    {
                        _logger.LogWarning($"Unauthorized access attempt by UserId {userId} for Contact ID {id}");
                        return Unauthorized(new ResponseModel<string> { Success = false, Message = "Unauthorized access" });
                    }
                    _logger.LogInformation($"Cache hit for contact ID: {id}");
                    return Ok(new ResponseModel<AddressBookEntity> { Success = true, Message = "Contact found (from cache)", Data = cachedContact });
                }
                _logger.LogInformation($"Fetching greeting from DB for UserID: {userId}, GreetingID: {id}");
                var contact = _addressBookService.GetContactById(userId, id);
                if (contact == null)
                    return NotFound(new ResponseModel<AddressBookEntity> { Success = false, Message = "Contact not found" });
                await _redisCacheService.SetCachedData(cacheKey, contact, TimeSpan.FromMinutes(20));
                return Ok(new ResponseModel<AddressBookEntity> { Success = true, Message = "Contact found", Data = contact });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetContactById method");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost]
        public IActionResult AddContact(string contactName, string contactNumber)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _addressBookService.AddContact(userId, contactName, contactNumber);
                _logger.LogInformation("Saving the Contact...");

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Contact saved successfully",
                    Data = "Contact Saved!"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Unauthorized access: {ex.Message}");
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Unauthorized access." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddContact: {ex.Message}");
                return StatusCode(500, new ResponseModel<string> { Success = false, Message = "Internal Server Error" });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContactById(int id, string name, string number)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation($"Attempting to update contact with ID: {id}");

                bool isUpdated = _addressBookService.UpdateContact(userId, id, name, number);

                if (!isUpdated)
                {
                    _logger.LogWarning($"Contact with ID {id} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = $"Contact with ID {id} not found."
                    });
                }
                // Removing Cache since data changed
                await _redisCacheService.RemoveCachedData("all_contacts");
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = $"Contact with ID {id} updated successfully.",
                    Data = $"New Contact Name: {name} and New Contact Number: {number}"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateContactById: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal Server Error"
                });
            }
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteContactById(int id)
        {
            try
            {
                int userId = GetUserIdFromToken();
                _logger.LogInformation($"Attempting to delete contact with ID: {id}");

                bool isDeleted = _addressBookService.DeleteContact(userId, id);

                if (!isDeleted)
                {
                    _logger.LogWarning($"Contact with ID {id} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Contact not found."
                    });
                }
                // Removing Cache
                await _redisCacheService.RemoveCachedData("all_contacts");
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Contact deleted successfully.",
                    Data = $"Deleted Contact ID: {id}"
                };
                _logger.LogInformation($"Contact with ID {id} deleted successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteContactById: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
