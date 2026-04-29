#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects;

public class StatusEffectManager
{
    private readonly Dictionary<EffectType, IStatusEffect> _activeTemporaryEffects;
    private readonly List<IStatusEffect> _activePermanentEffects;

    // Read-only public accessors (for GUI, etc.)
    public IEnumerable<IStatusEffect> ActiveEffects => _activeTemporaryEffects.Values;
    public IEnumerable<IStatusEffect> PermanentEffects => _activePermanentEffects;
    
    public StatusEffectManager()
    {
        _activeTemporaryEffects = new Dictionary<EffectType, IStatusEffect>();
        _activePermanentEffects = new List<IStatusEffect>();
    }

    /// <summary>
    /// Adds a status effect to the active temporary effects list.
    /// </summary>
    /// <param name="newEffect">The status effect to be added or merged into the active effects list.</param>
    /// <remarks>
    /// If an effect of the same type already exists in the internal representation, the new effect is merged with the
    /// existent effect, then reapplied. If no effect of the same type exists, the provided effect
    /// is added as a new entry to the internal representation and applied.
    /// </remarks>
    public void AddTemporaryEffect(IStatusEffect newEffect)
    {
        EffectType type = newEffect.Type;
        if (_activeTemporaryEffects.ContainsKey(type))
        {
            var oldEffect = _activeTemporaryEffects[type];
            oldEffect.OnRemove();
            
            // Delegate merge logic to the concrete implementation
            oldEffect.Merge(newEffect);
            oldEffect.OnApply();
        }
        else
        {
            // New status effect can simply be added
            _activeTemporaryEffects.Add(type, newEffect);
            newEffect.OnApply();
        }
    }

    /// <summary>
    /// Adds a status effect to the list of active permanent effects.
    /// </summary>
    /// <param name="newEffect">The status effect to be added to the active permanent effects list.</param>
    /// <remarks>
    /// Permanent effects are stored separately from temporary effects and do not expire.
    /// The provided effect is directly added to the internal representation with no merging logic applied.
    /// </remarks>
    public void AddPermanentEffect(IStatusEffect newEffect)
    {
        if (newEffect == null) return;
        _activePermanentEffects.Add(newEffect);
        newEffect.OnApply();
    }

    public void Update(GameTime delta)
    {
        if (_activeTemporaryEffects.Count <= 0) return;

        List<EffectType> markedForRemoval = new List<EffectType>();
        foreach (var pair in _activeTemporaryEffects)
        {
            IStatusEffect effect = pair.Value;
            effect.Update(delta);
            if (effect.IsFinished)
            {
                markedForRemoval.Add(pair.Key);
            }
        }

        foreach (var type in markedForRemoval)
        {
            _activeTemporaryEffects[type].OnRemove();
            _activeTemporaryEffects.Remove(type);
        }
    }

    /// <summary>
    /// Removes the specified status effect from the internal temporary effects representation, if it exists.
    /// </summary>
    /// <param name="effect">The status effect to be cleared.</param>
    /// <returns>
    /// True if the effect was successfully removed from the list; otherwise, false.
    /// </returns>
    public bool ClearTemporaryEffect(IStatusEffect effect)
    {
        if (effect == null) return false;
        return _activeTemporaryEffects.Remove(effect.Type);
    }

    /// <summary>
    /// Removes the specified status effect from the internal permanent effects representation, if it exists.
    /// </summary>
    /// <param name="effect">The status effect to be removed from the active permanent effects list.</param>
    /// <returns>
    /// True if the effect was successfully removed from the list; otherwise, false.
    /// </returns>
    public bool ClearPermanentEffect(IStatusEffect effect)
    {
        if (effect == null) return false;
        if (!_activePermanentEffects.Contains(effect)) return false;
        
        effect.OnRemove();
        return _activePermanentEffects.Remove(effect);
    }

    /// <summary>
    /// Removes all status effects from the internal representations.
    /// </summary>
    /// <remarks>
    /// This method also calls the <c>OnRemove()</c> method on each status effect in the internal representations.
    /// </remarks>
    public void ClearAllEffects()
    {
        foreach (var pair in _activeTemporaryEffects)
        {
            pair.Value.OnRemove();
        }
        _activeTemporaryEffects.Clear();
    }
}