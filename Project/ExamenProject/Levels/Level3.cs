using ExamenProject.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenProject.Levels
{
    public class Level3 : BaseLevel
    {
        public Level3(
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
            // Starting area
            for (int i = -2; i < 2; i++)
            {
                AddFloor(i * 80, 640);
            }

            //for (int i = 0; i < 50; i++)
            //{
            //    AddFloor(i * 80, 720);
            //}

            for (int i = 4; i < 25; i++)
            {
                AddFloor(i * 80, 640);
            }

            for(int i = 1; i < 8; i++)
            {
                AddBallistaEnemy(new Vector2(80 + i * 240, 0), new Vector2(0, 1), rotationDegrees: 180f);
            }

            for (int i = 3; i < 10; i++) 
            {
                AddSkeletonEnemy(new Vector2(i * 160, 500));
            }

            AddPlatform(new Vector2(2000, 480), new Rectangle(0, 13, 150, 30), scaleX: 6f);

            AddSkeletonEnemy(new Vector2(2300, 380));

            AddPlatform(new Vector2(3000, 702), new Rectangle(0, 13, 150, 30), scaleX: 6f);


            AddLevelEnd(3700, 580);
        }
    }
}
