using System;
using System.Transactions;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Entities.Players;

public class PlayerInventory : Inventory
{
    private IWeapon _currentPrimaryWeapon;
    private IWeapon _currentSecondaryWeapon;
    private IItem _currentActiveItem;

    public IWeapon CurrentPrimaryWeapon
    {
        get { return _currentPrimaryWeapon; }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            if (!_weapons.Contains(value))
            {
                throw new ArgumentException("Provided IWeapon argument does not exist within the inventory.",
                    nameof(value));
            }
            
            if (_currentPrimaryWeapon == value)
            {
                return;
            }
            
            _currentPrimaryWeapon = value;
            NotifyInventoryChanged();
        }
    }

    public IWeapon CurrentSecondaryWeapon
    {
        get { return _currentSecondaryWeapon;  }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            if (!_weapons.Contains(value))
            {
                throw new ArgumentException("Provided IWeapon arguemnt does not exist within the inventory.",
                    nameof(value));
            }

            if (_currentSecondaryWeapon == value)
            {
                return;
            }
            
            _currentSecondaryWeapon = value;
            NotifyInventoryChanged();
        }
    }

    public IItem CurrentActiveItem
    {
        get { return _currentActiveItem; }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            if (!_items.Contains(value))
            {
                throw new ArgumentException("Provided IWeapon arguemnt does not exist within the inventory.",
                    nameof(value));
            }

            if (_currentActiveItem == value)
            {
                return;
            }
            
            _currentActiveItem = value;
            NotifyInventoryChanged();
        }
    }
    
    public PlayerInventory(IPlayer player) : base()
    {
        CurrentPrimaryWeapon = new NoneWeapon();
        CurrentSecondaryWeapon = new NoneWeapon();
        CurrentActiveItem = new NoneItem(player);
    }
}
