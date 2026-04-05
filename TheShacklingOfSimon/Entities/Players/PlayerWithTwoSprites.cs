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
    public PlayerTwoStatesManager StatesManager { get; private set; }
    public PlayerTwoSpritesManager SpritesManager { get; private set; }
    public PlayerInputBuffer InputBuffer { get; private set; }
    
    // Renaming for clarity
    IPlayerState IPlayer.CurrentState => StatesManager.Body;
    
    // Renaming for clarity
    [Obsolete("PlayerWithTwoSprites does not use Sprite property. Use BodySprite or HeadSprite instead.", true)]
    public new ISprite Sprite
    {
        get => SpritesManager.Body;
        set => SpritesManager.Body = value;
    }

    public PlayerWithTwoSprites(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override bool TakeDamage(int damage)
    {
        if (!base.TakeDamage(damage)) return false;
        InvulnerabilityTimer = EffectStats[StatType.InvulnerabilityDuration];
      

        StatesManager.HandleDamageInterrupt(Health <= 0);

        return true;
    }

    public void Reset(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override void Update(GameTime delta)
    {
        base.Update(delta);

        // Handle user input
        StatesManager.Body.HandleMovement(InputBuffer.ConsumeMovement(), SpritesManager.MovementFrameDuration);
        StatesManager.Head.HandlePrimaryAttack(InputBuffer.ConsumePrimaryAttack(), Inventory.CurrentPrimaryWeapon.BaseCooldown + EffectStats[StatType.PrimaryCooldown]);
        StatesManager.Head.HandleSecondaryAttack(InputBuffer.ConsumeSecondaryAttack(), Inventory.CurrentSecondaryWeapon.BaseCooldown + EffectStats[StatType.SecondaryCooldown]);
        
        // Update position and velocity
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

        // Update states and sprites
        StatesManager.Update(delta);
        SpritesManager.Update(delta);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        SpritesManager.Draw(spriteBatch);
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
    
    // Passthrough for potential skin modification by IItem-implementing classes
    public string GetSkin(string category)
    {
        
        return SpritesManager.GetSkin(category);
    }
    
    // Passthrough for potential skin modification by IItem-implementing classes
    public void SetSkin(string category, string skinPrefix)
    {
        SpritesManager.SetSkin(category, skinPrefix);
    }

    // More explicit interface implementation for renaming purposes
    void IPlayer.ChangeState(IPlayerState newState)
    {
        switch (newState)
        {
            case IPlayerHeadState:
            {
                StatesManager.ChangeHeadState((IPlayerHeadState)newState);
                break;
            }

            case IPlayerBodyState:
            {
                StatesManager.ChangeBodyState((IPlayerBodyState)newState);
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
        Inventory = new PlayerInventory();
        SpritesManager = new PlayerTwoSpritesManager(this);
        StatesManager = new PlayerTwoStatesManager(this, SpritesManager);
    }
}
