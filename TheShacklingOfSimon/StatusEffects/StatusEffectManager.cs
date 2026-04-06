#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects;

public class StatusEffectManager
{
    private readonly Dictionary<EffectType, IStatusEffect> _activeEffects;

    public StatusEffectManager()
    {
        _activeEffects = new Dictionary<EffectType, IStatusEffect>();
    }

    /// <summary>
    /// Adds a status effect to the active effects list. If an effect of the same
    /// type already exists in the list, the existing effect is merged with the
    /// provided effect. If no effect of the same type exists, the provided effect
    /// is added as a new entry and reapplied.
    /// </summary>
    /// <param name="effect">The status effect to be added or merged into the active effects list.</param>
    public void AddEffect(IStatusEffect effect)
    {
        EffectType type = effect.Type;
        if (_activeEffects.ContainsKey(type))
        {
            // Delegate merge logic to the concrete implementation
            _activeEffects[type].Merge(effect);
            effect.OnRemove();
        }
        else
        {
            // New status effect can simply be added
            _activeEffects.Add(type, effect);
        }
        
        effect.OnApply();
    }

    public void Update(GameTime delta)
    {
        if (_activeEffects.Count <= 0) return;

        List<EffectType> markedForRemoval = new List<EffectType>();
        foreach (var pair in _activeEffects)
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
            _activeEffects[type].OnRemove();
            _activeEffects.Remove(type);
        }
    }

    /// <summary>
    /// Clears the specified status effect from the active effects list if it exists.
    /// Supports removing "infinite" duration status effects (i.e., effects caused
    /// by equipping an item).
    /// 
    /// </summary>
    /// <param name="effect">The status effect to be cleared.</param>
    public void ClearEffect(IStatusEffect effect)
    {
        if (_activeEffects.ContainsValue(effect))
        {
            _activeEffects.Remove(effect.Type);
        }
    }

    /// <summary>
    /// Removes all status effects from the active effects list. This action clears the
    /// entire collection of effects, regardless of their type, duration, or source.
    /// Performs any necessary cleanup by calling the appropriate removal logic for
    /// each effect.
    /// </summary>
    public void ClearAllEffects()
    {
        foreach (var pair in _activeEffects)
        {
            pair.Value.OnRemove();
        }
        _activeEffects.Clear();
    }
}