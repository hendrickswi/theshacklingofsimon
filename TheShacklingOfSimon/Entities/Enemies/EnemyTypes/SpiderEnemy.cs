using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies.States;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class SpiderEnemy : DamageableEntity, IEnemy
{    

    public IEnemyState CurrentState { get; private set; }
    public IWeapon Weapon { get; private set; }

    public float MoveSpeedStat { get; set; }
    public float AttackCooldown { get; set; }
    private float _attackTimer;
    public float AttackRange { get; set; }

    private Vector2 _movementInput;
    private Vector2 _attack;

    // Wander variables
    private static Random _rng = new Random();
    private float _wanderTimer;
    private float _wanderInterval = 1.5f;
    private Vector2 _wanderDirection;

    public SpiderEnemy(Vector2 startPosition)
    {   
        // IDamageable properties
        this.Health = 3;
        this.MaxHealth = 3;
        
        // Enemy properties
        
        // These can all be overriden with public set method
        this.MoveSpeedStat = 17.0f;
        this.AttackCooldown = 3000.0f;
        _attackTimer = 0f;
        this.AttackRange = 50.0f;
        this.Weapon = new BasicWeapon(new Projectiles.ProjectileManager());

        this.Sprite = SpriteFactory.Instance.CreateStaticSprite("EnemyIdleDown");
        
        Reset(startPosition);
    }

    public void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, 20, 20);
        Health = MaxHealth;
        this.CurrentState = new EnemyIdleState(this, Velocity);
        this.CurrentState.Enter();
        this._movementInput = Vector2.Zero;
        this._attack = Vector2.Zero;
    }

    public Vector2 FindTarget() //this method will return (0,0) if no target found
    {
        // Placeholder for target finding logic, e.g., find the player or other entities
        return Position + new Vector2(100, 0); // This will be replaced with actual target finding logic in a real implementation
    }

    private void Wander(float dt)
    {
        _wanderTimer -= dt;

        if (_wanderTimer <= 0f)
        {
            float angle = (float)(_rng.NextDouble() * Math.PI * 2);

            _wanderDirection = new Vector2(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle)
            );

            _wanderTimer = _wanderInterval;
        }

        _movementInput = _wanderDirection;
    }

    public void Pathfind(Vector2 targetPosition)
    {
        //pre-check if target is found
        if (targetPosition == Vector2.Zero)
        {
            _movementInput = Vector2.Zero;
            return;
        }

        // Simple pathfinding logic: move directly towards the target
        Vector2 direction = targetPosition - Position;
        if (direction.LengthSquared() > 0.0001f)
        {
            direction.Normalize();
        }
        _movementInput += direction * MoveSpeedStat;
    }

    public void RegisterAttack(float dt, Vector2 targetDirection)
    {
        /*
        if (targetDirection == Vector2.Zero)
        {
            _attack = Vector2.Zero;
            return;
        }
        Vector2 direction = targetDirection - Position;
        if (direction.LengthSquared() > 0.0001f)
        {
            direction.Normalize();
        }
        _attack += direction;
        */
        // Attack logic
        _attackTimer -= dt;
        if (_attackTimer < 0f)
            _attackTimer = 0f;
        if (_attackTimer <= 0f)
        {
            CurrentState.HandleAttack(Velocity, AttackCooldown);
            _attackTimer = AttackCooldown; // reset cooldown
        }
    }

    public void RegisterMovement(float dt, Vector2 targetPosition)
    {
        // Movement logic
        //Pathfind(targetPosition);
        //_movementInput = new Vector2(0.5f, 0f); // for testing
        Wander(dt);
        if (_movementInput.LengthSquared() > 0.0001f)
        {
            _movementInput.Normalize();
        }
        Velocity = _movementInput * MoveSpeedStat; // <-- apply velocity
        CurrentState.HandleMovement(_movementInput);
        _movementInput = Vector2.Zero;
    }

    public override void Update(GameTime delta)
    {
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        // Find target
        //Vector2 targetPosition = FindTarget();
        RegisterMovement(dt, new Vector2(0f, 0f)); // No specific target for now, just wander
        RegisterAttack(dt, Position + Velocity); // Attack in the direction of movement for testing
        /*
        if (targetPosition.LengthSquared() > 0.0001f)
        {
            RegisterAttack(targetPosition);
            CurrentState.HandleAttack(_attack, AttackCooldown);
        }
        */

        _attack = Vector2.Zero;
        
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
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }
    }
}