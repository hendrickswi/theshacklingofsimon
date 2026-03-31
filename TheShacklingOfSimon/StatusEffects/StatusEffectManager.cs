using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects;

public class StatusEffectManager
{
    private readonly Dictionary<Type, IStatusEffect> _activeEffects;
    private readonly DamageableEntity _owner;

    public StatusEffectManager(DamageableEntity owner)
    {
        _activeEffects = new Dictionary<Type, IStatusEffect>();
        _owner = owner;
    }

    public void AddEffect(IStatusEffect effect)
    {
        Type type = effect.GetType();
        if (_activeEffects.ContainsKey(type))
        {
            // Delegate merge logic to the concrete implementation
            _activeEffects[type].Merge(effect);
        }
        else
        {
            // New status effect can simply be added
            _activeEffects.Add(type, effect);
            effect.OnApply();
        }
    }

    public void Update(GameTime delta)
    {
        if (_activeEffects.Count <= 0) return;

        List<Type> markedForRemoval = new List<Type>();
        foreach (var pair in _activeEffects)
        {
            IStatusEffect effect = pair.Value;
            effect.Update(delta);
            if (effect.IsFinished)
            {
                markedForRemoval.Add(pair.Key);
            }
        }

        foreach (Type type in markedForRemoval)
        {
            _activeEffects[type].OnRemove();
            _activeEffects.Remove(type);
        }
    }

    public void ClearAll()
    {
        foreach (var effect in _activeEffects)
        {
            effect.Value.OnRemove();
        }
        _activeEffects.Clear();
    }
}