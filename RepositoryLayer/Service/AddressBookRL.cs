using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Service
{
    public class AddressBookRL
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

        public AddressBookEntity AddContact(int userId, string name, string number)
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
                UserId=userId
            };

            _context.AddressBookContacts.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public bool UpdateContact(int userId,int id,string newName,string newNumber)
        {
            var contact=_context.AddressBookContacts.FirstOrDefault(x=> x.Id== id && x.UserId==userId);
            if (contact == null)
            {
                return false;
            }
            contact.Name=newName;
            contact.Number=newNumber;
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

    }
}
