using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class ResetPlayerCommand : ICommand
{
    private readonly IPlayer _player;
    private readonly Vector2 _startPosition;

    public ResetPlayerCommand(IPlayer player, Vector2 startPosition)
    {
        _player = player;
        _startPosition = startPosition;
    }

    public void Execute()
    {
        _player.Reset(_startPosition);
    }
}