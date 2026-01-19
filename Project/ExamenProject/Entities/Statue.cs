using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities
{
    public class Statue : IGameObject, ILevelEnd
    {
        Texture2D texture;
        public Rectangle Bounds { get; }

        public Statue(Texture2D tex, int x, int y, int w, int h)
        {
            texture = tex;
            Bounds = new Rectangle(x, y, w, h);
        }

        public void Update(GameTime gameTime) { }

        public void OnHit(Hero hero)
        {
            // De level completion wordt afgehandeld in BaseLevel.Update()
            // Hier hoeven we niets te doen - het triggert alleen de check
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}