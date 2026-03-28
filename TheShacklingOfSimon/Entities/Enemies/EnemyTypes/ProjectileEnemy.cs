using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies.States;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public class ProjectileEnemy : DamageableEntity, IEnemy
{    

    public string Name { get; }
    public bool MarkedForRemoval { get; private set; }

    public IEnemyState CurrentState { get; private set; }
    public IWeapon Weapon { get; private set; }

    public float MoveSpeedStat { get; set; }
    public float AttackCooldown { get; set; }
    private float _attackTimer;
    public float AttackRange { get; set; }
    public float ContactDamage { get; set; }

    private EnemyMovementManager _movement;
    private Vector2 _movementInput;
    private Vector2 _attack;
    public event Action<IProjectile> OnProjectileCreated;

    public ProjectileEnemy(Vector2 startPosition, IWeapon weapon, string name = "SpiderEnemy")
    {   
        this.Name = name;
        var config = ConfigDB.Configs[name];
        
        // IDamageable properties
        this.MaxHealth = config.MaxHealth;
        this.Health = MaxHealth;
        
        // Movement class properties
        _movement = new EnemyMovementManager();
        
        // These can all be overriden with public set method
        this.MoveSpeedStat = config.MoveSpeed;
        this.AttackCooldown = config.AttackCooldown;
        this.ContactDamage = config.ContactDamage;
        this.AttackRange = config.AttackRange;
        _attackTimer = 0f;

        SetWeapon(weapon);
        
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
    
    public void SetWeapon(IWeapon weapon)
    {
        if (weapon == null) return;

        Weapon = weapon;
        Weapon.OnProjectileFired += proj => OnProjectileCreated?.Invoke(proj);
    }

    public void MarkForRemoval()
    {
        IsActive = false;
    }

    public Vector2 FindTarget() //this method will return (0,0) if no target found
    {
        // Placeholder for target finding logic, e.g., find the player or other entities
        Vector2 targetPosition = Position + new Vector2(25, 0);
        Vector2 targetDirection = targetPosition - Position;
        return targetDirection;
    }

    public void RegisterAttack(float dt, Vector2 targetDirection)
    {
        /*
        if (targetDirection.LengthSquared() > AttackRange)
        {
            _attack = Vector2.Zero;
        }
        else
        {
            Vector2 direction = targetDirection;

            if (direction.LengthSquared() > 0.0001f)
                direction.Normalize();

            _attack = direction;
        }
        */
        //temp so attacks go through, though theres no target
        Vector2 direction = Velocity;
        _attack = direction;

        // Attack cooldown logic
        _attackTimer -= dt;
        if (_attackTimer < 0f)
            _attackTimer = 0f;

        if (_attackTimer <= 0f && _attack != Vector2.Zero)
        {
            CurrentState.HandleAttack(_attack, AttackCooldown);
            _attackTimer = AttackCooldown;
        }
        _attack = Vector2.Zero;
    }

    public void StandardMovement(float dt, Vector2 targetDirection)
    {
        if (targetDirection.LengthSquared() > AttackRange)
            _movementInput = _movement.Wander(dt);
        else
            _movementInput = _movement.Pathfind(targetDirection);
    }

    public void RegisterMovement(float dt, Vector2 targetDirection)
    {
        // Movement logic
        _movementInput = Vector2.Zero;
        StandardMovement(dt, targetDirection);
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
        Vector2 targetDirection = FindTarget();
        RegisterMovement(dt, targetDirection); // No specific target for now, just wander
        RegisterAttack(dt, targetDirection); // Attack in the direction of movement for testing
        
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

    public override void OnCollision(IEntity other)
    {
        if (other == null || !IsActive) return;
        other.OnCollision(this);
    }

    public override void OnCollision(IPlayer player)
    {
        player.TakeDamage(1);
        // TODO: Should change state here
    }

    public override void OnCollision(IEnemy enemy)
    {
        // No-op for now (avoid enemies pushing each other until desired).
    }

    public override void OnCollision(IProjectile projectile)
    {
        // No-op, let the projectile deal with the interaction.
    }

    public override void OnCollision(ITile tile)
    {
        if (tile == null || !tile.BlocksGround) return;

        Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(Hitbox, tile.Hitbox);
        if (mtv == Vector2.Zero) return;

        Position += mtv;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

        switch (CollisionDetector.GetCollisionSideFromMtv(mtv))
        {
            case CollisionSide.Left:
            case CollisionSide.Right:
                Velocity = new Vector2(0.0f, Velocity.Y);
                break;

            case CollisionSide.Top:
            case CollisionSide.Bottom:
                Velocity = new Vector2(Velocity.X, 0.0f);
                break;
        }
    }

    public override void OnCollision(IPickup pickup)
    {
        // No-op
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