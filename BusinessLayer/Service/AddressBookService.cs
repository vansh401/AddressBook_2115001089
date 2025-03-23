using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace BusinessLayer.Service
{
    public class AddressBookService:IAddressBookService
    {
        private readonly IAddressBookRL _addressBookRL;
        public AddressBookService(IAddressBookRL addressBookRL)
        {
            _addressBookRL = addressBookRL;
        }
        public List<AddressBookEntity> GetAllContact(int userId)
        {
            return _addressBookRL.GetAllContact(userId);
        }
        public AddressBookEntity GetContactById(int userId, int id)
        {
            return _addressBookRL.GetContactById(userId, id);
        }
        public AddressBookEntity AddContact(int userId, string name, string number, string email, string address)
        {
            return _addressBookRL.AddContact(userId, name, number,email,address);
        }
        public bool UpdateContact(int userId, int id, string newName, string newNumber, string email, string address)
        {
            return _addressBookRL.UpdateContact(userId, id, newName, newNumber, email, address);
        }
        public bool DeleteContact(int userId, int id)
        {
            return _addressBookRL.DeleteContact(userId, id);
        }

        public List<AddressBookEntity> GetAllContactsForAdmin()
        {
            return _addressBookRL.GetAllContactsForAdmin();
        }
        public bool DeleteContactByAdmin(int contactId)
        {
            return _addressBookRL.DeleteContactByAdmin(contactId);
        }
    }
}
