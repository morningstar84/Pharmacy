using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AuthenticationService.API.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage="You must specify username between 4 and 100 chars")]
        public string Username { get; set; }   

        [Required]
        [StringLength(12, MinimumLength = 4, ErrorMessage="You must specify a password between 4 and 8 chars")]
        public string Password { get; set; }
    }
}