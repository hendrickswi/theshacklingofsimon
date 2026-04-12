#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Entities.Players;

public interface IPlayer : IDamageableEntity
{
    PlayerInventory Inventory { get; }
    PlayerInputBuffer InputBuffer { get; }
    
    // The context of the State pattern
    IPlayerState CurrentState { get; }

    /// <summary>
    /// Completely resets <c>this</c> to the default state. Also resets the player's position
    /// to the provided starting position.
    /// </summary>
    /// <param name="startPosition">The initial position to which <c>this</c> should be reset.</param>
    void Reset(Vector2 startPosition);

    /// <summary>
    /// Sets the skin prefix for the specified category.
    /// </summary>
    /// <param name="category">The category for which the skin prefix is being set.</param>
    /// <param name="skinPrefix">The skin prefix to be associated with the specified category.</param>
    void SetSkin(string category, string skinPrefix);

    /// <summary>
    /// Retrieves the skin prefix for the specified category.
    /// </summary>
    /// <param name="category">The category for which the skin prefix is being requested.</param>
    /// <returns>The skin prefix associated with the specified category.</returns>
    string GetSkin(string category);
    
    /// <summary>
    /// Retrieves a list of all active status effects <c>this</c> is currently affected by.
    /// </summary>
    /// <returns>An object of type <c>IEnumerable</c> collection of objects of type <c>IStatusEffect</c></returns>
    IEnumerable<IStatusEffect> GetActiveEffects();
}
