
using TheShacklingOfSimon.Entities.Players.States.Head;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

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
        _player.Sprite = SpriteFactory.Instance.CreateStaticSprite("");
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
        _timer += (float)delta.ElapsedGameTime.TotalSeconds;
        if (_timer >= _stateDuration)
        {
            _player.ChangeBodyState(new PlayerBodyIdleState(_player));
        }
        else
        {
            _player.HeadSprite.Update(delta);
        }
    }

    public void HandlePrimaryAttack(Vector2 direction, float stateDuration)
    {
        // No-op
    }

    public void HandleSecondaryAttack(Vector2 direction, float stateDuration)
    {
        // No-op
    }
}
