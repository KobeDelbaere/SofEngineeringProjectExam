using ExamenProject.Animation;
using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ExamenProject.Entities
{
    /// <summary>
    /// EnemyBase - Abstract base class for all enemy types
    /// 
    /// SOLID Principles Applied:
    /// - Single Responsibility Principle: Provides common enemy behavior (movement, health, collision).
    ///   Specific enemy types add their own behavior through inheritance.
    /// - Open/Closed Principle: Open for extension (derived classes like SkeletonEnemy, BallistaEnemy)
    ///   but closed for modification. New enemy types don't require changing this base class.
    /// - Liskov Substitution Principle: Any derived enemy can be used wherever EnemyBase is expected.
    /// - Interface Segregation Principle: Implements IGameObject (rendering) and IHazard (damage).
    /// - Dependency Inversion Principle: Depends on abstractions (IGameObject) not concrete classes.
    /// 
    /// Design Patterns:
    /// - Template Method Pattern: Update() defines the update flow, delegates to virtual methods
    ///   that subclasses can override (UpdateBehavior, UpdateMovement, OnDeath).
    /// - Strategy Pattern: Different enemy types implement different behaviors via inheritance.
    /// </summary>
    public abstract class EnemyBase : IGameObject, IHazard
    {
        // ====================================
        // Public Properties
        // ====================================

        /// <summary>
        /// Current position in world space
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Current velocity (movement per frame)
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Current health points
        /// </summary>
        public int Health { get; protected set; } = 1;

        /// <summary>
        /// Whether the enemy is dead (health <= 0)
        /// </summary>
        public bool IsDead => Health <= 0;

        /// <summary>
        /// Collision bounds for this enemy
        /// Open/Closed Principle: Can be overridden by derived classes for custom hitboxes
        /// </summary>
        public virtual Rectangle Bounds =>
            new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)(BaseFrameWidth * Scale),
                (int)(BaseFrameHeight * Scale)
            );

        // ====================================
        // Protected Configuration
        // Single Responsibility: Centralized configuration
        // ====================================

        protected float Scale = 3f;
        protected float Gravity = 0.6f;
        protected float MaxFall = 12f;
        protected float Speed = 1.5f;
        protected int BaseFrameWidth;
        protected int BaseFrameHeight;

        // ====================================
        // Protected State
        // ====================================

        protected bool grounded = false;
        protected int direction = 1; // 1 = right, -1 = left
        protected AnimationManager animManager;
        protected SpriteEffects flip = SpriteEffects.None;
        protected double hitCooldown = 0;
        protected const double HitCooldownTime = 500;
        protected bool isStationary = false; // For turret-type enemies like ballista

        // ====================================
        // Constructor
        // ====================================

        /// <summary>
        /// Initialize enemy at starting position
        /// Template Method Pattern: Provides base initialization
        /// </summary>
        protected EnemyBase(Vector2 startPos)
        {
            Position = startPos;
            animManager = new AnimationManager();
        }

        // ====================================
        // Main Update Loop - Template Method Pattern
        // ====================================

        /// <summary>
        /// Main update method - Template Method Pattern
        /// Defines the update flow, delegates to virtual methods that subclasses can override
        /// 
        /// SOLID Principle: Open/Closed - subclasses extend behavior without modifying this method
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // Update timers
            if (hitCooldown > 0)
                hitCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

            // Dead enemies only update death animation
            if (IsDead)
            {
                UpdateDead(gameTime);
                return;
            }

            // Custom behavior (override in subclasses)
            UpdateBehavior(gameTime);

            // Standard movement (skip for stationary enemies)
            if (!isStationary)
            {
                UpdateMovement(gameTime);
            }

            // Update animation
            animManager.Update(gameTime);
        }

        /// <summary>
        /// Update custom enemy behavior
        /// Template Method Pattern: Override point for specific enemy types
        /// 
        /// SOLID Principle: Open/Closed - new behavior without modifying base class
        /// </summary>
        protected virtual void UpdateBehavior(GameTime gameTime)
        {
            // Default: no custom behavior (walking enemies use default movement)
        }

        /// <summary>
        /// Update standard walking movement with gravity
        /// Single Responsibility Principle: Only handles movement physics
        /// 
        /// Can be overridden for enemies with different movement patterns
        /// </summary>
        protected virtual void UpdateMovement(GameTime gameTime)
        {
            // Horizontal movement
            Velocity = new Vector2(Speed * direction, Velocity.Y);

            // Apply gravity
            float newVelY = Velocity.Y + Gravity;
            if (newVelY > MaxFall)
                newVelY = MaxFall;

            Velocity = new Vector2(Velocity.X, newVelY);
            Position += Velocity;

            // Update sprite flip based on direction
            flip = (direction < 0)
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }

        /// <summary>
        /// Update death animation
        /// Single Responsibility Principle: Handles only death-related updates
        /// </summary>
        protected virtual void UpdateDead(GameTime gameTime)
        {
            animManager.Update(gameTime);
            // Dead enemies remain in place (no gravity/movement)
        }

        // ====================================
        // Collision Detection
        // ====================================

        /// <summary>
        /// Apply floor collision detection and resolution
        /// Single Responsibility Principle: Only handles floor collision
        /// 
        /// Dependency Inversion Principle: Depends on IGameObject abstraction
        /// </summary>
        public virtual void ApplyFloorCollision(List<IGameObject> floors)
        {
            if (isStationary)
                return;

            grounded = false;
            Rectangle nextY = Bounds;
            nextY.Y += (int)Velocity.Y;

            foreach (var floor in floors)
            {
                if (nextY.Intersects(floor.Bounds) && Velocity.Y > 0)
                {
                    Position = new Vector2(Position.X, floor.Bounds.Top - Bounds.Height);
                    Velocity = new Vector2(Velocity.X, 0);
                    grounded = true;
                }
            }
        }

        /// <summary>
        /// Check for edges and reverse direction to prevent falling
        /// Single Responsibility Principle: Only handles edge detection
        /// </summary>
        public virtual void CheckEdges(List<IGameObject> floors)
        {
            if (isStationary || !grounded)
                return;

            int lookAhead = 10;
            Rectangle checkPoint = new Rectangle(
                Bounds.X + (direction > 0 ? Bounds.Width + lookAhead : -lookAhead),
                Bounds.Bottom + 5,
                5,
                10
            );

            bool foundFloor = false;
            foreach (var floor in floors)
            {
                if (checkPoint.Intersects(floor.Bounds))
                {
                    foundFloor = true;
                    break;
                }
            }

            if (!foundFloor)
                ReverseDirection();
        }

        /// <summary>
        /// Check for wall collisions and reverse direction
        /// Single Responsibility Principle: Only handles wall collision
        /// </summary>
        public virtual void CheckWalls(List<IGameObject> platforms)
        {
            if (isStationary)
                return;

            Rectangle nextX = Bounds;
            nextX.X += (int)(Velocity.X * 2);

            foreach (var platform in platforms)
            {
                if (nextX.Intersects(platform.Bounds))
                {
                    ReverseDirection();
                    break;
                }
            }
        }

        /// <summary>
        /// Reverse movement direction
        /// Single Responsibility Principle: Only handles direction change
        /// </summary>
        protected virtual void ReverseDirection()
        {
            direction *= -1;
        }

        // ====================================
        // Combat System
        // ====================================

        /// <summary>
        /// Apply damage to enemy
        /// Single Responsibility Principle: Only handles damage logic
        /// 
        /// Includes cooldown to prevent spam damage
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (hitCooldown > 0 || IsDead)
                return;

            Health -= damage;
            hitCooldown = HitCooldownTime;

            if (Health <= 0)
            {
                Health = 0;
                Velocity = Vector2.Zero;
                OnDeath();
            }
        }

        /// <summary>
        /// Handle death - play death animation
        /// Template Method Pattern: Can be overridden for custom death behavior
        /// 
        /// Open/Closed Principle: Subclasses can extend death behavior
        /// </summary>
        protected virtual void OnDeath()
        {
            animManager.SetState(AnimationState.Death);
        }

        /// <summary>
        /// IHazard implementation - what happens when enemy hits hero
        /// Interface Segregation Principle: Implements only necessary IHazard method
        /// 
        /// Default: do nothing (damage is handled in LevelManager)
        /// Can be overridden for enemies with special collision effects
        /// </summary>
        public virtual void OnHit(Hero hero)
        {
            // Empty by default - damage handled in LevelManager
        }

        // ====================================
        // Rendering
        // ====================================

        /// <summary>
        /// Draw enemy sprite
        /// Single Responsibility Principle: Only handles rendering
        /// 
        /// Delegates to AnimationManager for actual drawing
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            animManager.Draw(spriteBatch, Position, flip, BaseFrameWidth, Scale);
        }
    }
}