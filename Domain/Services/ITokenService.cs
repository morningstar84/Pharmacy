using System.Security.Principal;
using Data.Models;

namespace Domain.Services
{
    public interface ITokenService
    {
        IPrincipal GetIdentityFromToken(string token);
        (string, long) GenerateNewTokenForIdentity(string username, string identifier, UserRole role);
    }
}