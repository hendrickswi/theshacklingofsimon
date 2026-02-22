using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyIdleState : IEnemyState
{
    private Enemy _enemy;
    private string _currentAnimation;
    private Vector2 _lookingDirection;

    public EnemyIdleState(Enemy enemy, Vector2 lastDirection)
    {
        _enemy = enemy;
        // Default to looking down
        _lookingDirection = (lastDirection.LengthSquared() < 0.0001f) ? new Vector2(0, 1) : lastDirection;
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

    private void UpdateSprite()
    {
        string newAnimationName = "EnemyIdleDown";
        Vector2 cardinal = GetCardinalDirection(_lookingDirection);
        
        if (cardinal == Vector2.UnitX)
        {
            newAnimationName = "EnemyIdleRight";
        }
        else if (cardinal == -Vector2.UnitX)
        {
            newAnimationName = "EnemyIdleLeft";
        }
        else if (cardinal == -Vector2.UnitY)
        {
            newAnimationName = "EnemyIdleUp";
        }

        if (newAnimationName != _currentAnimation)
        {
            _enemy.Sprite = SpriteFactory.Instance.CreateAnimatedSprite(newAnimationName, 0.2f);
            _currentAnimation = newAnimationName;
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