#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players.Config;
using TheShacklingOfSimon.Entities.Players.Drawing;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Entities.Players.States.Body;
using TheShacklingOfSimon.Entities.Players.States.Head;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerWithTwoSprites : DamageableEntity, IPlayer, ITargetProvider
{
    public PlayerInventory Inventory { get; private set; }
    public PlayerTwoStateManager StateManager { get; set; }

    // Renaming for clarity
    IPlayerState IPlayer.CurrentState => StateManager.Body;
    
    public ISprite HeadSprite { get; set; }
    public ISprite BodySprite { get; set; }
    
    // Renaming for clarity
    [Obsolete("PlayerWithTwoSprites does not use Sprite property. Use BodySprite or HeadSprite instead.", true)]
    public new ISprite Sprite
    {
        get => BodySprite;
        set => BodySprite = value;
    }
    
    public PlayerInputBuffer InputBuffer { get; private set; }

    private PlayerWithTwoSpritesDrawManager _drawManager;

    public PlayerWithTwoSprites(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        InvulnerabilityTimer = EffectStats[StatType.InvulnerabilityDuration];
        
        StateManager.HandleDamageInterrupt(Health <= 0);
    }

    public void Reset(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override void Update(GameTime delta)
    {
        base.Update(delta);
        
        StateManager.Body.HandleMovement(InputBuffer.ConsumeMovement(), _drawManager.MovementFrameDuration);
        StateManager.Head.HandlePrimaryAttack(InputBuffer.ConsumePrimaryAttack(), EffectStats[StatType.PrimaryCooldown]);
        StateManager.Head.HandleSecondaryAttack(InputBuffer.ConsumeSecondaryAttack(), EffectStats[StatType.SecondaryCooldown]);

        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

        StateManager.Update(delta);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        SpriteEffects flip = SpriteEffects.None;
        if (Velocity.X < -0.0001f)
        {
            flip = SpriteEffects.FlipHorizontally;
        }
        
        Vector2 drawPos = (StateManager.Body is PlayerBodyDamagedState) ? Position + _drawManager.DamagedStateOffset : Position;
        BodySprite?.Draw(spriteBatch, drawPos, Color.White, 0.0f,
            new Vector2(0, 0), 1.0f, flip, 0.0f);
        
        HeadSprite?.Draw(spriteBatch, Position + _drawManager.HeadOffset, Color.White);
    }

    public override void OnCollision(IEntity other)
    {
        if (other == null || !IsActive) return;
        other.OnCollision(this);
    }
    
    public override void OnCollision(IPlayer otherPlayer) { }
    public override void OnCollision(IEnemy enemy) { }
    public override void OnCollision(IProjectile projectile) { }
    public override void OnCollision(ITile tile) { }
    public override void OnCollision(IPickup pickup) { }
    
    // For ITargetProvider
    public Vector2 GetPosition()
    {
        return Position;
    }
    
    public string GetSkin(string category)
    {
        // Passthrough
        return _drawManager.GetSkin(category);
    }
    
    public void SetSkin(string category, string skinPrefix)
    {
        // Passthrough
        _drawManager.SetSkin(category, skinPrefix);
    }

    // More explicit interface implementation for renaming purposes
    void IPlayer.ChangeState(IPlayerState newState)
    {
        switch (newState)
        {
            case IPlayerHeadState:
            {
                StateManager.ChangeHeadState((IPlayerHeadState)newState);
                break;
            }

            case IPlayerBodyState:
            {
                StateManager.ChangeBodyState((IPlayerBodyState)newState);
                break;
            }
            default:
            {
                throw new ArgumentException("newState must be of type IPlayerHeadState, IPlayerBodyState.");
            }
        }
    }

    private void Initialize(Vector2 startPosition)
    {
        /*
         * Constructor logic moved here so Reset() can invoke
         * constructor logic without duplicating code
         */
        var config = ConfigDBPlayer.Configs["PlayerWithTwoSprites"];
        
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, config.HitboxWidth, config.HitboxHeight);
        Health = config.MaxHealth;
        MaxHealth = config.MaxHealth;
        
        EffectStats.Clear();
        EffectStats.Add(StatType.MaxHealth, config.MaxHealth);
        EffectStats.Add(StatType.InvulnerabilityDuration, config.InvulnerabilityDuration);
        EffectStats.Add(StatType.MoveSpeed, config.MoveSpeed);
        EffectStats.Add(StatType.DamageMultiplier, config.DamageMultiplier);
        EffectStats.Add(StatType.ProjectileSpeedMultiplier, config.ProjectileSpeedMultiplier);
        EffectStats.Add(StatType.PrimaryCooldown, config.PrimaryCooldown);
        EffectStats.Add(StatType.SecondaryCooldown, config.SecondaryCooldown);
        
        InputBuffer = new PlayerInputBuffer();
        _drawManager = new PlayerWithTwoSpritesDrawManager();
        Inventory = new PlayerInventory();
        StateManager = new PlayerTwoStateManager(this, _drawManager);
    }
}
