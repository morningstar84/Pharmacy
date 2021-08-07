using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IPrincipal GetIdentityFromToken(string token)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new List<SymmetricSecurityKey>();
            creds.Add(key);

            var validationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = false, // ignoring fronted request validation
                    ValidateAudience = false, // ignoring API endpoint validation
                    IssuerSigningKeys = creds
                };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            IPrincipal principal = null;
            try
            {
                principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception)
            {
                _logger.Info("Token verification failed");
                return null;
            }

            _logger.Info("Token verification suceeded");
            return principal;
        }

        public (string, long) GenerateNewTokenForIdentity(string username, string identifier, UserRole role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, identifier),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var unixTime = ((DateTimeOffset)tokenDescriptor.Expires.GetValueOrDefault()).ToUnixTimeSeconds();
            return (tokenHandler.WriteToken(token), unixTime);
        }
    }
}