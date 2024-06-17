using MongoDB.Driver;
using SaveUpBackend.Common;
using SaveUpBackend.Common.Enums;
using SaveUpBackend.Interfaces;
using SaveUpModels.Models;

namespace SaveUpBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoDBContext _context;

        public AuthService(IMongoDBContext context)
        {
            _context = context;
        }

        public async Task<TaskResult<User>> VerifyPasswordAsync(string username, string password)
        {
            var user = await _context.Users.FindSingleAsync(Builders<User>.Filter.Eq(x => x.Username, username));
            if (user == null) return CreateTaskResult.Error<User>(ErrorKey.ENTRY_NOT_FOUND);

            if (!user.Locked && VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                user.LoginAttempts = 0;
                await _context.Users.Collection.ReplaceOneAsync(Builders<User>.Filter.Eq(x => x.Id, user.Id), user);
                return CreateTaskResult.Success(user);
            }
            else if (user.Locked)
            {
                return CreateTaskResult.Error<User>(ErrorKey.USER_LOCKED);
            }

            user.LoginAttempts++;
            if (user.LoginAttempts >= 3)
            {
                user.Locked = true;
            }
            await _context.Users.Collection.ReplaceOneAsync(Builders<User>.Filter.Eq(x => x.Id, user.Id), user);
            return CreateTaskResult.Error<User>(ErrorKey.INVALID_CREDENTIALS);
        }

        /// <summary>
        /// Generate a Password Hash
        /// </summary>
        /// <param name="password">The Password</param>
        /// <param name="passwordHash">out Hash</param>
        /// <param name="passwordSalt">out Salt</param>
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Helper Method to verify a Password
        /// to encapsulate the logic using hmac
        /// </summary>
        /// <param name="password">The Password</param>
        /// <param name="passwordHash">The Hash</param>
        /// <param name="passwordSalt">The Salt</param>
        /// <returns>True if it maches</returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
