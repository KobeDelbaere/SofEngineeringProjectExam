using ExamenProject.Entities;
using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using ExamenProject.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ExamenProject.Services
{
    /// <summary>
    /// LevelManager - Centralized level management
    /// Volgt Single Responsibility Principle - alleen level management
    /// Volgt Dependency Inversion Principle - werkt met ILevel interface
    /// </summary>
    public class LevelManager
    {
        private List<ILevel> levels;
        private int currentLevelIndex;
        private ILevel currentLevel;
        private CollisionService collision;
        private TextureManager textures;

        public ILevel CurrentLevel => currentLevel;
        public int CurrentLevelIndex => currentLevelIndex;
        public bool HasNextLevel => currentLevelIndex + 1 < levels.Count;

        public LevelManager(CollisionService collisionService, TextureManager textureManager)
        {
            collision = collisionService;
            textures = textureManager;
            levels = new List<ILevel>();
            currentLevelIndex = 0;
        }

        /// <summary>
        /// Initialize alle levels met TextureManager
        /// </summary>
        public void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            levels.Clear();

            // Pass TextureManager naar elke level
            levels.Add(new Level1(content, graphicsDevice, collision, textures));
            levels.Add(new Level2(content, graphicsDevice, collision, textures));
            levels.Add(new Level3(content, graphicsDevice, collision, textures));
        }

        /// <summary>
        /// Load all level content
        /// </summary>
        public void LoadContent()
        {
            foreach (var level in levels)
            {
                level.LoadLevel();
            }
        }

        /// <summary>
        /// Load specific level by index
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levels.Count)
                return;

            currentLevelIndex = levelIndex;
            currentLevel = levels[currentLevelIndex];
            currentLevel.LoadLevel(); // Reset level
        }

        /// <summary>
        /// Load first level
        /// </summary>
        public void LoadFirstLevel()
        {
            LoadLevel(0);
        }

        /// <summary>
        /// Reload current level (voor restart)
        /// </summary>
        public void ReloadCurrentLevel()
        {
            LoadLevel(currentLevelIndex);
        }

        /// <summary>
        /// Load next level
        /// </summary>
        public bool LoadNextLevel()
        {
            if (!HasNextLevel)
                return false;

            LoadLevel(currentLevelIndex + 1);
            return true;
        }

        /// <summary>
        /// Get spawn point van current level
        /// </summary>
        public Vector2 GetSpawnPoint()
        {
            return currentLevel?.GetSpawnPoint() ?? Vector2.Zero;
        }

        /// <summary>
        /// Update current level - ALLE game logic
        /// Volgt Single Responsibility Principle
        /// </summary>
        public void Update(GameTime gameTime, Hero hero)
        {
            if (currentLevel == null)
                return;

            // Update level entities
            currentLevel.Update(gameTime, hero);

            // Update collision detection
            UpdateCollisions(hero);

            // Update combat
            UpdateCombat(hero);
        }

        /// <summary>
        /// Update all collision detection
        /// Volgt Single Responsibility Principle
        /// </summary>
        private void UpdateCollisions(Hero hero)
        {
            if (!(currentLevel is BaseLevel baseLevel))
                return;

            // Hero collision met floors/platforms
            collision.ResolveFloors(hero, baseLevel.GetFloors());
            collision.ResolvePlatforms(hero, baseLevel.GetPlatforms());

            // Enemy collision met environment
            foreach (var enemy in baseLevel.GetEnemies())
            {
                enemy.ApplyFloorCollision(baseLevel.GetFloors());
                enemy.CheckEdges(baseLevel.GetFloors());
                enemy.CheckWalls(baseLevel.GetPlatforms());

                // Ballista projectile collisions
                if (enemy is BallistaEnemy ballista)
                {
                    var obstacles = new List<IGameObject>();
                    obstacles.AddRange(baseLevel.GetFloors());
                    obstacles.AddRange(baseLevel.GetPlatforms());
                    ballista.CheckProjectileCollisions(obstacles);
                }
            }

            // Level end collision
            collision.ResolveLevelEnd(hero, baseLevel.GetLevelEnds());
        }

        /// <summary>
        /// Update combat - hero attacks en enemy collisions
        /// Volgt Single Responsibility Principle
        /// </summary>
        private void UpdateCombat(Hero hero)
        {
            if (!(currentLevel is BaseLevel baseLevel))
                return;

            // Hero attack hits enemies
            if (hero.IsAttacking && hero.AttackBounds != Rectangle.Empty)
            {
                foreach (var enemy in baseLevel.GetEnemies())
                {
                    if (!enemy.IsDead && hero.AttackBounds.Intersects(enemy.Bounds))
                    {
                        enemy.TakeDamage(1);
                    }
                }
            }

            // Enemy collision met hero (alleen levende enemies)
            foreach (var enemy in baseLevel.GetEnemies())
            {
                if (!enemy.IsDead && hero.Bounds.Intersects(enemy.Bounds))
                {
                    enemy.OnHit(hero);
                    break;
                }
            }
        }

        /// <summary>
        /// Check if hero should die
        /// Returns true als hero geraakt is
        /// </summary>
        public bool CheckHeroDeath(Hero hero)
        {
            if (!(currentLevel is BaseLevel baseLevel))
                return false;

            // Check hazards (spikes, enemies)
            foreach (var hazard in baseLevel.GetHazards())
            {
                if (hazard is EnemyBase enemy && enemy.IsDead)
                    continue;

                if (hero.Bounds.Intersects(hazard.Bounds))
                {
                    return true;
                }
            }

            // Check ballista projectiles
            foreach (var enemy in baseLevel.GetEnemies())
            {
                if (enemy is BallistaEnemy ballista)
                {
                    foreach (var projectile in ballista.GetProjectiles())
                    {
                        if (hero.Bounds.Intersects(projectile.Bounds))
                        {
                            return true;
                        }
                    }
                }
            }

            // Check fall death
            if (hero.Position.Y > 1000)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if level is complete
        /// </summary>
        public bool IsLevelComplete()
        {
            return currentLevel?.IsLevelComplete ?? false;
        }

        /// <summary>
        /// Draw current level
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            currentLevel?.Draw(spriteBatch);
        }
    }
}