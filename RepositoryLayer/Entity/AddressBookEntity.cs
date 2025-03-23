using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class AddressBookEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }
        [Required]
        public string Number { get; set; }

        [EmailAddress, MaxLength(255)]
        public string Email { get; set; }
        public string Address { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        //Navigation Property
        [ForeignKey("UserId")]
        public UserEntity Users { get; set; }
    }
}
