using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.Commands.PlayerInventoryManagement;

public class NextSecondaryWeaponCommand : ICommand
{
    private readonly IPlayer _player;

    public NextSecondaryWeaponCommand(IPlayer player)
    {
        this._player = player;
    }

    public void Execute()
    {
        List<ISecondaryWeapon> weapons = _player.Inventory.Weapons.OfType<ISecondaryWeapon>().ToList();
        if (weapons.Count <= 1) return;

        ISecondaryWeapon current = _player.Inventory.CurrentSecondaryWeapon;
        int currentIndex = weapons.IndexOf(current);
        int nextIndex = (currentIndex + 1 + weapons.Count ) % weapons.Count;

        _player.Inventory.CurrentSecondaryWeapon = weapons[nextIndex];
    }
}