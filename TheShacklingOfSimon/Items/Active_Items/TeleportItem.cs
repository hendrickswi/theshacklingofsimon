using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items;

public class TeleportItem : IItem
{
    public string Name { get; }
    public string Description { get; }
    public IPlayer Player { get; }
    public ItemEffects Effects { get; } // unused for teleport, but required by the interface

    private readonly Func<Vector2, bool> _isValidPosition;

    private readonly float _cooldownSeconds;
    private float _cooldownTimer;

    private readonly float _blinkDistance;
    private readonly float _step; // used to “walk back” if destination is invalid

    public TeleportItem(
        IPlayer player,
        Func<Vector2, bool> isValidPosition,
        float blinkDistance = 96f,
        float cooldownSeconds = 2.0f,
        float step = 8f)
    {
        Player = player;
        _isValidPosition = isValidPosition;

        _blinkDistance = blinkDistance;
        _cooldownSeconds = cooldownSeconds;
        _step = step;

        Name = "Blink";
        Description = "Teleports you a short distance forward.";
        Effects = new ItemEffects(0, 0, 0, 0, false);
    }

    // Call this once per frame from wherever we update items / player
    public void Update(GameTime gameTime)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    // Trigger this when the player uses the active item
    public void Effect()
    {
        if (_cooldownTimer > 0f) return;

        Vector2 dir = Player.Velocity;

        // If player isn't moving, don't blink and don't consume cooldown
        if (dir.LengthSquared() < 0.0001f)
            return;

        dir.Normalize();

        Vector2 start = Player.Position;
        Vector2 target = start + dir * _blinkDistance;

        Vector2 candidate = target;
        float traveledBack = 0f;

        while (traveledBack <= _blinkDistance)
        {
            if (_isValidPosition(candidate))
            {
                // Extra safety: don't consume if we didn't actually move
                if ((candidate - start).LengthSquared() < 0.0001f)
                    return;

                Player.TeleportTo(candidate);
                _cooldownTimer = _cooldownSeconds;
                return;
            }

            candidate -= dir * _step;
            traveledBack += _step;
        }

        // If nowhere valid, do nothing (no cooldown spent)
    }
}