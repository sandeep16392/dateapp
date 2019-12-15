using System.ComponentModel.DataAnnotations;

namespace DateApp.Core.DataModels
{
    public class LoginUserDm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
