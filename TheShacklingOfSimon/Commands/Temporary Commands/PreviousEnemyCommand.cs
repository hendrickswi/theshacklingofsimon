using TheShacklingOfSimon.Entities.Enemies;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class PreviousEnemyCommand : ICommand
{
    private readonly EnemyManager _enemyManager;

    public PreviousEnemyCommand(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }

    public void Execute()
    {
        _enemyManager.PreviousEnemy();
    }
}