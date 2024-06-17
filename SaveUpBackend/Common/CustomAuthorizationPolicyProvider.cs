using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SaveUpBackend.Common
{
    /// <summary>
    /// Custom Authorization Policy Provider
    /// Allows to override the Role on a per endpoint basis
    /// </summary>
    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options) { }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Try to get a policy with the base implementation first
            var policy = await base.GetPolicyAsync(policyName);
            if (policy == null && !string.IsNullOrEmpty(policyName) && policyName.StartsWith("CustomRole"))
            {
                var roles = policyName.Substring("CustomRole".Length).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.RequireAuthenticatedUser();
                // This accepts an array and requires one of the roles
                policyBuilder.RequireRole(roles);  
                return policyBuilder.Build();
            }

            return policy;
        }
    }
}
