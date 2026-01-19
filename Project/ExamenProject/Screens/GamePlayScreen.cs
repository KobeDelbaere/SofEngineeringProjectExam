using ExamenProject.Core;
using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using ExamenProject.Levels;
using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExamenProject.Screens
{
    /// <summary>
    /// GamePlayScreen - Main game loop coordinator
    /// Volgt Single Responsibility Principle - alleen game flow
    /// Volgt Dependency Inversion Principle - gebruikt TextureManager
    /// </summary>
    public class GamePlayScreen : IScreen
    {
        private ScreenManager screenManager;
        private GraphicsDevice graphicsDevice;
        private ContentManager content;

        private Hero hero;
        private Camera2D camera;
        private IInputService input;
        private LevelManager levelManager;
        private TextureManager textures;

        // Game mode
        private bool isGodMode;
        private Vector2 lastDeathPosition;

        private KeyboardState previousKeyState;
        private bool firstStart = true;

        public GamePlayScreen(
            ScreenManager manager,
            GraphicsDevice device,
            ContentManager contentManager,
            IInputService inputService,
            TextureManager textureManager)
        {
            screenManager = manager;
            graphicsDevice = device;
            content = contentManager;
            input = inputService;
            textures = textureManager;

            camera = new Camera2D();

            // Initialize LevelManager
            var collision = new CollisionService();
            levelManager = new LevelManager(collision, textures);
        }

        public void Initialize()
        {
            isGodMode = false;
            levelManager.Initialize(content, graphicsDevice);
        }

        public void LoadContent()
        {
            // Create hero using TextureManager
            hero = new Hero(
                textures.HeroIdle,
                textures.HeroRun,
                textures.HeroJump,
                textures.HeroFall,
                textures.HeroAttack,
                input
            );
            hero.SetGodMode(isGodMode);

            // Load all levels
            levelManager.LoadContent();
        }

        // ====================================
        // MODE SELECTION
        // ====================================

        public void StartNormalMode()
        {
            isGodMode = false;
            hero?.SetGodMode(false);
            StartFirstLevel();
        }

        public void StartGodMode()
        {
            isGodMode = true;
            hero?.SetGodMode(true);
            StartFirstLevel();
        }

        private void StartFirstLevel()
        {
            levelManager.LoadFirstLevel();
            SpawnHero();
        }

        public void RestartCurrentLevel()
        {
            levelManager.ReloadCurrentLevel();
            SpawnHero();
        }

        private void SpawnHero()
        {
            hero.Respawn(levelManager.GetSpawnPoint());
            hero.Velocity = Vector2.Zero;
            hero.IsGrounded = true;
        }

        // ====================================
        // UPDATE LOOP
        // ====================================

        public void Update(GameTime gameTime)
        {
            // First start initialization
            if (firstStart)
            {
                levelManager.LoadFirstLevel();
                SpawnHero();
                firstStart = false;
            }

            // Handle input
            HandleInput();

            // Update hero
            hero.Update(gameTime);
            hero.ApplyPhysics();

            // Update level
            levelManager.Update(gameTime, hero);

            // Check death
            if (levelManager.CheckHeroDeath(hero))
            {
                HandleDeath();
                return;
            }

            // Check level complete
            if (levelManager.IsLevelComplete())
            {
                HandleLevelComplete();
            }

            // Update camera
            camera.Follow(hero.Bounds);
        }

        private void HandleInput()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
            {
                screenManager.ChangeState(GameState.MainMenu);
            }

            previousKeyState = keyState;
        }

        // ====================================
        // DEATH HANDLING
        // ====================================

        private void HandleDeath()
        {
            lastDeathPosition = hero.Position;

            if (isGodMode)
            {
                HandleGodModeDeath();
            }
            else
            {
                HandleNormalModeDeath();
            }
        }

        private void HandleGodModeDeath()
        {
            Vector2 respawnPosition = new Vector2(
                lastDeathPosition.X,
                lastDeathPosition.Y - 1000
            );

            hero.Respawn(respawnPosition);
            hero.Velocity = Vector2.Zero;
            hero.IsGrounded = false;
        }

        private void HandleNormalModeDeath()
        {
            hero.Respawn(levelManager.GetSpawnPoint());
            hero.Velocity = Vector2.Zero;
            hero.IsGrounded = true;

            screenManager.ChangeState(GameState.GameOver);
        }

        // ====================================
        // LEVEL PROGRESSION
        // ====================================

        private void HandleLevelComplete()
        {
            if (levelManager.LoadNextLevel())
            {
                SpawnHero();
            }
            else
            {
                screenManager.ChangeState(GameState.MainMenu);
            }
        }

        // ====================================
        // DRAW
        // ====================================

        public void Draw(SpriteBatch spriteBatch)
        {
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(
                transformMatrix: camera.GetMatrix(graphicsDevice.Viewport),
                samplerState: SamplerState.PointClamp);

            levelManager.Draw(spriteBatch);
            hero.Draw(spriteBatch);

            spriteBatch.End();
        }

        public void UnloadContent() { }
    }
}