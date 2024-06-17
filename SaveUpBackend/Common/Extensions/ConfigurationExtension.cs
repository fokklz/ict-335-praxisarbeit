using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SaveUpBackend.Common.Extensions
{
    public static class ConfigurationExtension
    {
        /// <summary>
        /// Parameters for TokenValidation which should be used for the Application Tokens
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>a <see cref="TokenValidationParameters"/> instance</returns>
        public static TokenValidationParameters GetTokenValidationParameters(this IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JWT");

            var key = jwtConfig.GetValue("Key", "PLEASE_PROVIDE_A_VALID_STRONG_LONG_AND_SECURE_TOKEN")!;
            var audience = jwtConfig.GetValue("Audience", "SaveUp API");
            var issuer = jwtConfig.GetValue("Issuer", "SaveUp API");

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidAudience = audience,
                ValidIssuer = issuer,
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
    }
}
