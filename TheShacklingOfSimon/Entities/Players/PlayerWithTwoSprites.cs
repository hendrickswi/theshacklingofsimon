using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Entities.Players.States.Body;
using TheShacklingOfSimon.Entities.Players.States.Head;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerWithTwoSprites : DamageableEntity, IPlayer, ITargetProvider
{
    public PlayerInventory Inventory { get; private set; }

    private IPlayerHeadState CurrentHeadState { get; set; }
    private IPlayerBodyState CurrentBodyState { get; set; }

    // Use explicit interface implementation 
    IPlayerState IPlayer.CurrentState => CurrentBodyState;

    /*
     * Additional sprite to handle the head.
     * Allows non-combinatorial states
     *      i.e., head can be in shooting state and body can
     *      be in moving state, but they do so independently
     *      as opposed to a ShootingMoving combined state.
     */
    public ISprite HeadSprite { get; set; }
    public ISprite BodySprite { get; set; }

    // Use explicit interface implementation 
    [Obsolete("PlayerWithTwoSprites does not use Sprite property. Use BodySprite or HeadSprite instead.", true)]
    public new ISprite Sprite
    {
        get => BodySprite;
        set => BodySprite = value;
    }

    // The miscellaneous stats
    public PlayerStats Stats { get; private set; }
    public PlayerInputBuffer InputBuffer { get; private set; }
    
    private readonly Vector2 _headOffset = new Vector2(-4.75f, -16);
    private readonly Vector2 _damagedStateOffset = new Vector2(0, -5);
    
    private Dictionary<string, string> _skins;
    

    public PlayerWithTwoSprites(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override void TakeDamage(int damage)
    {
        // No-op if the player is already in the damaged state
        if (CurrentBodyState is PlayerBodyDamagedState)
        {
            return;
        }

        base.TakeDamage(damage);

        // Case for player dying
        if (Health <= 0)
        {
            ChangeHeadState(new PlayerHeadDeadState(this));
            ChangeBodyState(new PlayerBodyDeadState(this, Stats.DeathFrameDuration, Stats.DeathFrameDuration));
        }
        // If not dead, then damaged
        else
        {
            ChangeHeadState(new PlayerHeadDamagedState(this));
            ChangeBodyState(new PlayerBodyDamagedState(this, Stats.InvulnerabilityDuration));
        }
    }

    public void Reset(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override void Update(GameTime delta)
    {
        CurrentBodyState.HandleMovement(InputBuffer.ConsumeMovement(), Stats.MovementFrameDuration);
        CurrentHeadState.HandlePrimaryAttack(InputBuffer.ConsumePrimaryAttack(), Stats.PrimaryAttackCooldown);
        CurrentHeadState.HandleSecondaryAttack(InputBuffer.ConsumeSecondaryAttack(), Stats.SecondaryAttackCooldown);

        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Hitbox.Width, Hitbox.Height);

        CurrentHeadState.Update(delta);
        CurrentBodyState.Update(delta);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        SpriteEffects flip = SpriteEffects.None;
        if (Velocity.X < -0.0001f)
        {
            flip = SpriteEffects.FlipHorizontally;
        }
        
        Vector2 drawPos = (CurrentBodyState is PlayerBodyDamagedState) ? Position + _damagedStateOffset : Position;
        BodySprite?.Draw(spriteBatch, drawPos, Color.White, 0.0f,
            new Vector2(0, 0), 1.0f, flip, 0.0f);
        
        HeadSprite?.Draw(spriteBatch, Position + _headOffset, Color.White);
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

    public void SetSkin(string category, string skinPrefix)
    {
        if (_skins.ContainsKey(category))
        {
            _skins[category] = skinPrefix;
            
            // Force re-entry of states to grab the correct skin
            CurrentHeadState.Enter();
            CurrentBodyState.Enter();
        }
    }

    public string GetSkin(string category)
    {
        return _skins.ContainsKey(category) ? _skins[category] : "";
    }

    public void ChangeHeadState(IPlayerHeadState newHeadState)
    {
        if (CurrentHeadState != newHeadState)
        {
            CurrentHeadState?.Exit();
            CurrentHeadState = newHeadState;
            CurrentHeadState?.Enter();
        }
    }

    public void ChangeBodyState(IPlayerBodyState newBodyState)
    {
        if (CurrentBodyState != newBodyState)
        {
            CurrentBodyState?.Exit();
            CurrentBodyState = newBodyState;
            CurrentBodyState?.Enter();
        }
    }

    public Vector2 GetPosition()
    {
        return Position;
    }

    // More explicit interface implementation for renaming purposes
    void IPlayer.ChangeState(IPlayerState newState)
    {
        switch (newState)
        {
            case IPlayerHeadState:
            {
                ChangeHeadState((IPlayerHeadState)newState);
                break;
            }

            case IPlayerBodyState:
            {
                ChangeBodyState((IPlayerBodyState)newState);
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
        
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, 20, 20);
        Health = 6;
        MaxHealth = 6;
        Stats = new PlayerStats();
        InputBuffer = new PlayerInputBuffer();
        
        Inventory = new PlayerInventory(this);
        
        /*
         * Other properties such as CurrentPrimaryWeapon set by public methods
         * after instantiation
         */
        
        _skins = new Dictionary<string, string>
        {
            {"Head", "PlayerHead"},
            {"Body", "PlayerBody"},
        };
        CurrentHeadState = new PlayerHeadIdleState(this, Velocity);
        CurrentBodyState = new PlayerBodyIdleState(this);
        CurrentHeadState.Enter();
        CurrentBodyState.Enter();
    }
}
