using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Items;

public interface IActiveItem : IItem
{
    void ClearEffect();
    void Update(GameTime delta);
}