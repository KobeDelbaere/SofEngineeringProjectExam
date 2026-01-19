using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities
{
    public class Floor : IGameObject
    {
        Texture2D texture;
        public Rectangle Bounds { get; }

        public Floor(Texture2D tex, int x, int y, int w, int h)
        {
            texture = tex;
            Bounds = new Rectangle(x, y, w, h);
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}
