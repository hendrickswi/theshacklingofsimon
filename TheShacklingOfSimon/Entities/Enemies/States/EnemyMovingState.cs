using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyMovingState : IEnemyState
{
    private IEnemy _enemy;
    private IWeapon _weapon;
    private Vector2 _direction;
    private string _currentAnimation;
    private Vector2 _lookingDirection;

    public EnemyMovingState(IEnemy enemy, Vector2 lastDirection)
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
        // no-op
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        Vector2 cardinal = GetCardinalDirection(direction);
        if (direction != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyAttackingState(_enemy, _weapon, direction, stateDuration));
        }
    }

    private void UpdateSprite()
    {
        string newAnimationName = "EnemyWalkVertical";
        
        /*
         * Walking animation is horizontally biased.
         * e.g., If walking northeast (both up and right),
         * the horizontal walk animation is played.
         */
        if (_enemy.Velocity.X > 0)
        {
            newAnimationName = "EnemyWalkRight";
        }
        else if (_enemy.Velocity.X < 0)
        {
            newAnimationName = "EnemyWalkLeft";
        }
        else if (_enemy.Velocity.Y < 0)
        {
            newAnimationName = "EnemyWalkUp";
        }
        else if (_enemy.Velocity.Y > 0)
        {
            newAnimationName = "EnemyWalkDown";
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