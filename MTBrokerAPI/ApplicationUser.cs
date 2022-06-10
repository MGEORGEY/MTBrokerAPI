using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MTBrokerAPI
{
    public class ApplicationUser : IdentityUser<int>
    {

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime DateJoined { get; set; }


        [DataType(DataType.EmailAddress)]
        [Required]
        public override string Email { get => base.Email; set => base.Email = value; }


        [Required]
        public string Name { get; set; }


        [Required]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

    }
}
