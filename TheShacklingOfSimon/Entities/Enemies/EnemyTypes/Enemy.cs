using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies.States;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class Enemy : DamageableEntity, IEnemy
{    
    public IEnemyState CurrentState { get; private set; }

    public float MoveSpeedStat { get; set; }
    public float AttackDamage{ get; set; }
    public float AttackCooldown { get; set; }
    public float AttackRange { get; set; }

    private Vector2 _movementInput;
    private Vector2 _attack;

    public Enemy(Vector2 startPosition)
    {
        // IEntity properties
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        // Arbitrarily sized hitbox of 20x20
        Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, 20, 20);
        
        // IDamageable properties
        this.Health = 3;
        this.MaxHealth = 3;
        
        // Enemy properties
        
        // These can all be overriden with public set method
        this.MoveSpeedStat = 20.0f;
        this.AttackCooldown = 0.2f;
        this.AttackRange = 50.0f;
        this.AttackDamage = 1.0f;
        
        this.CurrentState = new EnemyIdleState(this, Velocity);
        this.CurrentState.Enter();
        this._movementInput = Vector2.Zero;
        this._attack = Vector2.Zero;
    }

    public Vector2 Pathfind(Vector2 targetPosition)
    {
        // Simple pathfinding logic: move directly towards the target
        Vector2 direction = targetPosition - Position;
        if (direction.LengthSquared() > 0.0001f)
        {
            direction.Normalize();
        }
        return direction * MoveSpeedStat;
    }

    public void RegisterAttack(Vector2 direction)
    {
        _attack += direction;
    }

    public override void Update(GameTime delta)
    {
        // Movement logic
        if (_movementInput.LengthSquared() > 0.0001f)
        {
            _movementInput.Normalize();
        }
        CurrentState.HandleMovement(_movementInput);
        _movementInput = Vector2.Zero;
        
        // Attack logic
        if (_attack.LengthSquared() > 0.0001f)
        {
            CurrentState.HandleAttack(_attack, AttackDamage, AttackCooldown, AttackRange);
        }

        _attack = Vector2.Zero;
        
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 20, 20);
        
        CurrentState.Update(delta);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        SpriteEffects flip = SpriteEffects.None;
        if (Velocity.X < -0.0001f)
        {
            flip = SpriteEffects.FlipHorizontally;
        }

        if (Sprite != null)
        {
            Sprite.Draw(spriteBatch, Position, Color.White, 0.0f,
                        new Vector2(0, 0), 1.0f, flip, 0.0f);
        }
        
    }

    public void ChangeState(IEnemyState newState)
    {
        if (CurrentState != newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}