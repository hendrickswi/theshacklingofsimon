#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies.Config;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Entities.Enemies.States;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Passive_Items;
using TheShacklingOfSimon.Rooms_and_Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public abstract class BaseEnemy : DamageableEntity, IEnemy
{
    public string Name { get; }
    public bool IsBoss { get; }
    public bool MarkedForRemoval { get; private set; }

    public IEnemyState CurrentState { get; private set; }
    public IWeapon Weapon { get; protected set; }

    public float MoveSpeedStat { get; set; }
    public float AttackCooldown { get; set; }
    protected float AttackTimer;
    public float AttackRange { get; set; }
    public float ContactDamage { get; set; }
    public string HurtSFX { get; set; }
    public string DieSFX { get; set; }
    public IItem EnemyDrop { get; set; }

    protected IMovementBehavior _movementBehaviour;

    protected IPathfindingService _pathfindingService;
    protected IPlayer _targetPlayer;

    public event Action<IProjectile> OnProjectileCreated;
    public event Action<IItem, Vector2> OnItemDropped;

    protected BaseEnemy(Vector2 startPosition, IWeapon weapon, string name)
    {
        Name = name;
        var config = ConfigDBEnemy.Configs[name];

        IsBoss = config.IsBoss;

        MaxHealth = config.MaxHealth;
        Health = MaxHealth;

        MoveSpeedStat = config.MoveSpeed;
        AttackCooldown = config.AttackCooldown;
        ContactDamage = config.ContactDamage;
        AttackRange = config.AttackRange;

        EffectStats.Add(StatType.InvulnerabilityDuration, config.InvulnerabilityDuration);

        AttackTimer = 0f;

        HurtSFX = SoundManager.Instance.AddSFX("enemy", "TearImpacts0");
        DieSFX = SoundManager.Instance.AddSFX("enemy", "goodeath0");

        _movementBehaviour = new NoMovementBehaviour();

        EnemyDrop = config.DropItemType switch
        {
            EnemyDropType.None => null,
            EnemyDropType.Health => new HealingItem(this),
            EnemyDropType.Speed => new SpeedItem(this),
            EnemyDropType.Coin => new CoinItem(this),
            EnemyDropType.Key => new KeyItem(this),
            _ => null
        };

        SetWeapon(weapon);
        Reset(startPosition);
    }

    public void SetPathfindingService(IPathfindingService pathfindingService)
    {
        _pathfindingService = pathfindingService;
    }

    public void SetTargetPlayer(IPlayer player)
    {
        _targetPlayer = player;
    }

    public virtual void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, 20, 20);

        Health = MaxHealth;

        CurrentState = new EnemyIdleState(this, Velocity);
        CurrentState.Enter();
    }

    public void SetWeapon(IWeapon weapon)
    {
        if (weapon == null) return;

        Weapon = weapon;
        Weapon.OnProjectileFired += proj => OnProjectileCreated?.Invoke(proj);
    }

    public virtual Vector2 FindTarget()
    {
        if (_targetPlayer == null)
        {
            return Vector2.Zero;
        }

        Vector2 myCenter = new Vector2(
            Hitbox.X + Hitbox.Width / 2f,
            Hitbox.Y + Hitbox.Height / 2f
        );

        Vector2 targetCenter = new Vector2(
            _targetPlayer.Hitbox.X + _targetPlayer.Hitbox.Width / 2f,
            _targetPlayer.Hitbox.Y + _targetPlayer.Hitbox.Height / 2f
        );

        if (_pathfindingService == null)
        {
            Vector2 direct = targetCenter - myCenter;
            if (direct.LengthSquared() < 0.0001f) return Vector2.Zero;
            direct.Normalize();
            return direct;
        }

        Func<ITile, bool> canTraverse = tile => !tile.BlocksGround;

        Func<ITile, float> getTraversalCost = tile =>
        {
            if (tile == null) return 1f;

            if (tile.BlocksGround)
            {
                return float.PositiveInfinity;
            }

            return tile switch
            {
                SpikeTile => 50f,
                FireTile => 70f,
                HoleTile => float.PositiveInfinity,
                _ => 1f
            };
        };

        return _pathfindingService.GetNextDirection(
            myCenter,
            targetCenter,
            canTraverse,
            getTraversalCost
        );
    }

    public virtual void RegisterMovement(float dt, Vector2 targetDirection)
    {
        Vector2 movementInput = _movementBehaviour.GetMovement(this, dt, targetDirection);

        if (movementInput.LengthSquared() > 0.0001f)
            movementInput.Normalize();

        Velocity = movementInput * MoveSpeedStat;
        CurrentState.HandleMovement(movementInput);
    }

    public abstract void RegisterAttack(float dt, Vector2 targetDirection);

    public bool CanAttack()
    {
        return AttackTimer <= 0f;
    }

    public void ResetAttackTimer()
    {
        AttackTimer = AttackCooldown;
    }

    public void UpdateAttackTimer(float dt)
    {
        AttackTimer -= dt;
        if (AttackTimer < 0f)
            AttackTimer = 0f;
    }

    public override void Update(GameTime delta)
    {
        base.Update(delta);

        float dt = (float)delta.ElapsedGameTime.TotalSeconds;

        Vector2 targetDirection = FindTarget();

        RegisterMovement(dt, targetDirection);
        RegisterAttack(dt, targetDirection);

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

    public override bool TakeDamage(int damage, bool bypassInvulnerability = false)
    {
        if (!base.TakeDamage(damage, bypassInvulnerability)) return false;

        CurrentState.HandleDamage(damage);
        return true;
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

    public void MarkForRemoval() => IsActive = false;

    public void SpawnPickup(IItem item, Vector2 position)
    {
        OnItemDropped?.Invoke(item, position);
    }

    public override void OnCollision(IEntity other)
    {
        if (other == null || !IsActive) return;
        other.OnCollision(this);
    }

    public override void OnCollision(IPlayer player)
    {
        if (player == null || !IsActive) return;
        player.TakeDamage((int)ContactDamage);
    }

    public override void OnCollision(IEnemy enemy)
    {
    }

    public override void OnCollision(IProjectile projectile)
    {
    }

    public override void OnCollision(IPickup pickup)
    {
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
}