using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Enemies
{
    public class EnemyManager
    {
        private readonly List<IEnemy> _enemies = new();
        private int _currentIndex = 0;

        public void AddEnemy(IEnemy enemy)
        {
            _enemies.Add(enemy);
        }

        public void NextEnemy()
        {
            if (_enemies.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _enemies.Count)
                _currentIndex = 0;
        }

        public void PreviousEnemy()
        {
            if (_enemies.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _enemies.Count - 1;
        }

        public void Update(GameTime gameTime)
        {
            if (_enemies.Count == 0) return;

            _enemies[_currentIndex].Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_enemies.Count == 0) return;

            _enemies[_currentIndex].Draw(spriteBatch);
        }
    }
}