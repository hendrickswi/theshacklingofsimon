using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Players.States.Body;

public class PlayerBodyMovingState : IPlayerBodyState
{
    private string _currentAnimation;
    private PlayerWithTwoSprites _player;

    public PlayerBodyMovingState(PlayerWithTwoSprites player)
    {
        _player = player;
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
        _player.Sprite.Update(delta);
    }

    public void HandleMovement(Vector2 direction)
    {
        if (direction.Length() < 0.0001f)
        {
            _player.Velocity = Vector2.Zero;
            _player.ChangeBodyState(new PlayerBodyIdleState(_player));
        }
        else
        {
            _player.Velocity = direction * _player.MoveSpeedStat;
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        string newAnimationName = "PlayerWalkVertical";
        
        /*
         * Walking animation is horizontally biased.
         * e.g., If walking northeast (both up and right),
         * the horizontal walk animation is played.
         */
        if (_player.Velocity.X != 0)
        {
            newAnimationName = "PlayerWalkHorizontal";
        }
        else if (_player.Velocity.Y != 0)
        {
            newAnimationName = "PlayerWalkVertical";
        }

        if (newAnimationName != _currentAnimation)
        {
            _player.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName);
            _currentAnimation = newAnimationName;
        }
    }
}