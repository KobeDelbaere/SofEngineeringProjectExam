using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities.Player
{
    /// <summary>
    /// Hero - Player character (Component-Based Architecture)
    /// Volgt Single Responsibility Principle - alleen hero coordination
    /// Volgt Dependency Inversion Principle - afhankelijk van components
    /// Volgt Composition over Inheritance
    /// </summary>
    public class Hero : IGameObject
    {
        // Position
        public Vector2 Position { get; set; }

        // Components - Composition over Inheritance
        private readonly HeroPhysics physics;
        private readonly HeroAnimationController animations;
        private readonly HeroCombat combat;
        private readonly IInputService input;

        // State
        public bool DropDown { get; private set; }
        public bool IsGodMode { get; private set; }

        // Delegated Properties - Facade Pattern
        public Vector2 Velocity
        {
            get => physics.Velocity;
            set => physics.Velocity = value;
        }

        public bool IsGrounded
        {
            get => physics.IsGrounded;
            set => physics.IsGrounded = value;
        }

        public bool IsAttacking => combat.IsAttacking;

        public Rectangle Bounds =>
            new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                HeroConfig.HitboxWidth,
                HeroConfig.HitboxHeight
            );

        public Rectangle AttackBounds =>
            combat.GetAttackBounds(Position, animations.CurrentFlip);

        /// <summary>
        /// Constructor - Dependency Injection
        /// </summary>
        public Hero(
            Texture2D idleTexture,
            Texture2D runTexture,
            Texture2D jumpTexture,
            Texture2D fallTexture,
            Texture2D attackTexture,
            IInputService inputService)
        {
            // Initialize components
            physics = new HeroPhysics();
            animations = new HeroAnimationController(
                idleTexture,
                runTexture,
                jumpTexture,
                fallTexture,
                attackTexture
            );
            combat = new HeroCombat();
            input = inputService;

            // Initial state
            Position = Vector2.Zero;
            DropDown = false;
            IsGodMode = false;
        }

        /// <summary>
        /// Main update - Coordinates all components
        /// Volgt Single Responsibility - alleen coordination
        /// </summary>
        public void Update(GameTime gameTime)
        {
            HandleInput();

            if (combat.IsAttacking)
            {
                UpdateAttackState(gameTime);
                return;
            }

            UpdateNormalState(gameTime);
        }

        /// <summary>
        /// Handle all input
        /// </summary>
        private void HandleInput()
        {
            // Attack input
            if (input.AttackPressed && !combat.IsAttacking)
            {
                combat.StartAttack();
                animations.SetAttackState();  // ← Use dedicated method!
            }


            // Jump input
            if (input.JumpPressed)
            {
                physics.Jump();
            }

            // Drop input
            DropDown = input.DropPressed;

            // Movement input
            physics.SetHorizontalVelocity(input.Horizontal);

            // Update sprite direction
            animations.UpdateDirection(input.Horizontal);
        }

        /// <summary>
        /// Update during attack
        /// </summary>
        private void UpdateAttackState(GameTime gameTime)
        {
            animations.Update(gameTime);

            if (animations.IsAnimationFinished)
            {
                combat.EndAttack();
            }
        }

        /// <summary>
        /// Update during normal state
        /// </summary>
        private void UpdateNormalState(GameTime gameTime)
        {
            physics.UpdateGroundedState();
            animations.UpdateState(physics.Velocity, input.Horizontal, combat.IsAttacking);
            animations.Update(gameTime);
        }

        /// <summary>
        /// Apply physics
        /// </summary>
        public void ApplyPhysics()
        {
            physics.ApplyGravity();
            Position += physics.Velocity;
        }

        /// <summary>
        /// Respawn at position
        /// </summary>
        public void Respawn(Vector2 position)
        {
            Position = position;
            physics.Reset();
            combat.Reset();
        }

        /// <summary>
        /// Start attack
        /// </summary>
        public void Attack()
        {
            combat.StartAttack();
        }

        /// <summary>
        /// Enable/disable God Mode
        /// </summary>
        public void SetGodMode(bool enabled)
        {
            IsGodMode = enabled;
        }

        /// <summary>
        /// Draw hero
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(
                Position.X,
                Position.Y + HeroConfig.SpriteOffsetY
            );

            animations.Draw(spriteBatch, spritePosition);
        }

        /// <summary>
        /// Draw debug visualization
        /// </summary>
        public void DrawDebug(SpriteBatch spriteBatch, Texture2D pixel)
        {
            // Draw hitbox
            spriteBatch.Draw(pixel, Bounds, Color.Green * 0.3f);

            // Draw attack hitbox
            if (combat.IsAttacking && AttackBounds != Rectangle.Empty)
            {
                spriteBatch.Draw(pixel, AttackBounds, Color.Yellow * 0.3f);
            }
        }
    }
}