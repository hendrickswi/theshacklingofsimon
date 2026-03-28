using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheShacklingOfSimon.Entities.Enemies
{
    public class EnemyManager
    {
        private readonly List<IEnemy> _enemies = new();

        public IReadOnlyList<IEnemy> Enemies => _enemies.AsReadOnly();

        public void AddEnemy(IEnemy enemy)
        {
            if (enemy != null && !_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
            }
        }

        public void RemoveEnemy(IEnemy enemy)
        {
            if (enemy != null)
            {
                _enemies.Remove(enemy);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                IEnemy enemy = _enemies[i];
                enemy.Update(gameTime);

                // Remove enemies that are marked for removal
                if (enemy.IsActive == false)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IEnemy enemy in _enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        public void ClearAllEnemies()
        {
            _enemies.Clear();
        }
    }
}