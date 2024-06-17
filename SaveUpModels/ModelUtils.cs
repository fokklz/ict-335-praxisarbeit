using SaveUpModels.Common.Attributes;
using SaveUpModels.Common.Options;
using System.Reflection;

namespace SaveUpModels
{
    public class ModelUtils
    {
        public static bool IsAllowed(PropertyInfo? prop, DTOParseOptions options)
        {
            if (prop == null) return false;

            var adminOnly = prop?.GetCustomAttribute<AdminOnlyAttribute>() != null;
            var ownerOrAdminOnly = prop?.GetCustomAttribute<OwnerOrAdminOnlyAttribute>() != null;

            if (ownerOrAdminOnly)
            {
                return options.IsOwner || options.IsAdmin;
            }
            if (adminOnly)
            {
                return options.IsAdmin;
            }

            return true;
        }
    }
}
