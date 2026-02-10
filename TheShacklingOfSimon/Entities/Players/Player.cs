using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players;

public class Player : DamageableEntity, IPlayer
{
    // Inherits Health and Max 
    
    public IWeapon CurrentWeapon { get; private set; }
    public IItem CurrentItem { get; private set; }
    public Inventory Inventory { get; }
    
    public IPlayerState CurrentState { get; }

    public Player()
    {
        this.Health = 3;
        this.MaxHealth = 3;
        this.Inventory = new Inventory();
        this.CurrentItem = new NoneItem();
        this.CurrentWeapon = new BasicWeapon();
        this.CurrentState = new IdlePlayerState();
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
        // TODO
    }

    public void Move(Vector2 direction)
    {
        // TODO
    }

    public void Update(GameTime delta)
    {
        // TODO
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // TODO
    }

    public void ChangeState()
    {
        // TODO
    }
}