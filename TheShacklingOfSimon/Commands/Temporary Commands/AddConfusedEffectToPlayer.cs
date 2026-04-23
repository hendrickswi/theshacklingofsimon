using System;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects.Implementations.Complex;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class AddConfusedEffectToPlayerCommand : ICommand
{
    private readonly IPlayer _player;
    private readonly Func<IStatusEffect> _effectFactory;

    public AddConfusedEffectToPlayerCommand(IPlayer player)
    {
        _player = player;
        _effectFactory = () => new ConfusedEffect("Confused!", player, 2f);
    }

    public void Execute()
    {
        _player.EffectManager.AddEffect(_effectFactory.Invoke());
    }
}