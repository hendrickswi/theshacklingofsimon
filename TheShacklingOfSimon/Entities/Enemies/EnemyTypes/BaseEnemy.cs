#region

using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies.Config;
using TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours.MovementBehaviours;
using TheShacklingOfSimon.Entities.Enemies.States;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Templates;
using TheShacklingOfSimon.Items.Passive_Items.Consumables;
using TheShacklingOfSimon.Items.Passive_Items.Inventory_Items;
using TheShacklingOfSimon.Items.Passive_Items.Projectile_Augmentation_Items;
using TheShacklingOfSimon.Rooms_and_Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

#endregion

namespace TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

public abstract class BaseEnemy : DamageableEntity, IEnemy
{
    private const float WaypointReachDistance = 4f;

    public string Name { get; }
    public bool IsBoss { get; }
    public bool MarkedForRemoval { get; private set; }

    public IEnemyState CurrentState { get; private set; }
    public IWeapon Weapon { get; protected set; }
    public bool HitboxEnabled { get; set; } = true;

    public float MoveSpeedStat { get; set; }
    public float AttackCooldown { get; set; }
    protected float AttackTimer;
    public float AttackRange { get; set; }
    public float ContactDamage { get; set; }
    public string HurtSFX { get; set; }
    public string DieSFX { get; set; }
    public IItem EnemyDrop { get; set; }

    protected IMovementBehavior _movementBehaviour;
    private static readonly Random _rng = new();
    private readonly IReadOnlyList<EnemyDropType> _dropPool;

    protected IPathfindingService _pathfindingService;
    protected IPlayer _targetPlayer;

    private Vector2? _currentPathWaypoint;

    public event Action<IProjectile> OnProjectileCreated;
    public event Action<IItem, Vector2> OnItemDropped;
    public event Action<IEnemy> OnEnemySpawned;

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

        EffectManager.ClearAllEffects();
        EffectStats.Clear();
        EffectStats.Add(StatType.InvulnerabilityDuration, config.InvulnerabilityDuration);

        AttackTimer = 0f;

        HurtSFX = SoundManager.Instance.AddSFX("enemy", "TearImpacts0");
        DieSFX = SoundManager.Instance.AddSFX("enemy", "goodeath0");

        _movementBehaviour = new NoMovementBehaviour();

        IReadOnlyList<EnemyDropType> initialDropPool = config.DropItemPool?.Length > 0
            ? config.DropItemPool
            : new[] { config.DropItemType };

        _dropPool = Array.AsReadOnly(new List<EnemyDropType>(initialDropPool).ToArray());
        EnemyDrop = null;

        SetWeapon(weapon);
        Reset(startPosition);
    }

    public IEnumerable<IStatusEffect> GetActiveEffects()
    {
        return EffectManager.ActiveEffects;
    }

    public void SetPathfindingService(IPathfindingService pathfindingService)
    {
        _pathfindingService = pathfindingService;
        _currentPathWaypoint = null;
    }

    public void SetTargetPlayer(IPlayer player)
    {
        _targetPlayer = player;
        _currentPathWaypoint = null;
    }

    public void CenterOnWorldPoint(Vector2 center)
    {
        Vector2 dimensions = GetSpriteDimensions();

        Position = new Vector2(
            center.X - dimensions.X / 2f,
            center.Y - dimensions.Y / 2f
        );

        Velocity = Vector2.Zero;
        Hitbox = GetSpriteHitbox(Position);
        _currentPathWaypoint = null;
    }

    public virtual void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        HitboxEnabled = true;
        _currentPathWaypoint = null;

        Health = MaxHealth;

        CurrentState = new EnemyIdleState(this, Velocity);
        CurrentState.Enter();

        Hitbox = GetSpriteHitbox(Position);
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
            _currentPathWaypoint = null;
            return Vector2.Zero;
        }

        Vector2 myCenter = GetHitboxCenter(Hitbox);
        Vector2 targetCenter = GetHitboxCenter(_targetPlayer.Hitbox);

        if (_pathfindingService is not GridPathfindingService gridPathfindingService)
        {
            return GetDirectDirection(myCenter, targetCenter);
        }

        if (_currentPathWaypoint.HasValue)
        {
            float distanceSquared = Vector2.DistanceSquared(myCenter, _currentPathWaypoint.Value);

            if (distanceSquared <= WaypointReachDistance * WaypointReachDistance)
            {
                _currentPathWaypoint = null;
            }
        }

        if (!_currentPathWaypoint.HasValue)
        {
            bool foundWaypoint = gridPathfindingService.TryGetNextWaypoint(
                myCenter,
                targetCenter,
                CanTraverseTile,
                GetTraversalCost,
                out Vector2 nextWaypoint
            );

            if (!foundWaypoint)
            {
                return Vector2.Zero;
            }

            _currentPathWaypoint = nextWaypoint;
        }

        return GetDirectDirection(myCenter, _currentPathWaypoint.Value);
    }

    protected static Vector2 GetDirectDirection(Vector2 currentCenter, Vector2 targetCenter)
    {
        Vector2 direction = targetCenter - currentCenter;

        if (direction.LengthSquared() < 0.0001f)
        {
            return Vector2.Zero;
        }

        direction.Normalize();
        return direction;
    }

    protected static Vector2 GetHitboxCenter(Rectangle hitbox)
    {
        return new Vector2(
            hitbox.X + hitbox.Width / 2f,
            hitbox.Y + hitbox.Height / 2f
        );
    }

    protected virtual bool CanTraverseTile(ITile tile)
    {
        if (tile == null) return true;

        if (tile.BlocksGround) return false;

        // These tiles are unsafe for normal ground enemies.
        if (tile is FireTile) return false;
        if (tile is SpikeTile) return false;
        if (tile is HoleTile) return false;

        return true;
    }

    protected virtual float GetTraversalCost(ITile tile)
    {
        if (tile == null) return 1f;

        if (!CanTraverseTile(tile))
        {
            return float.PositiveInfinity;
        }

        return 1f;
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

        MoveSafely(dt);

        if (HitboxEnabled)
        {
            Hitbox = GetSpriteHitbox(Position);
        }
        else
        {
            Hitbox = Rectangle.Empty;
        }

        CurrentState.Update(delta);
    }

    private void MoveSafely(float dt)
    {
        if (Velocity.LengthSquared() < 0.0001f)
        {
            return;
        }

        Vector2 move = Velocity * dt;

        Vector2 desiredPosition = Position + move;
        if (CanSafelyOccupy(desiredPosition))
        {
            Position = desiredPosition;
            return;
        }

        Vector2 xOnlyPosition = Position + new Vector2(move.X, 0f);
        if (CanSafelyOccupy(xOnlyPosition))
        {
            Position = xOnlyPosition;
            Velocity = new Vector2(Velocity.X, 0f);
            return;
        }

        Vector2 yOnlyPosition = Position + new Vector2(0f, move.Y);
        if (CanSafelyOccupy(yOnlyPosition))
        {
            Position = yOnlyPosition;
            Velocity = new Vector2(0f, Velocity.Y);
            return;
        }

        Velocity = Vector2.Zero;
        _currentPathWaypoint = null;
    }

    private bool CanSafelyOccupy(Vector2 candidatePosition)
    {
        if (_pathfindingService is not GridPathfindingService gridPathfindingService)
        {
            return true;
        }

        Rectangle candidateHitbox = GetSpriteHitbox(candidatePosition);

        return gridPathfindingService.IsAreaSafe(candidateHitbox, CanTraverseTile);
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

    protected virtual Point GetGameplayHitboxSize()
    {
        return new Point(26, 26);
    }

    private Rectangle GetSpriteHitbox(Vector2 position)
    {
        Vector2 dimensions = GetSpriteDimensions();

        int spriteWidth = Math.Max(1, (int)Math.Ceiling(dimensions.X));
        int spriteHeight = Math.Max(1, (int)Math.Ceiling(dimensions.Y));

        Point desiredSize = GetGameplayHitboxSize();

        int width = Math.Min(desiredSize.X, spriteWidth);
        int height = Math.Min(desiredSize.Y, spriteHeight);

        int x = (int)MathF.Round(position.X + (spriteWidth - width) / 2f);
        int y = (int)MathF.Round(position.Y + (spriteHeight - height) / 2f);

        return new Rectangle(x, y, width, height);
    }

    private Vector2 GetSpriteDimensions()
    {
        if (Sprite == null)
            return new Vector2(20, 20);

        return Sprite.RemoveDecorator().GetDimensions();
    }

    public void MarkForRemoval() => IsActive = false;

    public void SpawnPickup(IItem item, Vector2 position)
    {
        OnItemDropped?.Invoke(item, position);
    }

    public IItem CreateDropItem()
    {
        if (_dropPool == null || _dropPool.Count == 0)
        {
            return null;
        }

        EnemyDropType chosenDrop = _dropPool[_rng.Next(_dropPool.Count)];
        return chosenDrop switch
        {
            EnemyDropType.Health => new HealingItem(this),
            EnemyDropType.Speed => new SpeedItem(this),
            EnemyDropType.Coin => new CoinItem(this),
            EnemyDropType.Key => new KeyItem(this),
            EnemyDropType.Armor => new ArmorItem(this),
            EnemyDropType.Damage => new DamageItem(this),
            //EnemyDropType.ProjectileAugment => new ProjectileAugmentItem(this),
            _ => null
        };
    }

    public void SpawnEnemy(IEnemy enemy)
    {
        OnEnemySpawned?.Invoke(enemy);
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
        Hitbox = GetSpriteHitbox(Position);
        _currentPathWaypoint = null;

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