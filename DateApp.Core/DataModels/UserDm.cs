using System;
using System.ComponentModel.DataAnnotations;

namespace DateApp.Core.DataModels
{
    public class UserDm
    {
        [Required]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Username should have 7 to 20 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Password Length should be between 7 to 15 characters.")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }

        public UserDm()
        {
            LastActive = DateTime.Now;
            Created = DateTime.Now;
        }
    }
}
