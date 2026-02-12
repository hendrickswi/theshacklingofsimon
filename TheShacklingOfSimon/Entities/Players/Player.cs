using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Projectiles;
using TheShacklingOfSimon.Sprites.Products;
using TheShacklingOfSimon.Weapons;
using TheShacklingOfSimon.Entities.Players.States;

namespace TheShacklingOfSimon.Entities.Players;

public class Player : DamageableEntity, IPlayer
{
    public Inventory Inventory { get; private set; }
    public IWeapon CurrentWeapon { get; private set; }
    public IWeapon CurrentSecondaryWeapon { get; private set; }
    public IItem CurrentItem { get; private set; }
    
    public IPlayerHeadState CurrentHeadState { get; private set; }
    public IPlayerBodyState CurrentBodyState { get; private set; }
    
    
    public ISprite HeadSprite { get; private set; }
    /*
     * Sprite property inherited from IEntity
     * Sprite is the body sprite in this class.
     */ 

    public float MoveSpeedStat { get; private set; }
    public float DamageMultiplierStat { get; private set; }
    
    private readonly Vector2 _headOffset = new Vector2(0, -15);
    private Vector2 _movementInput;

    public Player(Vector2 startPosition)
    {
        // IEntity properties
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsActive = true;
        // Arbitrarily sized hitbox of 20x20
        Hitbox = new Rectangle((int)startPosition.X, (int)startPosition.Y, 20, 20);
        // Not setting the initial Sprite in the constructor
        
        // IDamageable properties
        this.Health = 3;
        this.MaxHealth = 3;
        
        // Player properties
        this.Inventory = new Inventory();
        this.Inventory.AddWeapon(new BasicWeapon());
        this.Inventory.AddWeapon(new BombWeapon());
        this.Inventory.AddItem(new NoneItem());
        this.CurrentWeapon = Inventory.Weapons[0];
        this.CurrentWeapon = Inventory.Weapons[1];
        this.CurrentItem = Inventory.Items[0];
        
        this.DamageMultiplierStat = 1.0f;
        this.MoveSpeedStat = 20.0f;
        
        this.CurrentHeadState = new PlayerHeadIdleState();
        this.CurrentBodyState = new PlayerBodyIdleState();
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

    public void EquipWeapon(int pos)
    {
        if (pos < Inventory.Weapons.Count)
        {
            CurrentWeapon = Inventory.Weapons[pos];
        }
    }

    public void EquipItem(int pos)
    {
        if (pos < Inventory.Items.Count)
        {
            CurrentItem = Inventory.Items[pos];
        }
    }

    public void Attack(Vector2 direction)
    {
        CurrentHeadState.HandleAttack(direction);
    }

    public void AttackSecondary(Vector2 direction)
    {
        CurrentHeadState.HandleAttackSecondary(direction);
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

    public override void Update(GameTime delta)
    {
        if (_movementInput.LengthSquared() > 0)
        {
            _movementInput.Normalize();
        }
        CurrentBodyState.HandleMovement(this, _movementInput);
        _movementInput = Vector2.Zero;
        
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;
        Position += Velocity * dt;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 20, 20);
        
        CurrentHeadState.Update(delta);
        CurrentBodyState.Update(delta);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        SpriteEffects flip = SpriteEffects.None;
        if (Velocity.X < 0)
        {
            flip = SpriteEffects.FlipHorizontally;
        }
        
        Sprite.Draw(spriteBatch, Position, Color.White, 0.0f,
            new Vector2(0, 0), 1.0f, flip, 0.0f);
    }

    public void ChangeHeadState(IPlayerHeadState newHeadState)
    {
        if (CurrentHeadState != newHeadState)
        {
            CurrentHeadState.Exit(this);
            CurrentHeadState = newHeadState;
            CurrentHeadState.Enter(this);
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
}