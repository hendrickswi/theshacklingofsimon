#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Players.States.Head;

public class PlayerHeadAttackingState : IPlayerHeadState
{
    private readonly PlayerWithTwoSprites _player;
    private readonly IWeapon _weapon;
    private readonly Vector2 _direction;
    private float _timer;
    private readonly float _stateDuration;

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
        float projectilePositionX = _player.Hitbox.X + _player.Hitbox.Width / 2.0f;
        float projectilePositionY = _player.Hitbox.Y;

        IProjectile projectile = _weapon.GetPrototype();
        _weapon.Fire(
            new Vector2(projectilePositionX, projectilePositionY),
            _direction, 
            new ProjectileStats(
                projectile.Stats.Damage * (int) Math.Ceiling(_player.GetStat(StatType.DamageMultiplier)), 
                projectile.Stats.Speed * _player.GetStat(StatType.ProjectileSpeedMultiplier), 
                ProjectileOwner.Player)
            );

        string spriteAnimationName = _player.SpritesManager.GetSkin("Head");
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
        
        if (_weapon.SFX != null)
        {
            SoundManager.Instance.PlaySFX(_weapon.SFX);   
        }

        _player.SpritesManager.Head = SpriteFactory.Instance.CreateAnimatedSprite(spriteAnimationName, _stateDuration * 0.51f);
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
            _player.StatesManager.ChangeHeadState(new PlayerHeadIdleState(_player, new Vector2(0, 1)));
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