using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Items;
public class EquipItems : IItem
{
    public string Name { get; set; }
    public string Description { get; set; }

    public void Effect()
    {
        
    }

    void GiveStatChange(int amt)
    {
        
    }

    int ClearStatChange()
    {
        return 0;
    }
}