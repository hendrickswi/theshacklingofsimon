using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Players.States.Head;

public class PlayerHeadIdleState : IPlayerHeadState
{
    private PlayerWithTwoSprites _player;
    private string _currentAnimation;
    private Vector2 _lookingDirection;

    public PlayerHeadIdleState(PlayerWithTwoSprites player, Vector2 lastDirection)
    {
        _player = player;
        
        // Default to looking down
        _lookingDirection = (lastDirection.Length() < 0.0001f) ? new Vector2(0, 1) : lastDirection;
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
        if (_player.HeadSprite != null)
        {
            _player.HeadSprite.Update(delta);
        }
    }

    public void HandlePrimaryAttack(Vector2 direction, float stateDuration)
    {
        Vector2 cardinal = GetCardinalDirection(direction);
        if (cardinal != Vector2.Zero)
        {
            _player.ChangeHeadState(new PlayerHeadAttackingState(_player, _player.CurrentPrimaryWeapon, cardinal, stateDuration));
        }
    }

    public void HandleSecondaryAttack(Vector2 direction, float stateDuration)
    {
        Vector2 cardinal = GetCardinalDirection(direction);
        if (cardinal != Vector2.Zero)
        {
            _player.ChangeHeadState(new PlayerHeadAttackingState(_player, _player.CurrentSecondaryWeapon, cardinal, stateDuration));
        }
    }
    
    private void UpdateSprite()
    {
        string newAnimationName = "PlayerHeadIdleDown";
        Vector2 cardinal = GetCardinalDirection(_lookingDirection);
        
        if (cardinal == Vector2.UnitX)
        {
            newAnimationName = "PlayerHeadIdleRight";
        }
        else if (cardinal == -Vector2.UnitX)
        {
            newAnimationName = "PlayerHeadIdleLeft";
        }
        else if (cardinal == -Vector2.UnitY)
        {
            newAnimationName = "PlayerHeadIdleUp";
        }

        if (newAnimationName != _currentAnimation)
        {
            _player.HeadSprite = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName);
            _currentAnimation = newAnimationName;
        }
    }

    private Vector2 GetCardinalDirection(Vector2 input)
    {
        Vector2 cardinal = Vector2.Zero;
        if (Math.Sqrt(input.X * input.X + input.Y * input.Y) > float.Epsilon)
        {
            if (Math.Abs(input.X) > Math.Abs(input.Y))
            {
                cardinal = new Vector2(Math.Sign(input.X), 0);
            }
            else
            {
                cardinal = new Vector2(0, Math.Sign(input.Y));
            }
        }
        return cardinal;
    }
}