using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.UI
{
    internal class HUD
    {
        private IPlayer _player;
        private ISprite _heartHalfSprite;
        private ISprite _heartFilledSprite;
        private ISprite _heartEmptySprite;

        public HUD(IPlayer player)
        {
            _player = player;
           
            _heartHalfSprite = SpriteFactory.Instance.CreateStaticSprite("HalfHeart");
            _heartFilledSprite = SpriteFactory.Instance.CreateStaticSprite("FilledHeart");
            _heartEmptySprite = SpriteFactory.Instance.CreateStaticSprite("EmptyHeart");
        }



    
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_player == null) return;

            int maxHeartSlots = _player.MaxHealth / 2;
            float scale = 4f;
            int spacing = 48; 

            for (int i = 0; i < maxHeartSlots; i++)
            {
                Vector2 pos = new Vector2(0 + (i * spacing), 20);

                
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
    }
}