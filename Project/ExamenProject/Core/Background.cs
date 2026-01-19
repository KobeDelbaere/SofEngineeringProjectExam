using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Core
{
    public class Background
    {
        Texture2D texture;
        float speed;
        float scale;
        float offsetX;

        public float OffsetY { get; set; }

        public Background(Texture2D tex, float parallaxSpeed, float scale)
        {
            texture = tex;
            speed = parallaxSpeed;
            this.scale = scale;
        }

        public void Update(float cameraMoveX)
        {
            offsetX -= cameraMoveX * speed;

            float width = texture.Width * scale;

            if (offsetX <= -width) offsetX += width;
            if (offsetX >= width) offsetX -= width;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float width = texture.Width * scale;

            for (int i = -1; i <= 1; i++)
            {
                spriteBatch.Draw(
                    texture,
                    new Vector2(offsetX + i * width, OffsetY),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
