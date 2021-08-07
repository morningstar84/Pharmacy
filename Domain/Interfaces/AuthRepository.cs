using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace AuthenticationService.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PharmacyDatabaseContext _context;
        private readonly ICryptoService _cryptoService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AuthRepository(PharmacyDatabaseContext context, ICryptoService cryptoService)
        {
            _context = context;
            _cryptoService = cryptoService;
        }

        async Task<User> IAuthRepository.Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                _logger.Error("User " + username + " doesn't exist");
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.Error("Failed to verify passwd for user: " + username);
                return null;
            }

            _logger.Info("Login successful for user " + username);
            return user;
        }

        async Task<User> IAuthRepository.Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            _cryptoService.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        async Task<bool> IAuthRepository.UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username)) return true;
            return false;
        }

        async Task<User> IAuthRepository.GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return false;
            try
            {
                _context.Users.Remove(user);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while deleting user with id =" + id);
                return false;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                    if (computedHash[i] != passwordHash[i])
                        return false;
                return true;
            }
        }
    }
}