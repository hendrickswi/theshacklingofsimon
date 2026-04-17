#region

using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.Items;

/// <summary>
/// The base interface for all items. Base set of functionality.
/// </summary>
public interface IItem
{
    string Name { get; }
    string Description { get; }
    IDamageableEntity Entity { get; set; }
    
    /// <summary>
    /// Attempts to apply the item's effect to <c>this.Entity</c>.
    /// </summary>
    /// <remarks>
    /// Ensure that <c>this.Entity</c> is the desired target before calling this method.
    /// Otherwise, unexpected behavior may occur.
    /// </remarks>
    /// <returns>
    /// True if the effect was successfully applied, false otherwise.
    /// </returns>
    bool ApplyEffect();
}