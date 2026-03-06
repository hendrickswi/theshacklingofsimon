using TheShacklingOfSimon.Entities.Enemies;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class NextEnemyCommand : ICommand
{
    private readonly EnemyManager _enemyManager;

    public NextEnemyCommand(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }

    //not for use in current implementation
    public void Execute()
    {
        //_enemyManager.NextEnemy();
    }
}