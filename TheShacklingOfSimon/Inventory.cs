using System.Collections.Generic;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon;

public class Inventory
{
    public List<IWeapon> Weapons { get; private set; }
    public List<IItem> Items { get; private set; }
    
    public Inventory()
    {
        Weapons = new List<IWeapon>();
        Items = new List<IItem>();
    }
    
    public void AddWeapon(IWeapon w)
    {
        if (!Weapons.Contains(w))
        {
            Weapons.Add(w);
        }
    }

    /*
     * Returns null if the specified IWeapon does not exist.
     */
    public IWeapon RemoveWeapon(int pos)
    {
        IWeapon item = null;
        if (pos < Weapons.Count)
        {
            item = Weapons[pos];
            Items.RemoveAt(pos);
        }
        return item;
    }
    
    public void AddItem(IItem item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
        }
    }

    /*
     * Returns null if the specified IItem does not exist.
     */
    public IItem RemoveItem(int pos)
    {
        IItem item = null;
        if (pos < Items.Count)
        {
            item = Items[pos];
            Items.RemoveAt(pos);
        }
        return item;
    }
}