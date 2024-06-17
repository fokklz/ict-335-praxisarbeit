using Microsoft.AspNetCore.Authorization;

namespace SaveUpBackend.Common.Attributes
{
    /// <summary>
    /// Custom Authorize Attribute
    /// </summary>
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(string role = null)
        {
            if (!string.IsNullOrEmpty(role))
            {
                Roles = role;
            }
        }
    }
}
