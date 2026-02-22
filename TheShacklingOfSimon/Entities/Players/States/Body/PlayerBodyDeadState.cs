using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States.Head;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Players.States.Body;

public class PlayerBodyDeadState : IPlayerBodyState
{
    private readonly PlayerWithTwoSprites _player;
    private readonly float _frameDuration;
    private readonly float _spriteSwitchTime;
    private float _timer;
    private bool _switched = false;

    public PlayerBodyDeadState(PlayerWithTwoSprites player, float frameDuration, float spriteSwitchTime)
    {
        _player = player;
        _frameDuration = frameDuration;
        _spriteSwitchTime = spriteSwitchTime;
    }
    
    public void Enter()
    {
        _player.BodySprite = SpriteFactory.Instance.CreateAnimatedSprite("PlayerDeadAnimation", _frameDuration);
    }

    public void Exit()
    {
        /*
         * Could use this later for stopping any sounds related to moving (e.g., walking)
         */
    }

    public void Update(GameTime delta)
    {
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;
        
        /*
         * Only allow switching once
         *      i.e., dying animated sprite -> static dead sprite
         */
        if (!_switched)
        {
            _player.BodySprite.Update(delta);
            if (_timer >= _spriteSwitchTime)
            {
                _player.BodySprite = SpriteFactory.Instance.CreateStaticSprite("PlayerDeadFinal");
                _switched = true;
            }
        }
    }

    public void HandleMovement(Vector2 direction, float frameDuration)
    {
        // No movement allowed
        _player.Velocity = Vector2.Zero;
    }
}