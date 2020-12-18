using MVC_Store.Models.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Store.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM()
        {}

        public UserProfileVM(UserDTO user)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.EmailAdress = user.EmailAdress;
            this.Username = user.Username;
            this.Password = user.Password;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 3)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 3)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string EmailAdress { get; set; }
        [Required]
        [DisplayName("User Name")]
        public string Username { get; set; }

        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}