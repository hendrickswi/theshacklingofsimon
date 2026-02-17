using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players.States.Head;

public class PlayerHeadAttackingState : IPlayerHeadState
{
    private PlayerWithTwoSprites _player;
    private IWeapon _weapon;
    private Vector2 _direction;
    private float _timer;
    private readonly float _stateDuration;

    public PlayerHeadAttackingState(PlayerWithTwoSprites player, IWeapon weapon, Vector2 direction, float stateDuration)
    {
        this._player = player;
        this._weapon = weapon;
        this._direction = direction;
        this._stateDuration = stateDuration;
    }

    public void Enter()
    {
        // _direction is already "cardinalized" from PlayerHeadIdleState
        _weapon.Fire(_player.Position, _direction, new ProjectileStats(1.0f * _player.DamageMultiplierStat, 200.0f));

        string spriteAnimationName = "PlayerHeadShootingDown";
        if (_direction.X > 0)
        {
            spriteAnimationName = "PlayerHeadShootingRight";
        }
        else if (_direction.X < 0)
        {
            spriteAnimationName = "PlayerHeadShootingLeft";
        }
        else if (_direction.Y < 0)
        {
            spriteAnimationName = "PlayerHeadShootingUp";
        }

        _player.HeadSprite = SpriteFactory.Instance.CreateAnimatedSprite(spriteAnimationName, _stateDuration);
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
            _player.ChangeHeadState(new PlayerHeadIdleState(_player, new Vector2(0, 1)));
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