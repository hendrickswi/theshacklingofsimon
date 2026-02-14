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
    
    public IPlayerHeadState CurrentHeadState { get; private set; }
    public IPlayerBodyState CurrentBodyState { get; private set; }
    
    // Use explicit interface implementation 
    IPlayerState IPlayer.CurrentBodyState => CurrentBodyState;
    
    /*
     * Additional sprite to handle the head.
     * Allows non-combinatorial states
     *      i.e., head can be in shooting state and body can
     *      be in moving state, but they do so independently
     *      as opposed to a ShootingMoving combined state.
     */
    public ISprite HeadSprite { get; set; }
    /*
     * Sprite property inherited from IEntity
     * Sprite is arbitrarily the body sprite in this class.
     */ 

    public float MoveSpeedStat { get; set; }
    public float DamageMultiplierStat { get; set; }
    public float PrimaryAttackCooldown { get; set; }
    public float SecondaryAttackCooldown { get; set; }
    
    private readonly Vector2 _headOffset = new Vector2(-5, -15);
    private Vector2 _movementInput;
    private Vector2 _primaryAttackInput;
    private Vector2 _secondaryAttackInput;

    public PlayerWithTwoSprites(Vector2 startPosition)
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
        
        // Player properties
        this.Inventory = new Inventory();
        // this.Inventory.AddWeapon(new BasicWeapon());
        // this.Inventory.AddWeapon(new BombWeapon());
        // this.Inventory.AddItem(new NoneItem());
        // this.CurrentPrimaryWeapon = Inventory.Weapons[0];
        // this.CurrentSecondaryWeapon = Inventory.Weapons[1];
        // this.CurrentItem = Inventory.Items[0];
        
        // These can all be overriden with public set method
        this.DamageMultiplierStat = 1.0f;
        this.MoveSpeedStat = 20.0f;
        this.PrimaryAttackCooldown = 0.2f;
        this.SecondaryAttackCooldown = 0.5f;
        
        this.CurrentHeadState = new PlayerHeadIdleState(this, Velocity);
        this.CurrentBodyState = new PlayerBodyIdleState(this);
        this.CurrentHeadState.Enter();
        this.CurrentBodyState.Enter();
        this._movementInput = Vector2.Zero;
    }

    public void AddWeaponToInventory(IWeapon weapon)
    {
        Inventory.AddWeapon(weapon);
    }

    public IWeapon RemoveWeaponFromInventory(int pos)
    {
        return Inventory.RemoveWeapon(pos);
    }
    
    public void AddItemToInventory(IItem item)
    {
        Inventory.AddItem(item);
    }

    public IItem RemoveItemFromInventory(int pos)
    {
        return Inventory.RemoveItem(pos);
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
        _primaryAttackInput += direction;
    }

    public void RegisterSecondaryAttackInput(Vector2 direction)
    {
        _secondaryAttackInput += direction;
    }

    public override void Update(GameTime delta)
    {
        // Movement logic
        if (_movementInput.Length() > 0.0001f)
        {
            _movementInput.Normalize();
            CurrentBodyState.HandleMovement(_movementInput);
        }
        _movementInput = Vector2.Zero;
        
        // Attack logic
        if (_primaryAttackInput.Length() > 0.0001f)
        {
            CurrentHeadState.HandlePrimaryAttack(_primaryAttackInput, PrimaryAttackCooldown);
        }
        if (_secondaryAttackInput.Length() > 0.0001f)
        {
            CurrentHeadState.HandleSecondaryAttack(_secondaryAttackInput, SecondaryAttackCooldown);
        }

        _primaryAttackInput = Vector2.Zero;
        _secondaryAttackInput = Vector2.Zero;
        
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 20, 20);
        
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

        if (Sprite != null)
        {
            Sprite.Draw(spriteBatch, Position, Color.White, 0.0f,
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

    // More explicit interface implementation
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
}