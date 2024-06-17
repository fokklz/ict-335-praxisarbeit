namespace SaveUpModels.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OwnerOrAdminOnlyAttribute : Attribute
    {
        // This class doesn't need any additional logic for now.
        // It serves as a marker on properties.
    }
}
