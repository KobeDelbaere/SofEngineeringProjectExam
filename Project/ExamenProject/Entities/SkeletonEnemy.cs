using ExamenProject.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities
{
    /// <summary>
    /// Skeleton Enemy - Walking enemy
    /// </summary>
    public class SkeletonEnemy : EnemyBase
    {
        private const double AnimationSpeed = 120;

        public SkeletonEnemy(Texture2D walkTex, Texture2D deathTex, Vector2 startPos)
            : base(startPos)
        {
            BaseFrameWidth = 22;
            BaseFrameHeight = 33;
            Health = 1;
            Scale = 2.5f;
            Speed = 1.5f;

            // WALK ANIMATION
            SpriteAnimation walkAnim = new SpriteAnimation(walkTex)
            {
                FrameTime = AnimationSpeed,
                Loop = true
            };

            // 13 frames in een horizontale rij
            for (int i = 0; i < 13; i++)
            {
                walkAnim.AddFrame(new Rectangle(i * 22, 0, 22, 33));
            }

            // DEATH ANIMATION
            SpriteAnimation deathAnim = new SpriteAnimation(deathTex)
            {
                FrameTime = AnimationSpeed,
                Loop = false
            };

            // 15 frames
            for (int i = 0; i < 15; i++)
            {
                deathAnim.AddFrame(new Rectangle(i * 33, 0, 33, 32));
            }

            animManager.AddAnimation(AnimationState.Run, walkAnim);
            animManager.AddAnimation(AnimationState.Death, deathAnim);
            animManager.SetState(AnimationState.Run);
        }

        // Skeleton gebruikt default behavior van EnemyBase
        // Geen override nodig - loopt gewoon heen en weer
    }
}