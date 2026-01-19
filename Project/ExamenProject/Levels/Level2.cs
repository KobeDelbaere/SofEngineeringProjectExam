using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Levels
{
    
    public class Level2 : BaseLevel
    {
        public Level2(
        ContentManager content,
        GraphicsDevice device,
        CollisionService collision,
        TextureManager textures)
        : base(content, device, collision, textures) { }

        public override Vector2 GetSpawnPoint()
        {
            return new Vector2(0, 500);
        }

        protected override void BuildLevel()
        {
            // Starting area
            for (int i = -2; i < 0; i++)
            {
                AddFloor(i * 80, 640);
            }

            for (int i = 0; i < 50; i++)
            {
                AddFloor(i * 80, 720);
            }

            for (int i = 0; i < 10; i++)
            {
                AddFloor(i * 320, 640);
            }

            AddFloor(2080, 480);

            for (int i = 0; i < 10; i++)
            {
                AddSpike(new Vector2(84 + i * 320, 656), rotationDegrees: 0);
                AddSpike(new Vector2(164 + i * 320, 656), rotationDegrees: 0);
            }

            for (int i = 0; i < 9; i++)
            {
                AddSpike(new Vector2(244 + i * 320, 656), rotationDegrees: 0);
            }

            AddBallistaEnemy(new Vector2(3300, 480), new Vector2(-1, 0), rotationDegrees: 270f);

            AddLevelEnd(3800, 596);
        }
    }
}