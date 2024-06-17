namespace SaveUpModels.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModelTypeAttribute : Attribute
    {
        public Type ModelType { get; private set; }

        public ModelTypeAttribute(Type modelType)
        {
            ModelType = modelType;
        }
    }
}
