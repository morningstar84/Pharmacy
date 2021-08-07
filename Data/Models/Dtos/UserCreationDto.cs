using AuthenticationService.API.Dtos;

namespace Data.Models.Dtos
{
    public class UserCreationDto : UserRegisterDto
    {
        public UserRole UserRole { get; set; }
    }
}