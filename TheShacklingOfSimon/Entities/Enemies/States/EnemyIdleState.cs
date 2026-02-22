using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Enemies.States;

public class EnemyIdleState : IEnemyState
{
    private IEnemy _enemy;
    private IWeapon _weapon;
    private Vector2 _direction;
    private string _currentAnimation;

    public EnemyIdleState(IEnemy enemy, IWeapon weapon, Vector2 direction)
    {
        _enemy = enemy;
        _weapon = weapon;
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
        _enemy.Sprite.Update(delta);
    }

    public void HandleMovement(Vector2 direction)
    {
        if (direction != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyMovingState(_enemy, direction));
        }
    }

    public void HandleAttack(Vector2 direction, float stateDuration)
    {
        Vector2 cardinal = GetCardinalDirection(direction);
        if (direction != Vector2.Zero)
        {
            _enemy.ChangeState(new EnemyAttackingState(_enemy, _weapon, cardinal, stateDuration));

        }
    }

    private void UpdateSprite()
    {
        string newAnimationName = "EnemyIdleDown";
        Vector2 cardinal = GetCardinalDirection(_direction);
        
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