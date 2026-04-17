#region

using TheShacklingOfSimon.Items;

#endregion

namespace TheShacklingOfSimon.Entities.Pickup;
public interface IPickup : IEntity
{
    IItem Item { get; }
}