using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Levels
{
    /// <summary>
    /// Level 1 - Tutorial/Easy level
    /// Volgt Open/Closed Principle - extends BaseLevel zonder het te modificeren
    /// </summary>
    public class Level1 : BaseLevel
    {
        public Level1(
        ContentManager content,
        GraphicsDevice device,
        CollisionService collision,
        TextureManager textures)
        : base(content, device, collision, textures)
        {
        }

        public override Vector2 GetSpawnPoint()
        {
            return new Vector2(0, 500);
        }

        protected override void BuildLevel()
        {
            // Main floor (16 tiles)
            for (int i = 0; i < 16; i++)
            {
                AddFloor(i * 80, 640);
            }

            // Elevated floors
            AddFloor(1280, 480);
            AddFloor(1600, 480);

            // Small floor section
            for (int i = 1; i < 5; i++)
            {
                AddFloor(i * 80 + 1600, 560);
            }

            // Vertical wall
            for (int i = 1; i < 6; i++)
            {
                AddFloor(1760, i * 80);
            }

            // Upper floors
            //AddFloor(1920, 180);
            AddFloor(1920, 354);

            // Tall wall
            for (int i = 1; i < 8; i++)
            {
                AddFloor(3000, i * 80);
            }

            // Upper wall section
            for (int i = -4; i < 4; i++)
            {
                AddFloor(3000, i * 80);
            }

            // Final floor section
            for (int i = 0; i < 10; i++)
            {
                AddFloor(i * 80 + 3008, 804);
            }

            // Platform 1
            AddPlatform(new Vector2(500, 450), new Rectangle(0, 13, 150, 30));

            // Platform 2 (wide)
            AddPlatform(new Vector2(2100, 160), new Rectangle(0, 13, 150, 30), scaleX: 6f);

            // Wall spikes (rotated 90 degrees)
            for (int i = 1; i < 7; i++)
            {
                AddSpike(new Vector2(1840, i * 64 + 20), rotationDegrees: 90f);
            }

            // Enemy
            AddSkeletonEnemy(new Vector2(900, 500));

            // Level end statue
            AddLevelEnd(3600, 680, 128, 128);
        }
    }
}