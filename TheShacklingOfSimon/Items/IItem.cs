using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Projectiles;

namespace TheShacklingOfSimon.Items;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    void Effect();
}