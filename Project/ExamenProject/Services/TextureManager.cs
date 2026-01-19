using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ExamenProject.Services
{
    /// <summary>
    /// TextureManager - Centralized texture loading and caching
    /// Volgt Single Responsibility Principle - alleen texture management
    /// Volgt Singleton Pattern - één centrale texture cache
    /// </summary>
    public class TextureManager
    {
        private ContentManager content;
        private GraphicsDevice graphicsDevice;
        private Dictionary<string, Texture2D> textureCache;

        // Hero textures
        public Texture2D HeroIdle { get; private set; }
        public Texture2D HeroRun { get; private set; }
        public Texture2D HeroJump { get; private set; }
        public Texture2D HeroFall { get; private set; }
        public Texture2D HeroAttack { get; private set; }

        // Level environment textures
        public Texture2D Floor { get; private set; }
        public Texture2D Platform { get; private set; }
        public Texture2D Spikes { get; private set; }
        public Texture2D Statue { get; private set; }
        public Texture2D Background { get; private set; }

        // Enemy textures
        public Texture2D EnemyWalk { get; private set; }
        public Texture2D EnemyDeath { get; private set; }
        public Texture2D BallistaShoot { get; private set; }
        public Texture2D BallistaDestroyed { get; private set; }
        public Texture2D Arrow { get; private set; }

        // UI textures
        public Texture2D MenuBackground { get; private set; }
        public Texture2D GameOverBackground { get; private set; }
        public Texture2D Pixel { get; private set; }

        public TextureManager(ContentManager contentManager, GraphicsDevice device)
        {
            content = contentManager;
            graphicsDevice = device;
            textureCache = new Dictionary<string, Texture2D>();
        }

        /// <summary>
        /// Load all game textures
        /// </summary>
        public void LoadAllTextures()
        {
            LoadHeroTextures();
            LoadLevelTextures();
            LoadEnemyTextures();
            LoadUITextures();
        }

        /// <summary>
        /// Load hero animation textures
        /// </summary>
        private void LoadHeroTextures()
        {
            HeroIdle = LoadTexture("_Idle");
            HeroRun = LoadTexture("_Run");
            HeroJump = LoadTexture("_Jump");
            HeroFall = LoadTexture("_Fall");
            HeroAttack = LoadTexture("_Attack");
        }

        /// <summary>
        /// Load level environment textures
        /// </summary>
        private void LoadLevelTextures()
        {
            Floor = LoadTexture("woodfl");
            Platform = LoadTexture("platform");
            Spikes = LoadTexture("16-bit-spike-Sheet");
            Statue = LoadTexture("15");
            Background = LoadTexture("jungle parallax background");
        }

        /// <summary>
        /// Load enemy textures
        /// </summary>
        private void LoadEnemyTextures()
        {
            EnemyWalk = LoadTexture("Skeleton Walk");
            EnemyDeath = LoadTexture("Skeleton Dead");
            BallistaShoot = LoadTexture("Balista");
            BallistaDestroyed = LoadTexture("DestroyedBalista");
            Arrow = LoadTexture("Arrow");
        }

        /// <summary>
        /// Load UI textures
        /// </summary>
        private void LoadUITextures()
        {
            MenuBackground = LoadTextureSafe("StartScreen");
            GameOverBackground = LoadTextureSafe("EndScreen");

            // Create pixel texture - CRITICAL: moet Color[] array zijn
            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// Load texture with caching
        /// Volgt DRY principle - één centrale load methode
        /// </summary>
        private Texture2D LoadTexture(string assetName)
        {
            if (textureCache.ContainsKey(assetName))
                return textureCache[assetName];

            var texture = content.Load<Texture2D>(assetName);
            textureCache[assetName] = texture;
            return texture;
        }

        /// <summary>
        /// Load texture safely (returns null if not found)
        /// Gebruikt voor optionele textures zoals UI backgrounds
        /// </summary>
        private Texture2D LoadTextureSafe(string assetName)
        {
            try
            {
                return LoadTexture(assetName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get texture by name (voor custom loading)
        /// </summary>
        public Texture2D GetTexture(string assetName)
        {
            return LoadTexture(assetName);
        }

        /// <summary>
        /// Clear texture cache (voor memory management)
        /// </summary>
        public void UnloadTextures()
        {
            textureCache.Clear();
        }
    }
}