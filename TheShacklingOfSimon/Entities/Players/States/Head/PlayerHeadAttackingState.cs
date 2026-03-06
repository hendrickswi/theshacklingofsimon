using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players.States.Head;

public class PlayerHeadAttackingState : IPlayerHeadState
{
    private readonly PlayerWithTwoSprites _player;
    private readonly IWeapon _weapon;
    private readonly Vector2 _direction;
    private float _timer;
    private readonly float _stateDuration;
    private readonly float _extraProjectileOffset = 10.0f;

    public PlayerHeadAttackingState(PlayerWithTwoSprites player, IWeapon weapon, Vector2 direction, float stateDuration)
    {
        _player = player;
        _weapon = weapon;
        _direction = direction;
        _timer = 0.0f;
        _stateDuration = stateDuration;
    }

    public void Enter()
    {
        // _direction is already "cardinalized" from PlayerHeadIdleState
        Vector2 projectileStartPos;
        if (_direction == Vector2.UnitX)
        {
            projectileStartPos = new Vector2(_player.Position.X + _player.Hitbox.Width + _extraProjectileOffset, _player.Position.Y);
        }
        else if (_direction == -Vector2.UnitX)
        {
            projectileStartPos = new Vector2(_player.Position.X - _extraProjectileOffset, _player.Position.Y);
        }
        else if (_direction == Vector2.UnitY)
        {
            projectileStartPos = new Vector2(_player.Position.X, _player.Position.Y + _player.Hitbox.Height + _extraProjectileOffset);
        }
        else
        {
            projectileStartPos = new Vector2(_player.Position.X, _player.Position.Y - _extraProjectileOffset);
        }
        
        _weapon.Fire(
            projectileStartPos,
            _direction, 
            new ProjectileStats(_player.DamageMultiplierStat, 200.0f * _player.ProjectileSpeedMultiplierStat)
            );

        string spriteAnimationName = _player.GetSkin("Head");
        if (_direction.X > float.Epsilon)
        {
            spriteAnimationName += "ShootingRight";
        }
        else if (_direction.X < -float.Epsilon)
        {
            spriteAnimationName += "ShootingLeft";
        }
        else if (_direction.Y < -float.Epsilon)
        {
            spriteAnimationName += "ShootingUp";
        }
        else
        {
            spriteAnimationName += "ShootingDown";
        }

        _player.HeadSprite = SpriteFactory.Instance.CreateAnimatedSprite(spriteAnimationName, _stateDuration * 0.51f);
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
            _player.HeadSprite?.Update(delta);
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