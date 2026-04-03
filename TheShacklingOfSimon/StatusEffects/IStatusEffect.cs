#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects;

public interface IStatusEffect
{
    bool IsFinished { get; }
    IDamageableEntity Owner { get; }
    
    void OnApply();
    void OnRemove();
    void Update(GameTime delta);
    
    /// <summary>
    /// Merges <c>this</c> and <c>other</c>.
    /// The resultant merged effect is represented in <c>this</c>.
    /// The resultant merged effect is arbitrarily determined by the implementation.
    /// </summary>
    /// <param name="other">The other instance of <c>this</c> to be merged.</param>
    void Merge(IStatusEffect other);
}