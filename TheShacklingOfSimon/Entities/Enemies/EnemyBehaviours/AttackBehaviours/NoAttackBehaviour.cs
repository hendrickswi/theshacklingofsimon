using Vector2 = Microsoft.Xna.Framework.Vector2;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public class NoAttackBehaviour : IAttackBehaviour
{
    public void Execute(BaseEnemy enemy, float dt, Vector2 targetDirection)
    {
        // Do nothing
    }
}