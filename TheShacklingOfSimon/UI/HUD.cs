using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;
using static System.Formats.Asn1.AsnWriter;

namespace TheShacklingOfSimon.UI
{
    internal class HUD
    {
        private IPlayer _player;
        private ISprite _heartHalfSprite;
        private ISprite _heartFilledSprite;
        private ISprite _heartEmptySprite;
        private ISprite _bombIndicator;
        private ISprite _basicIndicator;
        private ISprite _fireballIndicator;

        public HUD(IPlayer player)
        {
            _player = player;
           
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
            WeaponIndicator(spriteBatch);
            
        }

        private void DrawHearts(SpriteBatch spriteBatch)
        {
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

        private void WeaponIndicator(SpriteBatch spriteBatch) {

            //_fireballIndicator.Draw(spriteBatch, new Vector2(10, 100), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
            _basicIndicator.Draw(spriteBatch, new Vector2(10, 100), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            _bombIndicator.Draw(spriteBatch, new Vector2(80 , 90), Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);

        }
    }
}