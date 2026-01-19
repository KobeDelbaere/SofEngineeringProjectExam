using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities
{
    public class Spikes : IGameObject, IHazard
    {
        Texture2D texture;
        Rectangle source = new Rectangle(0, 0, 16, 16);
        Vector2 position;
        float scale = 4;
        float rotation; // In radialen
        Vector2 origin;

        public Rectangle Bounds
        {
            get
            {
                // Voor geroteerde objecten berekenen we de bounds anders
                int width = (int)(source.Width * scale);
                int height = (int)(source.Height * scale);

                // Als de rotatie 90 of 270 graden is, wissel width en height
                float degrees = MathHelper.ToDegrees(rotation) % 360;
                if (degrees == 90 || degrees == 270 || degrees == -90 || degrees == -270)
                {
                    return new Rectangle(
                        (int)(position.X - origin.X * scale),
                        (int)(position.Y - origin.Y * scale),
                        height,
                        width
                    );
                }

                return new Rectangle(
                    (int)(position.X - origin.X * scale),
                    (int)(position.Y - origin.Y * scale),
                    width,
                    height
                );
            }
        }

        // Constructor met default waarden
        public Spikes(Texture2D tex, Vector2 pos, float rotationDegrees = 0f)
        {
            texture = tex;
            position = pos;
            this.scale = scale;
            this.rotation = MathHelper.ToRadians(rotationDegrees);

            // Origin is standaard de linkerbovenhoek
            origin = Vector2.Zero;

            // Als we roteren, zetten we de origin naar het midden voor betere rotatie
            if (rotationDegrees != 0)
            {
                origin = new Vector2(source.Width / 2f, source.Height / 2f);
                // Compenseer positie voor de origin
                position += origin * scale;
            }
        }

        public void OnHit(Hero hero)
        {
            //hero.Respawn(new Vector2(2000, 200));
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                position,
                source,
                Color.White,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}