using ExamenProject.Entities.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenProject.Interfaces
{
    public interface ILevel
    {
        void LoadLevel();
        void Update(GameTime gameTime, Hero hero);
        void Draw(SpriteBatch spriteBatch);
        Vector2 GetSpawnPoint();
        bool IsLevelComplete { get; }
    }
}
