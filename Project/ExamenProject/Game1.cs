using ExamenProject.Interfaces;
using ExamenProject.Screens;
using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private ScreenManager screenManager;
        private TextureManager textureManager;
        private IInputService inputService;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Screen setup (optioneel)
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // 1. Create services
            textureManager = new TextureManager(Content, GraphicsDevice);
            textureManager.LoadAllTextures();

            inputService = new KeyboardInputService();
            screenManager = new ScreenManager();

            // 2. Create screens
            var gameplayScreen = new GamePlayScreen(
                screenManager,
                GraphicsDevice,
                Content,
                inputService,
                textureManager);

            var mainMenuScreen = new MainMenuScreen(
                screenManager,
                GraphicsDevice,
                textureManager,
                this);

            var gameOverScreen = new GameOverScreen(
                screenManager,
                GraphicsDevice,
                textureManager,
                this);

            // 3. Register screens
            screenManager.AddScreen(GameState.Playing, gameplayScreen);
            screenManager.AddScreen(GameState.MainMenu, mainMenuScreen);
            screenManager.AddScreen(GameState.GameOver, gameOverScreen);

            // 4. Initialize all screens
            gameplayScreen.Initialize();
            mainMenuScreen.Initialize();
            gameOverScreen.Initialize();

            // 5. Load content for all screens
            gameplayScreen.LoadContent();
            mainMenuScreen.LoadContent();
            gameOverScreen.LoadContent();

            // 6. Start at main menu
            screenManager.ChangeState(GameState.MainMenu);
        }

        protected override void Update(GameTime gameTime)
        {
            // BELANGRIJK: Update input service EERST
            inputService?.Update();

            // Dan update screen manager
            screenManager?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Current screen draws itself
            screenManager?.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}