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
        public AddressBookEntity AddContact(int userId, string name, string number)
        {
            return _addressBookRL.AddContact(userId, name, number);
        }
        public bool UpdateContact(int userId, int id, string newName, string newNumber)
        {
            return _addressBookRL.UpdateContact(userId, id, newName, newNumber);
        }
        public bool DeleteContact(int userId, int id)
        {
            return _addressBookRL.DeleteContact(userId, id);
        }
    }
}
