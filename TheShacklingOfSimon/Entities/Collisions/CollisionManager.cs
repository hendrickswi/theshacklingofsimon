using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Collisions;

public class CollisionManager
{
    private readonly List<IEntity> _dynamicEntities;
    private readonly List<IEntity> _staticEntities;

    public CollisionManager()
    {
        _dynamicEntities = new List<IEntity>();
        _staticEntities = new List<IEntity>();
    }

    public void Update(GameTime delta)
    {
        PruneInactive(_dynamicEntities);
        PruneInactive(_staticEntities);

        // Dynamic vs dynamic (each unordered pair once)
        for (int i = 0; i < _dynamicEntities.Count; i++)
        {
            IEntity a = _dynamicEntities[i];
            if (a == null) continue;

            for (int j = i + 1; j < _dynamicEntities.Count; j++)
            {
                IEntity b = _dynamicEntities[j];
                if (b == null) continue;

                if (CollisionDetector.CheckRectangleCollision(a.Hitbox, b.Hitbox))
                {
                    // manager calls both directions
                    a.OnCollision(b);
                    b.OnCollision(a);
                }
            }
        }

        // Dynamic vs static
        foreach (IEntity dynamicEntity in _dynamicEntities)
        {
            if (dynamicEntity == null) continue;

            foreach (IEntity staticEntity in _staticEntities)
            {
                if (staticEntity == null) continue;

                if (CollisionDetector.CheckRectangleCollision(dynamicEntity.Hitbox, staticEntity.Hitbox))
                {
                    dynamicEntity.OnCollision(staticEntity);
                    staticEntity.OnCollision(dynamicEntity);
                }
            }
        }
    }

    private static void PruneInactive(List<IEntity> entities)
    {
        // Remove null or inactive entries (e.g., destroyed tiles/projectiles)
        entities.RemoveAll(e => e == null || !e.IsActive);
    }

    public void AddDynamicEntity(IEntity dynamicEntity)
    {
        if (dynamicEntity == null || !dynamicEntity.IsActive) return;
        _dynamicEntities.Add(dynamicEntity);
    }

    public void AddStaticEntity(IEntity staticEntity)
    {
        if (staticEntity == null || !staticEntity.IsActive) return;
        _staticEntities.Add(staticEntity);
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

    public void ClearEntityLists()
    {  
        //clear for readonly
        _dynamicEntities.Clear();
        _staticEntities.Clear();
    }
}