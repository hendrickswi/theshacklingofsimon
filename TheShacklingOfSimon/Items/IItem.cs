using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    IPlayer Player { get; }
    void ItemEffect();
}