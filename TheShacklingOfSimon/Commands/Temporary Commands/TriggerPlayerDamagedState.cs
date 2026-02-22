using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class TriggerPlayerDamagedState : ICommand
{
    private readonly IPlayer _player;
    private readonly int _dmg;

    public TriggerPlayerDamagedState(IPlayer player, int dmg)
    {
        _player = player;
        _dmg = dmg;
    }

    public void Execute()
    {
        _player.TakeDamage(_dmg);
    }
}