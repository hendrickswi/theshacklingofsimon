using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyMovingState : IEnemyState
{
    private IEnemy _enemy;
    private string _currentAnimation;
    private SpriteEffects _effects = SpriteEffects.None;
    private Vector2 _direction;

    public EnemyMovingState(IEnemy enemy, Vector2 lastDirection)
    {
        _enemy = enemy;
        // Default to looking down
        _direction = (lastDirection.LengthSquared() < 0.0001f) ? new Vector2(0, 1) : lastDirection;
        _enemy.Velocity = _direction * _enemy.MoveSpeedStat;
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
        _enemy.Sprite?.Update(delta);
    }

    public void HandleMovement(Vector2 direction)
    {
        if (direction.LengthSquared() < 0.0001f)
        {
            _enemy.Velocity = Vector2.Zero;
            _enemy.ChangeState(new EnemyIdleState(_enemy, direction));
        }
        else
        {
            _enemy.Velocity = direction * _enemy.MoveSpeedStat;
            UpdateSprite();
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

    private void UpdateSprite()
    {
        string newAnimationName = _enemy.Name + "_EnemyWalkRight";
        _effects = SpriteEffects.None;
        
        /*
         * Walking animation is horizontally biased.
         * e.g., If walking northeast (both up and right),
         * the horizontal walk animation is played.
         */
        /*
        * Walking animation is horizontally biased.
        * If moving left, we flip the right-walk animation.
        */
        if (_enemy.Velocity.X < 0)
        {
            newAnimationName = _enemy.Name + "_EnemyWalkRight";
            _effects = SpriteEffects.FlipHorizontally;
        }
        /* for testing
        else if (_enemy.Velocity.Y < 0)
        {
            newAnimationName = _enemy.Name + "_EnemyWalkUp";
        }
        else if (_enemy.Velocity.Y > 0)
        {
            newAnimationName = _enemy.Name + "_EnemyWalkDown";
        }
        */

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