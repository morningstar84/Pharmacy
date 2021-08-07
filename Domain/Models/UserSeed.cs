using System.IO;
using System.Linq;
using Data;
using Data.Models;
using Domain.Services;

namespace Domain.Models
{
    public class UserSeed
    {
        public static void Seed(PharmacyDatabaseContext context, ICryptoService cryptoService)
        {
            if (!context.Users.Any())
            {
                var json = File.ReadAllText("../Domain/Models/User.json");
                var adminUser = User.FromJson(json);
                byte[] passHash, passSalt;
                cryptoService.CreatePasswordHash("scrubs651", out passHash, out passSalt);
                adminUser.PasswordHash = passHash;
                adminUser.PasswordSalt = passSalt;
                adminUser.Username = adminUser.Username.ToLower();
                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}