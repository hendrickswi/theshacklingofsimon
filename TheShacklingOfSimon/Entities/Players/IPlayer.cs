#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States;

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
    /// Sets the skin prefix for the specified category by invoking the internal SkinManager.
    /// </summary>
    /// <param name="category">The category for which the skin prefix is being set.</param>
    /// <param name="skinPrefix">The skin prefix to be associated with the specified category.</param>
    void SetSkin(string category, string skinPrefix);

    /// <summary>
    /// Retrieves the skin prefix for the specified category of <c>this</c>.
    /// </summary>
    /// <param name="category">The category for which the skin prefix is being requested.</param>
    /// <returns>The skin prefix associated with the specified category.</returns>
    string GetSkin(string category);

    /// <summary>
    /// Changes the current state of <c>this</c> player to the specified new state.
    /// </summary>
    /// <param name="newState">The new state to which <c>this</c> should transition.
    /// Must be an instance of an appropriate subinterface. This varies by player implementation.</param>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="newState"/>
    /// is not of an appropriate subinterface type.</exception>
    void ChangeState(IPlayerState newState);
}
