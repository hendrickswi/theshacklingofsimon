#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.UI
{
   public class HUD
    {
        private const int BossHealthBarWidth = 50;
        private const int BossHealthBarHeight = 10;
        
        private readonly IPlayer _player;
        private readonly MiniMap _miniMap;
        private readonly RoomManager _roomManager;
        private readonly GraphicsDevice _graphicsDevice;

        private readonly ISprite _heartHalfSprite;
        private readonly ISprite _heartFilledSprite;
        private readonly ISprite _heartEmptySprite;
        private readonly ISprite _bombIndicator;
        private readonly ISprite _basicIndicator;
        private readonly ISprite _fireballIndicator;
        private readonly ISprite _Coin;
        private readonly ISprite _key;
   

        private readonly ISprite _pixelSprite;
        private ISprite _coinFont;
        private ISprite _keyFont;
       
        public HUD(IPlayer player, RoomManager roomManager, GraphicsDevice graphicsDevice)
        {
            _player = player;
            _miniMap = new MiniMap(roomManager, graphicsDevice);
            _roomManager = roomManager;
            _graphicsDevice = graphicsDevice;

            _heartHalfSprite = SpriteFactory.Instance.CreateStaticSprite("HalfHeart");
            _heartFilledSprite = SpriteFactory.Instance.CreateStaticSprite("FilledHeart");
            _heartEmptySprite = SpriteFactory.Instance.CreateStaticSprite("EmptyHeart");
            _bombIndicator = SpriteFactory.Instance.CreateStaticSprite("BombIndicator");
            _basicIndicator = SpriteFactory.Instance.CreateStaticSprite("BasicProjectile");
            _fireballIndicator = SpriteFactory.Instance.CreateStaticSprite("FireballProjectile");
            _Coin = SpriteFactory.Instance.CreateStaticSprite("Coin");
            _key = SpriteFactory.Instance.CreateStaticSprite("key");
            



            _pixelSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawHearts(spriteBatch);
            DrawWeaponIndicator(spriteBatch);
            DrawPickupIndicators(spriteBatch);
            _miniMap.Draw(spriteBatch);
            if (_roomManager.CurrentRoom.IsBossRoom)
            {
                DrawBossHealthBar(spriteBatch);
            }
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

        private void DrawBossHealthBar(SpriteBatch spriteBatch)
        {
            IEnemy boss = null;
            foreach (IEntity e in _roomManager.CurrentRoom.Entities)
            {
                if (boss != null) break;
                if ( e is not IEnemy enemy || !enemy.IsBoss) continue;
                boss = enemy;
            }

            if (boss == null) return;
            float healthPercent = MathHelper.Clamp((float) boss.Health / boss.MaxHealth, 0f, 1f);

            int x = (int) (_graphicsDevice.Viewport.Width - BossHealthBarHeight) / 2;
            int y = 20;
            
            Rectangle backgroundRectangle = new Rectangle(x, y, BossHealthBarWidth, BossHealthBarHeight);
            Rectangle foregroundRectangle = new Rectangle(x, y, (int) (BossHealthBarWidth * healthPercent), BossHealthBarWidth);

            _pixelSprite.Draw(spriteBatch, backgroundRectangle, Color.Black);
            _pixelSprite.Draw(spriteBatch, foregroundRectangle, Color.Red);
        }

        
        private void DrawPickupIndicators(SpriteBatch spriteBatch)
        {


           
            _coinFont = SpriteFactory.Instance.CreateTextSprite("Upheaval16", "x" + _player.Inventory.NumCoins.ToString());
            _coinFont.Draw(spriteBatch, new Vector2(80, 160), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

            _keyFont = SpriteFactory.Instance.CreateTextSprite("Upheaval16", "x" + _player.Inventory.NumKeys.ToString());
            _keyFont.Draw(spriteBatch, new Vector2(80, 210), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

            _Coin.Draw(spriteBatch, new Vector2(20 , 160), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            _key.Draw(spriteBatch, new Vector2(25, 210), Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
            


        }
        public void Reset()
        {
            _miniMap.Reset();
        }
    }
}