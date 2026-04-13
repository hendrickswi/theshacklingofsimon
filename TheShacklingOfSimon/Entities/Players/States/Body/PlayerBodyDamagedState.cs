#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.States.Head;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Entities.Players.States.Body;

public class PlayerBodyDamagedState : IPlayerBodyState
{
    private PlayerWithTwoSprites _player;
    private float _timer;
    private readonly float _stateDuration;

    public PlayerBodyDamagedState(PlayerWithTwoSprites player, float stateDuration)
    {
        _player = player;
        _stateDuration = stateDuration;
    }
    
    public void Enter()
    {
        string spritePrefix = _player.SpritesManager.GetSkin("Body");
        _player.SpritesManager.Body = SpriteFactory.Instance.CreateStaticSprite(spritePrefix + "Hurt");
        SoundManager.Instance.PlaySFX(_player.HurtSFX);
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
        if (_timer >= _stateDuration)
        {
            _player.StatesManager.ChangeBodyState(new PlayerBodyIdleState(_player));
            _player.StatesManager.ChangeHeadState(new PlayerHeadIdleState(_player, new Vector2(0, 1)));
        }
    }

    public void HandleMovement(Vector2 direction, float frameDuration)
    {
        if (direction.LengthSquared() < 0.0001f)
        {
            _player.Velocity = Vector2.Zero;
        }
        else
        {
            _player.Velocity = direction * _player.GetStat(StatType.MoveSpeed);
        }
    }
}
