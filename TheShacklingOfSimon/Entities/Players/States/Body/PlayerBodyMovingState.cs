#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Entities.Players.States.Body;

public class PlayerBodyMovingState : IPlayerBodyState
{
    private string _currentAnimation;
    private PlayerWithTwoSprites _player;
    private float _animationSpeed;

    public PlayerBodyMovingState(PlayerWithTwoSprites player, float animationSpeed)
    {
        _player = player;
        _animationSpeed = animationSpeed;
    }
    
    public void Enter()
    {
        UpdateSprite();
    }

    public void Exit()
    {
        /*
         * No-op for now
         * Could use this later for stopping any sounds related to moving (e.g., walking)
         */
    }
    
    public void Update(GameTime delta)
    {
    }

    public void HandleMovement(Vector2 direction, float frameDuration)
    {
        if (_player.GetStat(StatType.StunCount) > float.Epsilon)
        {
            _player.Velocity = Vector2.Zero;
            _player.StatesManager.ChangeBodyState(new PlayerBodyIdleState(_player));
            return;
        }
        
        if (direction.LengthSquared() < 0.0001f)
        {
            _player.Velocity = Vector2.Zero;
            _player.StatesManager.ChangeBodyState(new PlayerBodyIdleState(_player));
        }
        else
        {
            _player.Velocity = direction * _player.GetStat(StatType.MoveSpeed) * _player.GetStat(StatType.MoveSpeedMultiplier);
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    { 
        string newAnimationName = _player.SpritesManager.GetSkin("Body");
        
        /*
         * Walking animation is horizontally biased.
         * e.g., If walking northeast (both up and right),
         * the horizontal walk animation is played.
         */
        if (MathF.Abs(_player.Velocity.X) > float.Epsilon)
        {
            newAnimationName += "WalkHorizontal";
        }
        else if (MathF.Abs(_player.Velocity.Y) > float.Epsilon)
        {
            newAnimationName += "WalkVertical";
        }
        else
        {
            // Fallback to idle sprite
            newAnimationName += "Idle";
        }

        if (newAnimationName != _currentAnimation)
        {
            _player.SpritesManager.Body = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName, _animationSpeed);
            _currentAnimation = newAnimationName;
        }
    }
}