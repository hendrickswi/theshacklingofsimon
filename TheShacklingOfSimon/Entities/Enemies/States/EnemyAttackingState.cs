using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyAttackingState : IEnemyState
{
    private Enemy _enemy;
    private Vector2 _direction;

    public EnemyAttackingState(Enemy enemy, Vector2 attackInput, float attackDamage, float attackCooldown, float attackRange)
    {
        _enemy = enemy;
        // Default to looking down
        _direction = (attackInput.LengthSquared() < 0.0001f) ? new Vector2(0, 1) : attackInput;
    }
    public void Enter()
    {
        // _direction is already "cardinalized" from PlayerHeadIdleState

        // could implement as weapons not being specialized to one enemy
        //_weapon.Fire(_player.Position, _direction, new ProjectileStats(1.0f * _player.DamageMultiplierStat, 20.0f));

        string spriteAnimationName = "EnemyAttack";

        _enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(spriteAnimationName, 0.2f);
    }
    
    public void Exit()
    {
        // Reset any idle-specific state if necessary
    }

    public void Update(GameTime delta)
    {
        _enemy.Sprite.Update(delta);
    }

    public void HandleMovement(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyMovingState(_enemy, direction));
        }
    }

    public void HandleAttack(Vector2 attackInput, float attackDamage, float attackCooldown, float attackRange)
    {
        Vector2 cardinal = GetCardinalDirection(attackInput);
        if (attackInput != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyAttackingState(_enemy, cardinal, attackDamage, attackCooldown, attackRange));
        }
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