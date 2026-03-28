using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.GameStates;

public interface IGameState
{
    void Enter();
    void Exit();
    void Update(GameTime delta);
    void Draw(SpriteBatch spriteBatch);
}