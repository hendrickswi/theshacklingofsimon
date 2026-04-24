#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyIdleState : IEnemyState
{
    private IEnemy _enemy;
    private IWeapon _weapon;
    private Vector2 _direction;
    private string _currentAnimation;

    public EnemyIdleState(IEnemy enemy, Vector2 direction)
    {
        _enemy = enemy;
        _weapon = _enemy.Weapon;
        // Default to looking down
        _direction = (direction.LengthSquared() < 0.0001f) ? new Vector2(0, 1) : direction;
    }
    public void Enter()
    {
        UpdateSprite();
    }
    
    public void Exit()
    {
        // Reset any idle-specific state if necessary
    }

    public void Update(GameTime delta)
    {
        UpdateSprite();
    }

    public void HandleMovement(Vector2 direction)
    {
        if (direction.LengthSquared() > 0.0001f)
        {
            _enemy.ChangeState(new EnemyMovingState(_enemy, direction));
        }
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        Vector2 cardinal = GetCardinalDirection(direction);
        if (direction != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyAttackingState(_enemy, cardinal, stateDuration));
        }
    }

    public void HandleDamage(int damage)
    {
        if (_enemy.Health <= 0)
        {
            _enemy.ChangeState(new EnemyDeadState(_enemy, 0.5f));
        }
        else
        {
            _enemy.ChangeState(new EnemyDamagedState(_enemy, 0.2f));
        }
    }

    private void UpdateSprite()
    {
        string newAnimationName = _enemy.Name + "_Idle";
        
        _enemy.HitboxEnabled = true;
        _enemy.Sprite = SpriteFactory.Instance.CreateStaticSprite(newAnimationName);
        _currentAnimation = newAnimationName;
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