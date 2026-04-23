using System;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects.Implementations.Complex;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.Commands.Temporary_Commands;

public class AddStunEffectToPlayerCommand : ICommand
{
    private readonly IPlayer _player;
    private readonly Func<IStatusEffect> _effectFactory;

    public AddStunEffectToPlayerCommand(IPlayer player)
    {
        _player = player;
        _effectFactory = () => new StunEffect("Stunned!", player, 2f, 3f);
    }

    public void Execute()
    {
        _player.EffectManager.AddEffect(_effectFactory.Invoke());
    }
}