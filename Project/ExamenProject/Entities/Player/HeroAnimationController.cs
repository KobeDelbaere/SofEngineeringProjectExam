using ExamenProject.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities.Player
{
    /// <summary>
    /// Hero Animation Controller - Manages all hero animations
    /// Volgt Single Responsibility Principle - alleen animation logic
    /// </summary>
    public class HeroAnimationController
    {
        private readonly AnimationManager animManager;
        private const double AnimationSpeed = 100;
        private const int BaseFrameWidth = 28;

        public SpriteEffects CurrentFlip { get; private set; }
        public bool IsAnimationFinished => animManager.IsAnimationFinished;

        public HeroAnimationController(
            Texture2D idleTexture,
            Texture2D runTexture,
            Texture2D jumpTexture,
            Texture2D fallTexture,
            Texture2D attackTexture)
        {
            animManager = new AnimationManager();
            CurrentFlip = SpriteEffects.None;

            InitializeAnimations(
                idleTexture,
                runTexture,
                jumpTexture,
                fallTexture,
                attackTexture
            );
        }

        /// <summary>
        /// Initialize all animations
        /// </summary>
        private void InitializeAnimations(
            Texture2D idle,
            Texture2D run,
            Texture2D jump,
            Texture2D fall,
            Texture2D attack)
        {
            animManager.AddAnimation(AnimationState.Idle, CreateLoopingAnimation(idle, 10));
            animManager.AddAnimation(AnimationState.Run, CreateLoopingAnimation(run, 10));
            animManager.AddAnimation(AnimationState.Jump, CreateLoopingAnimation(jump, 3));
            animManager.AddAnimation(AnimationState.Fall, CreateLoopingAnimation(fall, 3));
            animManager.AddAnimation(AnimationState.Attack, CreateAttackAnimation(attack));

            animManager.SetState(AnimationState.Idle);
        }

        /// <summary>
        /// Create looping animation
        /// </summary>
        private SpriteAnimation CreateLoopingAnimation(Texture2D texture, int frameCount)
        {
            var animation = new SpriteAnimation(texture)
            {
                FrameTime = AnimationSpeed,
                Loop = true
            };

            for (int i = 0; i < frameCount; i++)
            {
                animation.AddFrame(new Rectangle(43 + i * 120, 0, 28, 80));
            }

            return animation;
        }

        /// <summary>
        /// Create attack animation (non-looping, wider frames)
        /// </summary>
        private SpriteAnimation CreateAttackAnimation(Texture2D texture)
        {
            var animation = new SpriteAnimation(texture)
            {
                FrameTime = 80,
                Loop = false
            };

            for (int i = 0; i < 4; i++)
            {
                animation.AddFrame(new Rectangle(43 + i * 120, 0, 120, 80));
            }

            return animation;
        }

        /// <summary>
        /// Update animation state based on velocity and input
        /// </summary>
        public void UpdateState(Vector2 velocity, float horizontal, bool isAttacking)
        {
            if (isAttacking)
            {
                animManager.SetState(AnimationState.Attack);
            }
            else if (velocity.Y < 0)
            {
                animManager.SetState(AnimationState.Jump);
            }
            else if (velocity.Y > 0)
            {
                animManager.SetState(AnimationState.Fall);
            }
            else if (horizontal != 0)
            {
                animManager.SetState(AnimationState.Run);
            }
            else
            {
                animManager.SetState(AnimationState.Idle);
            }
        }

        /// <summary>
        /// Update sprite direction based on input
        /// </summary>
        public void UpdateDirection(float horizontal)
        {
            if (horizontal < 0)
            {
                CurrentFlip = SpriteEffects.FlipHorizontally;
            }
            else if (horizontal > 0)
            {
                CurrentFlip = SpriteEffects.None;
            }
        }

        /// <summary>
        /// Update animation
        /// </summary>
        public void Update(GameTime gameTime)
        {
            animManager.Update(gameTime);
        }

        public void SetAttackState()
        {
            animManager.SetState(AnimationState.Attack);
        }


        /// <summary>
        /// Draw animation
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            animManager.Draw(spriteBatch, position, CurrentFlip, BaseFrameWidth);
        }
    }
}