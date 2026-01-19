using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities
{
    public class Platform : IGameObject
    {
        Texture2D texture;
        Rectangle source;
        Vector2 position;
        float scaleX;
        float scaleY;

        public Rectangle Bounds =>
            new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)(source.Width * scaleX),
                (int)(source.Height * scaleY)
            );

        // Constructor met aparte X en Y scale
        public Platform(Texture2D tex, Vector2 pos, Rectangle src, float scaleX = 2f, float scaleY = 2f)
        {
            texture = tex;
            position = pos;
            source = src;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                position,
                source,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(scaleX, scaleY),  // Aparte X en Y scale
                SpriteEffects.None,
                0f
            );
        }
    }
}