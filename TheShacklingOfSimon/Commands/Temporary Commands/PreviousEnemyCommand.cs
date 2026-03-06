using TheShacklingOfSimon.Entities.Enemies;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class PreviousEnemyCommand : ICommand
{
    private readonly EnemyManager _enemyManager;

    public PreviousEnemyCommand(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }

    //not for use in current implementation
    public void Execute()
    {
        //_enemyManager.PreviousEnemy();
    }
}