using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IAddressBookService
    {
        List<AddressBookEntity> GetAllContact(int userId);
        AddressBookEntity GetContactById(int userId, int id);
        AddressBookEntity AddContact(int userId, string name, string number, string email, string address);
        bool UpdateContact(int userId, int id, string newName, string newNumber, string email, string address);
        bool DeleteContact(int userId, int id);
        List<AddressBookEntity> GetAllContactsForAdmin();
        bool DeleteContactByAdmin(int contactId);
    }
}
