using System.Text;
using System.Threading.Tasks;
using DateApp.Core.Contracts.DAL;
using DateApp.Core.EntityModels;
using DateApp.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DateApp.DAL.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<AuthRepository> logger;

        public AuthRepository(DataContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            this.logger = logger;
        }
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordSalt, out passwordHash);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x => x.UserName.Equals(username));
            if (user == null)
            {
                logger.LogError($"Invalid user - {username}");
                return null;
            }

            if (!VerifyPasswordHash(user.PasswordHash, user.PasswordSalt, password))
            {
                logger.LogError($"Invalid password for user - {username}");
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(byte[] passwordHash, byte[] passwordSalt, string password)
        {
            using (var hash = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (var i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.UserName.Equals(username)))
                return true;
            return false;
        }
        private void CreatePasswordHash(string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hash = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
