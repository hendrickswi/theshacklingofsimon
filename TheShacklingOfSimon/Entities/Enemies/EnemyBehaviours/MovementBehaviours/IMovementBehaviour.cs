using Vector2 = Microsoft.Xna.Framework.Vector2;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;

namespace TheShacklingOfSimon.Entities.Enemies.EnemyBehaviours;

public interface IMovementBehavior
{
    Vector2 GetMovement(BaseEnemy enemy, float dt, Vector2 targetDirection);
}