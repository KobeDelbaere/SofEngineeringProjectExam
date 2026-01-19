using ExamenProject.Core;
using ExamenProject.Entities;
using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ExamenProject.Levels
{
    /// <summary>
    /// Abstract base class voor levels
    /// Volgt Single Responsibility Principle - alleen level LAYOUT en ENTITIES
    /// Volgt Dependency Inversion Principle - afhankelijk van TextureManager
    /// </summary>
    public abstract class BaseLevel : ILevel
    {
        protected ContentManager content;
        protected GraphicsDevice graphicsDevice;
        protected CollisionService collision;
        protected TextureManager textures; // Dependency Injection

        // Collections - ALLEEN voor level entities
        protected List<IGameObject> objects = new();
        protected List<IGameObject> platforms = new();
        protected List<IGameObject> floors = new();
        protected List<IHazard> hazards = new();
        protected List<EnemyBase> enemies = new();
        protected List<ILevelEnd> levelEnds = new();

        protected Background background;
        protected bool levelComplete;

        public bool IsLevelComplete => levelComplete;

        // Getters voor LevelManager
        public List<IHazard> GetHazards() => hazards;
        public List<EnemyBase> GetEnemies() => enemies;
        public List<IGameObject> GetFloors() => floors;
        public List<IGameObject> GetPlatforms() => platforms;
        public List<ILevelEnd> GetLevelEnds() => levelEnds;

        protected BaseLevel(
            ContentManager contentManager,
            GraphicsDevice device,
            CollisionService collisionService,
            TextureManager textureManager)
        {
            content = contentManager;
            graphicsDevice = device;
            collision = collisionService;
            textures = textureManager;
            levelComplete = false;
        }

        /// <summary>
        /// Load/Reset level
        /// </summary>
        public virtual void LoadLevel()
        {
            // Clear alle lijsten
            objects.Clear();
            platforms.Clear();
            floors.Clear();
            hazards.Clear();
            enemies.Clear();
            levelEnds.Clear();
            levelComplete = false;

            // Setup background en build level
            SetupBackground();
            BuildLevel();
        }

        protected virtual void SetupBackground()
        {
            background = new Background(textures.Background, 0.15f, 1f)
            {
                OffsetY = -400
            };
        }

        // Template method - child classes implementeren level layout
        protected abstract void BuildLevel();
        public abstract Vector2 GetSpawnPoint();

        /// <summary>
        /// Update - ALLEEN entity updates en background
        /// </summary>
        public virtual void Update(GameTime gameTime, Hero hero)
        {
            // Update enemies
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
            }

            // Check level complete
            foreach (var levelEnd in levelEnds)
            {
                if (hero.Bounds.Intersects(levelEnd.Bounds))
                {
                    levelComplete = true;
                }
            }

            // Update background
            background.Update(hero.Velocity.X);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);

            foreach (var obj in objects)
            {
                obj.Draw(spriteBatch);
            }
        }

        // ====================================
        // Helper methods voor level building
        // ====================================

        protected void AddFloor(int x, int y, int width = 80, int height = 80)
        {
            var floor = new Floor(textures.Floor, x, y, width, height);
            objects.Add(floor);
            floors.Add(floor);
        }

        protected void AddPlatform(Vector2 pos, Rectangle source, float scaleX = 2f, float scaleY = 2f)
        {
            var platform = new Platform(textures.Platform, pos, source, scaleX, scaleY);
            objects.Add(platform);
            platforms.Add(platform);
        }

        protected void AddSpike(Vector2 pos, float rotationDegrees = 0f)
        {
            var spike = new Spikes(textures.Spikes, pos, rotationDegrees);
            objects.Add(spike);
            hazards.Add(spike);
        }

        protected void AddSkeletonEnemy(Vector2 pos)
        {
            var enemy = new SkeletonEnemy(textures.EnemyWalk, textures.EnemyDeath, pos);
            enemies.Add(enemy);
            objects.Add(enemy);
            hazards.Add(enemy);
        }

        protected void AddBallistaEnemy(Vector2 pos, Vector2 direction, float rotationDegrees = 0f)
        {
            var ballista = new BallistaEnemy(
                textures.BallistaShoot,
                textures.BallistaDestroyed,
                textures.Arrow,
                pos,
                direction,
                rotationDegrees
            );
            enemies.Add(ballista);
            objects.Add(ballista);
            hazards.Add(ballista);
        }

        protected void AddLevelEnd(int x, int y, int width = 128, int height = 128)
        {
            var statue = new Statue(textures.Statue, x, y, width, height);
            objects.Add(statue);
            levelEnds.Add(statue);
        }
    }
}