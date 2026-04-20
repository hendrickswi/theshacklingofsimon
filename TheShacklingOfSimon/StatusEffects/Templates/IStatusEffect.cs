#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Templates;

public interface IStatusEffect
{
    string Name { get; }
    EffectType Type { get; }
    bool IsFinished { get; }
    IDamageableEntity Owner { get; }

    /// <summary>
    /// Executes logic for when a status effect is applied to its owner.
    /// This method is typically used to initialize or configure the effect,
    /// alter properties of the owner, or trigger associated behaviors that occur upon application.
    /// </summary>
    void OnApply();

    /// <summary>
    /// Handles logic for when a status effect is removed from its owner.
    /// This method is typically used to clean up resources, reset altered properties,
    /// or perform specific actions associated with the status effect's removal.
    /// </summary>
    void OnRemove();

    /// <summary>
    /// Updates the state of the status effect based on the elapsed game time.
    /// </summary>
    /// <param name="delta">An object of type <c>GameTime</c> representing the elapsed time
    /// since the last update call. This is used to progress the internal state
    /// or application of the status effect.</param>
    void Update(GameTime delta);

    /// <summary>
    /// Merges <c>this</c> and <c>other</c>.
    /// The resultant merged effect is represented in <c>this</c>.
    /// The resultant merged effect is arbitrarily determined by the implementation.
    /// </summary>
    /// <param name="other">The other instance of <c>this</c> to be merged.</param>
    void Merge(IStatusEffect other);

    /// <summary>
    /// Creates a new instance of <c>this</c> that is linked with the specified target entity.
    /// The cloned status effect is an independent copy of the original, bound to a different owner.
    /// </summary>
    /// <param name="newTarget">The entity to associate the cloned status effect with.</param>
    /// <returns>A new instance of the status effect attached to the specified new target.</returns>
    IStatusEffect Clone(IDamageableEntity newTarget);
}