using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Collisions;

public class CollisionManager
{
    private List<IEntity> _dynamicEntities;
    private List<IEntity> _staticEntities;

    public CollisionManager()
    {
        this._dynamicEntities = new List<IEntity>();
        this._staticEntities = new List<IEntity>();
    }

    public void Update(GameTime delta)
    {
        /*
         * Check dynamic vs. dynamic entities
         */
        for (int i = 0; i < _dynamicEntities.Count; i++)
        {
            for (int j = i + 1; j < _dynamicEntities.Count; j++)
            {
                if (CollisionDetector.CheckRectangleCollision(
                        _dynamicEntities[i].Hitbox,
                        _dynamicEntities[j].Hitbox)
                   )
                {
                    _dynamicEntities[i].
                }
            }
        }
        
        /*
         * Check dynamic vs. static entities
         */
        
        /*
         * Do not check static vs. static entities
         */
    }
}