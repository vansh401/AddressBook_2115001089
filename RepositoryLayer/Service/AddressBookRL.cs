using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRL:IAddressBookRL
    {
        private readonly AppDbContext _context;
        public AddressBookRL(AppDbContext context)
        {
            _context = context;
        }

        public List<AddressBookEntity> GetAllContact(int userId)
        {
            return _context.AddressBookContacts.Where(x=> x.UserId == userId).ToList();
        }

        public AddressBookEntity GetContactById(int userId,int id)
        {
            return _context.AddressBookContacts.FirstOrDefault(x=> x.UserId==userId && x.Id == id);
        }

        public AddressBookEntity AddContact(int userId, string name, string number, string email, string address)
        {
            var user=_context.Users.FirstOrDefault(x=>x.Id== userId);
            if (user == null)
            {
                throw new InvalidOperationException("User Not Found");
            }

            var contact = new AddressBookEntity
            {
                Name=name,
                Number=number,
                Email=email,
                Address=address,
                UserId=userId
            };

            _context.AddressBookContacts.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public bool UpdateContact(int userId,int id,string newName,string newNumber, string email, string address)
        {
            var contact=_context.AddressBookContacts.FirstOrDefault(x=> x.Id== id && x.UserId==userId);
            if (contact == null)
            {
                return false;
            }
            contact.Name=newName;
            contact.Number=newNumber;
            contact.Email=email;
            contact.Address=address;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteContact(int userId,int id)
        {
            var contact=_context.AddressBookContacts.FirstOrDefault(x=>x.Id==id &&  x.UserId==userId);
            if(contact == null)
            {
                return false;
            }
            _context.AddressBookContacts.Remove(contact);
            _context.SaveChanges();
            return true;
        }
        public List<AddressBookEntity> GetAllContactsForAdmin()
        {
            return _context.AddressBookContacts.ToList();
        }

        public bool DeleteContactByAdmin(int contactId)
        {
            var contact = _context.AddressBookContacts.FirstOrDefault(g => g.Id == contactId);
            if (contact == null)
                return false;

            _context.AddressBookContacts.Remove(contact);
            _context.SaveChanges();
            return true;
        }

    }
}
