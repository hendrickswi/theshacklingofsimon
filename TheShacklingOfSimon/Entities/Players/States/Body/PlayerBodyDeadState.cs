#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

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
        string spritePrefix = _player.SpritesManager.GetSkin("Body");
        _player.SpritesManager.Body = SpriteFactory.Instance.CreateAnimatedSprite(spritePrefix + "DeadAnimation", _frameDuration);
        SoundManager.Instance.PlaySFX(_player.DieSFX);
        MediaPlayer.Stop();
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
        if (!_switched && _timer >= _spriteSwitchTime)
        {
            string spritePrefix = _player.SpritesManager.GetSkin("Body");
            _player.SpritesManager.Body = SpriteFactory.Instance.CreateStaticSprite(spritePrefix + "DeadFinal");
            _switched = true;
        }
    }

    public void HandleMovement(Vector2 direction, float frameDuration)
    {
        // No movement allowed
        _player.Velocity = Vector2.Zero;
    }
}