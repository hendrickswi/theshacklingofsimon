#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Items.Active_Items;

public class TeleportItem : ActiveItem, IInventoryItem
{
    private readonly string _sfx;
    private float _timer;
    private readonly float _cooldownDuration;
    private bool _buffActive;

    private readonly Func<Vector2, bool> _isValidPosition;

    private readonly float _blinkDistance;
    private readonly float _step; // used to “walk back” if destination is invalid

    public TeleportItem(
        IDamageableEntity entity,
        Func<Vector2, bool> isValidPosition,
        float blinkDistance = 96f,
        float cooldownSeconds = 2.0f,
        float step = 8f) 
        : base(entity)
    {
        Entity = entity;
        _isValidPosition = isValidPosition;

        _blinkDistance = blinkDistance;
        _cooldownDuration = cooldownSeconds;
        _step = step;

        Name = "Blink";
        Description = "Teleports you a short distance forward.";

        SFX = SoundManager.Instance.AddSFX("items","warp");
    }
    
    public override void Update(GameTime gameTime)
    {
        if (!_buffActive) return;
        _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_timer <= 0f)
        {
            _buffActive = false;
        }
    }
    
    public override bool ApplyEffect()
    {
        if (_timer > 0f) return false;

        Vector2 dir = Entity.Velocity;
        if (dir.LengthSquared() < 0.0001f) return false;
        dir.Normalize();
        
        Vector2 target = Entity.Position + dir * _blinkDistance;

        Vector2 candidate = target;
        float traveledBack = 0f;
        while (traveledBack <= _blinkDistance)
        {
            if (_isValidPosition(candidate))
            {
                // Don't consume if the entity shouldn't move
                if ((candidate - Entity.Position).LengthSquared() < 0.0001f) return false;

                Entity.SetPosition(target);
                _buffActive = true;
                _timer = _cooldownDuration;
                SoundManager.Instance.PlaySFX(_sfx);
                return true;
            }

            // Move back a bit and check again
            candidate -= dir * _step;
            traveledBack += _step;
        }
        return false;
    }
}