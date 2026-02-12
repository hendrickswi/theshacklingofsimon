using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Players.States;

public class PlayerBodyMovingState : IPlayerBodyState
{
    private string _currentAnimation;
    
    public void Update(IPlayer player, GameTime delta)
    {
        
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        
    }
    
    public void Enter(IPlayer player)
    {
        
    }

    public void Exit(IPlayer player)
    {
        player.Sprite = SpriteFactory.Instance.CreateAnimatedSprite("PlayerBodyIdle");
    }

    public void HandleMovement(IPlayer player, Vector2 direction)
    {
        player.Velocity = direction;
    }

    private void UpdateSprite(IPlayer player)
    {
        string newAnimationName = "PlayerWalkVertical";
        
        /*
         * Walking animation is horizontally biased.
         * e.g., If walking northeast (both up and right),
         * the horizontal walk animation is played.
         */
        if (player.Velocity.X != 0)
        {
            player.Sprite = SpriteFactory.Instance.CreateAnimatedSprite("PlayerWalkHorizontal");
        }
        else if (player.Velocity.Y != 0)
        {
            player.Sprite = SpriteFactory.Instance.CreateAnimatedSprite("PlayerWalkVertical");
        }

        if (newAnimationName != _currentAnimation)
        {
            player.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName);
            _currentAnimation = newAnimationName;
        }
    }
}