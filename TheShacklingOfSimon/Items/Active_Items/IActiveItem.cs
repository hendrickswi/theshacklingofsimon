#region

using Microsoft.Xna.Framework;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;

/// <summary>
/// Extended interface for active items with additional
/// functionality for time-based logic.
/// </summary>
public interface IActiveItem : IItem
{
    void ClearEffect();
    void Update(GameTime delta);
}