#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.UI
{
    internal sealed class HUD
    {
        private readonly IPlayer _player;
        private readonly MiniMap _miniMap;

        private readonly ISprite _heartHalfSprite;
        private readonly ISprite _heartFilledSprite;
        private readonly ISprite _heartEmptySprite;
        private readonly ISprite _bombIndicator;
        private readonly ISprite _basicIndicator;
        private readonly ISprite _fireballIndicator;

        public HUD(IPlayer player, RoomManager roomManager, GraphicsDevice graphicsDevice)
        {
            _player = player;
            _miniMap = new MiniMap(roomManager, graphicsDevice);

            _heartHalfSprite = SpriteFactory.Instance.CreateStaticSprite("HalfHeart");
            _heartFilledSprite = SpriteFactory.Instance.CreateStaticSprite("FilledHeart");
            _heartEmptySprite = SpriteFactory.Instance.CreateStaticSprite("EmptyHeart");
            _bombIndicator = SpriteFactory.Instance.CreateStaticSprite("BombIndicator");
            _basicIndicator = SpriteFactory.Instance.CreateStaticSprite("BasicProjectile");
            _fireballIndicator = SpriteFactory.Instance.CreateStaticSprite("FireballProjectile");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawHearts(spriteBatch);
            DrawWeaponIndicator(spriteBatch);
            _miniMap.Draw(spriteBatch);
        }

        private void DrawHearts(SpriteBatch spriteBatch)
        {
            int maxHeartSlots = _player.MaxHealth / 2;
            float scale = 4f;
            int spacing = 48;

            for (int i = 0; i < maxHeartSlots; i++)
            {
                Vector2 pos = new Vector2(i * spacing, 20);
                int heartSlot = (i * 2) + 1;

                if (_player.Health >= heartSlot + 1)
                {
                    _heartFilledSprite.Draw(spriteBatch, pos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                }
                else if (_player.Health == heartSlot)
                {
                    _heartHalfSprite.Draw(spriteBatch, pos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                }
                else
                {
                    _heartEmptySprite.Draw(spriteBatch, pos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                }
            }
        }

        private void DrawWeaponIndicator(SpriteBatch spriteBatch)
        {
            //_fireballIndicator.Draw(spriteBatch, new Vector2(10, 100), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
            _basicIndicator.Draw(spriteBatch, new Vector2(10, 100), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            _bombIndicator.Draw(spriteBatch, new Vector2(80, 90), Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
        }

        public void Reset()
        {
            _miniMap.Reset();
        }
    }
}