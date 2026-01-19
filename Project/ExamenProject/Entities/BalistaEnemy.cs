using ExamenProject.Animation;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ExamenProject.Entities
{
    /// <summary>
    /// Ballista Enemy - Stationary turret that shoots projectiles
    /// Volgt Single Responsibility Principle - alleen ballista behavior
    /// Volgt Open/Closed Principle - uitbreidbaar via inheritance
    /// Volgt Dependency Inversion Principle - afhankelijk van IHazard abstractie
    /// </summary>
    public class BallistaEnemy : EnemyBase
    {
        private Texture2D projectileTexture;
        private List<Projectile> projectiles;
        private Vector2 shootDirection;
        private float rotation;
        private Vector2 origin;

        private float shootTimer;
        private float shootStartDelay = 2.4f; // vertraging in seconden voor eerste shot
        private bool firstShot = true;

        // Configuration constants - makkelijk aan te passen
        private const float ShootInterval = 3.0f;
        private const double AnimationSpeed = 100;
        private const float ProjectileSpeed = 6f;
        private const float ProjectileLifetime = 20f;
        private const float ProjectileScale = 0.2f;

        public BallistaEnemy(
            Texture2D shootTexture,
            Texture2D destroyedTexture,
            Texture2D projectileTexture,
            Vector2 position,
            Vector2 direction,
            float rotationDegrees = 0f)
            : base(position)
        {
            // Dependency Injection van textures
            this.projectileTexture = projectileTexture;
            this.projectiles = new List<Projectile>();

            // Configuration
            ConfigureAsStationary();
            ConfigureAnimations(shootTexture, destroyedTexture);
            ConfigureShooting(direction, rotationDegrees);

            // Rotatie setup
            this.rotation = MathHelper.ToRadians(rotationDegrees);
            this.origin = rotationDegrees != 0
                ? new Vector2(BaseFrameWidth / 2f, BaseFrameHeight / 2f)
                : Vector2.Zero;

            if (rotationDegrees != 0)
            {
                Position += origin * Scale;
            }

        }

        /// <summary>
        /// Configure ballista als stationary enemy
        /// Volgt Single Responsibility Principle - aparte configuratie methode
        /// </summary>
        private void ConfigureAsStationary()
        {
            isStationary = true;
            Speed = 0f;
            Gravity = 0f;
            MaxFall = 0f;
            Scale = 2f;
            BaseFrameWidth = 128;
            BaseFrameHeight = 128;
        }

        /// <summary>
        /// Configure animations
        /// </summary>
        private void ConfigureAnimations(Texture2D shootTex, Texture2D destroyedTex)
        {
            // Shoot animation
            var shootAnim = new SpriteAnimation(shootTex)
            {
                FrameTime = 495,
                Loop = true
            };

            for (int i = 0; i < 6; i++)
            {
                int col = i % 2;
                int row = i / 2;
                shootAnim.AddFrame(new Rectangle(col * 128, row * 128, 128, 128));
            }

            // Destroyed animation
            var destroyedAnim = new SpriteAnimation(destroyedTex)
            {
                FrameTime = AnimationSpeed,
                Loop = false
            };
            destroyedAnim.AddFrame(new Rectangle(0, 0, 128, 128));

            animManager.AddAnimation(AnimationState.Run, shootAnim);
            animManager.AddAnimation(AnimationState.Death, destroyedAnim);
            animManager.SetState(AnimationState.Run);
        }

        /// <summary>
        /// Configure shooting direction
        /// </summary>
        private void ConfigureShooting(Vector2 direction, float rotationDegrees)
        {
            shootDirection = Vector2.Normalize(direction);

            if (rotationDegrees == 0)
            {
                flip = shootDirection.X < 0
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                int width = (int)(BaseFrameWidth * Scale);
                int height = (int)(BaseFrameHeight * Scale);

                // Swap dimensions voor 90/270 graden rotatie
                float degrees = MathHelper.ToDegrees(rotation) % 360;
                bool isVertical = degrees == 90 || degrees == 270 ||
                                  degrees == -90 || degrees == -270;

                return new Rectangle(
                    (int)(Position.X - origin.X * Scale),
                    (int)(Position.Y - origin.Y * Scale),
                    isVertical ? height : width,
                    isVertical ? width : height
                );
            }
        }

        /// <summary>
        /// Update behavior - shooting and projectile management
        /// Volgt Single Responsibility Principle
        /// </summary>
        protected override void UpdateBehavior(GameTime gameTime)
        {
            UpdateProjectiles(gameTime);

            if (!IsDead)
            {
                UpdateShooting(gameTime);
            }
            else if (IsDead) 
            {
                projectiles.Clear();
            }
        }

        /// <summary>
        /// Update all projectiles - Volgt SRP
        /// </summary>
        private void UpdateProjectiles(GameTime gameTime)
        {
            // Update en cleanup in één loop (efficiency)
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update(gameTime);

                if (projectiles[i].IsExpired)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Update shooting timer - Volgt SRP
        /// </summary>
        private void UpdateShooting(GameTime gameTime)
        {
            shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Als het de eerste keer is, gebruik de start delay
            float interval = firstShot ? shootStartDelay : ShootInterval;

            if (shootTimer >= interval)
            {
                Shoot();
                shootTimer = 0f;
                firstShot = false; // de eerste vertraging is nu voorbij
            }
        }

        protected override void UpdateDead(GameTime gameTime)
        {
            base.UpdateDead(gameTime);

            // Update projectiles ook tijdens death animatie
            UpdateProjectiles(gameTime);
        }

        /// <summary>
        /// Create and fire projectile
        /// Volgt Dependency Inversion Principle - gebruikt IHazard abstractie
        /// </summary>
        private void Shoot()
        {
            Vector2 spawnPosition = CalculateSpawnPosition();

            var projectile = new Projectile(
                projectileTexture,
                spawnPosition,
                shootDirection,
                ProjectileSpeed,
                ProjectileLifetime,
                ProjectileScale
            );

            projectiles.Add(projectile);
        }

        /// <summary>
        /// Calculate waar projectile moet spawnen
        /// </summary>
        private Vector2 CalculateSpawnPosition()
        {
            return Position;// + shootOffset;
        }

        /// <summary>
        /// Get all active projectiles as IHazard
        /// Volgt Interface Segregation Principle - return interface, niet implementatie
        /// </summary>
        public IEnumerable<IHazard> GetProjectiles()
        {
            return projectiles.Cast<IHazard>();
        }

        /// <summary>
        /// Check projectile collisions met obstacles (floors, platforms)
        /// Volgt Single Responsibility Principle
        /// </summary>
        public void CheckProjectileCollisions(List<IGameObject> obstacles)
        {
            foreach (var projectile in projectiles)
            {
                foreach (var obstacle in obstacles)
                {
                    if (projectile.Bounds.Intersects(obstacle.Bounds))
                    {
                        projectile.MarkAsHit();
                        break; // Stop checking deze projectile
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsDead && animManager.IsAnimationFinished)
                return;

            DrawBallista(spriteBatch);
            DrawProjectiles(spriteBatch);
        }

        /// <summary>
        /// Draw ballista sprite - Volgt SRP
        /// </summary>
        private void DrawBallista(SpriteBatch spriteBatch)
        {
            var currentFrame = animManager.Current.Current;

            spriteBatch.Draw(
                animManager.Current.Texture,
                Position,
                currentFrame.Source,
                Color.White,
                rotation,
                origin,
                Scale,
                flip,
                0f
            );
        }

        /// <summary>
        /// Draw all projectiles - Volgt SRP
        /// </summary>
        private void DrawProjectiles(SpriteBatch spriteBatch)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}