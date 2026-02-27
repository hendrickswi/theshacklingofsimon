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
                        _dynamicEntities[j].Hitbox
                        )
                    )
                {
                    /*
                     * Trigger the "double dispatch"
                     *      i.e., let the entities resolve their own changes to
                     *      their respective health, state, physics, etc.
                     */
                    _dynamicEntities[i].OnCollision(_dynamicEntities[j]);
                    _dynamicEntities[j].OnCollision(_dynamicEntities[i]);
                }
            }
        }
        
        /*
         * Check dynamic vs. static entities
         */
        foreach (IEntity dynamicEntity in _dynamicEntities)
        {
            foreach (IEntity staticEntity in _staticEntities)
            {
                if (CollisionDetector.CheckRectangleCollision(
                        dynamicEntity.Hitbox,
                        staticEntity.Hitbox
                        )
                    )
                {
                    /*
                     * Trigger the "double dispatch"
                     *      i.e., let the entities resolve their own changes to
                     *      their respective health, state, physics, etc.
                     */
                    dynamicEntity.OnCollision(staticEntity);
                    staticEntity.OnCollision(dynamicEntity);
                }
            }
        }
        
        /*
         * Do not check static vs. static entities
         */
    }

    public void AddDynamicEntity(IEntity dynamicEntity)
    {
        _dynamicEntities.Add(dynamicEntity);
    }

    public IEntity RemoveDynamicEntity(int pos)
    {
        IEntity result = null;
        if (pos < _dynamicEntities.Count)
        {
            result = _dynamicEntities[pos];
            _dynamicEntities.RemoveAt(pos);
        }

        return result;
    }

    public void AddStaticEntity(IEntity staticEntity)
    {
        _staticEntities.Add(staticEntity);
    }

    public IEntity RemoveStaticEntity(int pos)
    {
        IEntity result = null;
        if (pos < _staticEntities.Count)
        {
            result = _staticEntities[pos];
            _staticEntities.RemoveAt(pos);
        }

        return result;
    }
}