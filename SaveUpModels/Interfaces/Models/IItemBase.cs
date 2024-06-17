using SaveUpModels.Interfaces.Base;

namespace SaveUpModels.Interfaces.Models
{
    public interface IItemBase : IModelBase
    {
        string Description { get; set; }
        string Name { get; set; }
        int? Price { get; set; }
    }
}