using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Core
{
    public class Camera2D
    {
        public Vector2 Position;

        public Matrix GetMatrix(Viewport vp)
        {
            return
                Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                Matrix.CreateTranslation(vp.Width / 2f, vp.Height / 2f, 0);
        }

        public void Follow(Rectangle target)
        {
            Position = new Vector2(target.Center.X + 200, target.Center.Y);
        }
    }
}
