using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players.Drawing;
using TheShacklingOfSimon.Entities.Players.States.Body;
using TheShacklingOfSimon.Entities.Players.States.Head;

namespace TheShacklingOfSimon.Entities.Players.States;

public class PlayerTwoStateManager
{
    private readonly PlayerWithTwoSprites _player;
    private readonly PlayerWithTwoSpritesDrawManager _drawManager;
    
    public IPlayerHeadState Head { get; private set; }
    public IPlayerBodyState Body { get; private set; }

    public PlayerTwoStateManager(PlayerWithTwoSprites player, PlayerWithTwoSpritesDrawManager drawManager)
    {
        _player = player;
        _drawManager = drawManager;
        
        Head = new PlayerHeadIdleState(player, Vector2.Zero);
        Body = new PlayerBodyIdleState(player);
        Head.Enter();
        Body.Enter();
    }

    public void Update(GameTime delta)
    {
        Head.Update(delta);
        Body.Update(delta);
    }
    
    public void ChangeHeadState(IPlayerHeadState newState)
    {
        Head.Exit();
        Head = newState;
        Head.Enter();
    }
    
    public void ChangeBodyState(IPlayerBodyState newState)
    {
        Body.Exit();
        Body = newState;
        Body.Enter();
    }

    public void HandleDamageInterrupt(bool lethal)
    {
        if (lethal)
        {
            ChangeHeadState(new PlayerHeadDeadState(_player));
            ChangeBodyState(new PlayerBodyDeadState(_player, _drawManager.DeathFrameDuration, _drawManager.DeathFrameDuration));
        }
        else
        {
            ChangeHeadState(new PlayerHeadDamagedState(_player));
            ChangeBodyState(new PlayerBodyDamagedState(_player, _drawManager.HurtFrameDuration));
        }
    }
}