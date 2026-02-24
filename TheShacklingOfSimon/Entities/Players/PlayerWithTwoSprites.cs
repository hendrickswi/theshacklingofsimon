using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players.States;
using TheShacklingOfSimon.Entities.Players.States.Body;
using TheShacklingOfSimon.Entities.Players.States.Head;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Sprites.Products;
using TheShacklingOfSimon.Weapons;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerWithTwoSprites : DamageableEntity, IPlayer
{
    public Inventory Inventory { get; private set; }
    public IWeapon CurrentPrimaryWeapon { get; private set; }
    public IWeapon CurrentSecondaryWeapon { get; private set; }
    public IItem CurrentItem { get; private set; }
    
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

    public float MoveSpeedStat { get; set; }
    public float DamageMultiplierStat { get; set; }
    public float PrimaryAttackCooldown { get; set; }
    public float ProjectileSpeedMultiplierStat { get; set; }
    public float SecondaryAttackCooldown { get; set; }
    public float MovementFrameDuration { get; set; }
    public float DeathFrameDuration { get; set; }
    public float InvulnerabilityDuration { get; set; }
    
    private readonly Vector2 _headOffset = new Vector2(-4.75f, -16);
    private readonly Vector2 _damagedStateOffset = new Vector2(0, -5);
    private Vector2 _movementInput;
    private Vector2 _primaryAttackInput;
    private Vector2 _secondaryAttackInput;

    public PlayerWithTwoSprites(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public void AddWeaponToInventory(IWeapon weapon)
    {
        Inventory.AddWeapon(weapon);
    }

    public IWeapon RemoveWeaponFromInventory(int pos)
    {
        IWeapon weapon = new NoneWeapon();
        if (pos < Inventory.Weapons.Count)
        {
            if (Inventory.Weapons[pos] == CurrentPrimaryWeapon)
            {
                CurrentPrimaryWeapon = new NoneWeapon();
            }
            else if (Inventory.Weapons[pos] == CurrentSecondaryWeapon)
            {
                CurrentSecondaryWeapon = new NoneWeapon();
            }

            weapon = Inventory.RemoveWeapon(pos);
        }

        return weapon;
    }
    
    public void AddItemToInventory(IItem item)
    {
        Inventory.AddItem(item);
    }

    public IItem RemoveItemFromInventory(int pos)
    {
        IItem item = new NoneItem(this);
        if (pos < Inventory.Items.Count)
        {
            if (Inventory.Items[pos] == CurrentItem)
            {
                CurrentPrimaryWeapon = new NoneWeapon();
            }

            item = Inventory.RemoveItem(pos);
        }

        return item;
    }

    public void EquipPrimaryWeapon(int pos)
    {
        if (pos < Inventory.Weapons.Count)
        {
            CurrentPrimaryWeapon = Inventory.Weapons[pos];
        }
    }

    public void EquipSecondaryWeapon(int pos)
    {
        if (pos < Inventory.Weapons.Count)
        {
            CurrentSecondaryWeapon = Inventory.Weapons[pos];
        }
    }

    public void EquipItem(int pos)
    {
        if (pos < Inventory.Items.Count)
        {
            CurrentItem = Inventory.Items[pos];
        }
    }
    
    public void RegisterMoveInput(Vector2 direction)
    {
        /*
         * Allow diagonal movement
         * 
         * Also catches edge cases where the player
         * presses three movement keys
         */
        _movementInput += direction;
    }

    public void RegisterPrimaryAttackInput(Vector2 direction)
    {
        if (CurrentPrimaryWeapon != null)
        {
            _primaryAttackInput += direction;
        }
    }

    public void RegisterSecondaryAttackInput(Vector2 direction)
    {
        if (CurrentSecondaryWeapon != null)
        {
            _secondaryAttackInput += direction;
        }
    }

    public void SetPosition(Vector2 worldPosition)
    {
        Position = worldPosition;
        Velocity = Vector2.Zero; // stop sliding after teleport
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 30, 30);
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
            ChangeBodyState(new PlayerBodyDeadState(this, DeathFrameDuration, DeathFrameDuration));
        }
        // If not dead, then damaged
        else
        {
            ChangeHeadState(new PlayerHeadDamagedState(this));
            ChangeBodyState(new PlayerBodyDamagedState(this, InvulnerabilityDuration));
        }
    }

    public void Reset(Vector2 startPosition)
    {
        Initialize(startPosition);
    }

    public override void Update(GameTime delta)
    {
        // Movement logic
        if (_movementInput.LengthSquared() > 0.0001f)
        {
            _movementInput.Normalize();
        }
        CurrentBodyState.HandleMovement(_movementInput, MovementFrameDuration);
        _movementInput = Vector2.Zero;
        
        // Attack logic
        if (_primaryAttackInput.LengthSquared() > 0.0001f)
        {
            CurrentHeadState.HandlePrimaryAttack(_primaryAttackInput, PrimaryAttackCooldown);
        }
        if (_secondaryAttackInput.LengthSquared() > 0.0001f)
        {
            CurrentHeadState.HandleSecondaryAttack(_secondaryAttackInput, SecondaryAttackCooldown);
        }

        _primaryAttackInput = Vector2.Zero;
        _secondaryAttackInput = Vector2.Zero;
        
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 30, 30);
        
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

        if (BodySprite != null)
        {
            Vector2 drawPos = (CurrentBodyState is PlayerBodyDamagedState) ? 
                    Position + _damagedStateOffset : Position;
            BodySprite.Draw(spriteBatch, drawPos, Color.White, 0.0f,
                        new Vector2(0, 0), 1.0f, flip, 0.0f);
        }

        if (HeadSprite != null)
        {
            HeadSprite.Draw(spriteBatch, Position + _headOffset, Color.White);
        }
    }

    public void ChangeHeadState(IPlayerHeadState newHeadState)
    {
        if (CurrentHeadState != newHeadState)
        {
            CurrentHeadState.Exit();
            CurrentHeadState = newHeadState;
            CurrentHeadState.Enter();
        }
    }

    public void ChangeBodyState(IPlayerBodyState newBodyState)
    {
        if (CurrentBodyState != newBodyState)
        {
            CurrentBodyState.Exit();
            CurrentBodyState = newBodyState;
            CurrentBodyState.Enter();
        }
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
        
        // IEntity properties
        this.Position = startPosition;
        this.Velocity = Vector2.Zero;
        this.IsActive = true;
        // Arbitrarily sized hitbox of 30x30
        this.Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, 30, 30);
        
        // IDamageable properties
        this.Health = 6;
        this.MaxHealth = 6;
        
        // Player property defaults
        // These can all be overriden with public set method
        this.DamageMultiplierStat = 1.0f;
        this.MoveSpeedStat = 100.0f;
        this.PrimaryAttackCooldown = 0.5f;
        this.ProjectileSpeedMultiplierStat = 1.0f;
        this.SecondaryAttackCooldown = 1.5f;
        this.MovementFrameDuration = 0.05f;
        this.DeathFrameDuration = 1.0f;
        this.InvulnerabilityDuration = 0.333334f;
        this.Inventory = new Inventory();
        
        /*
         * Other properties such as CurrentPrimaryWeapon set by public methods
         * after instantiation
         */
        
        this.CurrentHeadState = new PlayerHeadIdleState(this, Velocity);
        this.CurrentBodyState = new PlayerBodyIdleState(this);
        this.CurrentHeadState.Enter();
        this.CurrentBodyState.Enter();
        this._movementInput = Vector2.Zero;
        this._primaryAttackInput = Vector2.Zero;
        this._secondaryAttackInput = Vector2.Zero;
    }
}