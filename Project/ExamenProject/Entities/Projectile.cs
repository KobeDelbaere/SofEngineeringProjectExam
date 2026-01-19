using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ExamenProject.Entities
{
    /// <summary>
    /// Projectile - Simple moving hazard (like a moving spike)
    /// Volgt Single Responsibility Principle - alleen projectile movement en rendering
    /// Volgt Liskov Substitution Principle - kan vervangen worden door IHazard
    /// </summary>
    public class Projectile : IGameObject, IHazard
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float rotation;
        private float scale;
        private float elapsed;
        private float lifetime;
        private bool hasHit; // Track of projectile iets heeft geraakt

        public bool IsExpired => elapsed >= lifetime || hasHit;

        public Rectangle Bounds =>
            new Rectangle(
                (int)position.X - 4,
                (int)position.Y - 4,
                8,
                8
            );

        /// <summary>
        /// Constructor - Dependency Injection van texture
        /// </summary>
        public Projectile(
            Texture2D texture,
            Vector2 startPosition,
            Vector2 direction,
            float speed,
            float lifetime,
            float scale = 0.2f)
        {
            this.texture = texture;
            this.position = startPosition;
            this.velocity = direction * speed;
            this.lifetime = lifetime;
            this.scale = scale;
            this.elapsed = 0f;

            // Rotatie gebaseerd op movement direction
            this.rotation = (float)Math.Atan2(direction.Y, direction.X);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsed += deltaTime;

            // Update position
            position += velocity * deltaTime * 60f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture == null)
                return;

            spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                rotation,
                new Vector2(texture.Width / 2, texture.Height / 2),
                scale,
                SpriteEffects.None,
                0f
            );
        }

        /// <summary>
        /// IHazard implementation - doe niets, death wordt afgehandeld in GamePlayScreen
        /// Volgt Interface Segregation Principle - alleen noodzakelijke implementatie
        /// </summary>
        public void OnHit(Hero hero)
        {
            // Mark als geraakt zodat het verdwijnt
            hasHit = true;
            // Empty - GamePlayScreen handelt death af
        }

        /// <summary>
        /// Mark projectile als geraakt door een obstacle
        /// Volgt Single Responsibility Principle
        /// </summary>
        public void MarkAsHit()
        {
            hasHit = true;
        }
    }
}