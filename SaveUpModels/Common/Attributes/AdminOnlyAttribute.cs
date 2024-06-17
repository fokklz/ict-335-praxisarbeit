namespace SaveUpModels.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AdminOnlyAttribute : Attribute
    {
        // This class doesn't need any additional logic for now.
        // It serves as a marker on properties.
    }
}
