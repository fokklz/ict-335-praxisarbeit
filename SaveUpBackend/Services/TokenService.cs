using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using SaveUpBackend.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SaveUpBackend.Common.Extensions;
using SaveUpModels.DTOs;
using SaveUpModels.Models;
using SaveUpBackend.Common;
using SaveUpBackend.Common.Enums;
using System.Diagnostics;

namespace SaveUpBackend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IMongoDBContext _context;
        private readonly IConfiguration _configuration;

        private readonly SecurityKey _key;

        public TokenService(IConfiguration configuration, IMongoDBContext context)
        {
            _context = context;
            _configuration = configuration;

            _key = configuration.GetTokenValidationParameters().IssuerSigningKey;
        }

        /// <summary>
        /// Create a token for a user. When a refresh token is created it is stored in the database
        /// </summary>
        /// <param name="user">The user to create a token for</param>
        /// <param name="keep">whether to include a refresh token or not</param>
        /// <returns>The token data for the new Token</returns>
        public async Task<TokenData> CreateToken(User user, bool keep = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = keep ? GenerateRefreshToken(user) : null;
            await _context.Users.Collection.UpdateOneAsync(Builders<User>.Filter.Eq("_id", user.Id), Builders<User>.Update.Set(x => x.RefreshToken, refreshToken));

            return new TokenData
            {
                Token = tokenHandler.WriteToken(token),
                Expires = token.ValidTo,
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Refresh a token using a expired access token and a valid refresh token. 
        /// 
        /// The token is refreshed for the user and stored in the database before returning the new token
        /// </summary>
        /// <param name="token">a expired access token</param>
        /// <param name="refreshToken">a valid refresh token</param>
        /// <returns>a TaskResult containg a RefreshResult with the token information</returns>
        public async Task<TaskResult<RefreshResult>> RefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            if (!principal.IsSuccess) return CreateTaskResult.Error<RefreshResult>(principal);

            var userId = principal.Result.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return CreateTaskResult.Error<RefreshResult>(ErrorKey.INVALID_CREDENTIALS);

            var user = await _context.Users.FindByIdAsync(ObjectId.Parse(userId));
            if (user == null || user.RefreshToken != refreshToken) return CreateTaskResult.Error<RefreshResult>(ErrorKey.INVALID_CREDENTIALS);

            var tokenData = await CreateToken(user, true);
            return CreateTaskResult.Success(new RefreshResult
            {
                TokenData = tokenData,
                User = user
            });
        }

        /// <summary>
        /// Get the principal from an expired token
        /// </summary>
        /// <param name="token">The token to extract the principal for</param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException">Thrown when the token is invalid</exception>
        private TaskResult<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var validateParams = _configuration.GetTokenValidationParameters();
            validateParams.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validateParams, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                return CreateTaskResult.Error<ClaimsPrincipal>(ErrorKey.INVALID_CREDENTIALS, new SecurityTokenException("Invalid token"));

            return CreateTaskResult.Success(principal);
        }

        /// <summary>
        /// Helper to generate a refresh token for a user
        /// 
        /// This function does not store the refresh token in the database it only generates it
        /// </summary>
        /// <param name="user">The user to generate a token for</param>
        /// <returns>The generated refresh token</returns>
        private string GenerateRefreshToken(User user)
        {
            string userInfo = $"{user.Id};{user.Role};{Guid.NewGuid()}";
            using (var sha256 = SHA256.Create())
            {
                byte[] hashData = sha256.ComputeHash(Encoding.UTF8.GetBytes(userInfo));
                StringBuilder hexString = new StringBuilder(hashData.Length * 2);
                foreach (byte b in hashData)
                {
                    hexString.AppendFormat("{0:x2}", b);
                }

                Debug.WriteLine("---------------Refresh token generated");
                return hexString.ToString();
            }
        }
    }
}
