using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Entities.Players.States;

public class PlayerHeadIdleState : IPlayerHeadState
{
    private IPlayer _player;
    private Vector2 _lookingDirection;

    public PlayerHeadIdleState(IPlayer player, Vector2 lastDirection)
    {
        _player = player;
        _lookingDirection = (lastDirection.LengthSquared() < 0.0001f) ? new Vector2(0, 1) : lastDirection;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update(GameTime delta)
    {
        
    }

    public void HandleAttack(Vector2 direction)
    {
        
    }

    public void HandleAttackSecondary(Vector2 direction)
    {
        
    }
}