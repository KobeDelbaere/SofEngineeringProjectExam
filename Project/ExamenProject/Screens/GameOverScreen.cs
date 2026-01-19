using ExamenProject.Interfaces;
using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExamenProject.Screens
{
    /// <summary>
    /// Game Over Screen
    /// Volgt Single Responsibility Principle - alleen game over logic
    /// Volgt Dependency Inversion Principle - gebruikt TextureManager
    /// </summary>
    public class GameOverScreen : IScreen
    {
        private ScreenManager screenManager;
        private GraphicsDevice graphicsDevice;
        private TextureManager textures;
        private Game game;

        private KeyboardState previousKeyState;
        private MouseState previousMouseState;

        private double displayTimer;
        private const double MinDisplayTime = 0.5;

        // Clickable areas
        private Rectangle replayButtonBounds;
        private Rectangle mainMenuButtonBounds;
        private Rectangle exitButtonBounds;

        // Hover states
        private bool hoveringReplay;
        private bool hoveringMainMenu;
        private bool hoveringExit;

        public GameOverScreen(
            ScreenManager manager,
            GraphicsDevice device,
            TextureManager textureManager,
            Game gameInstance)
        {
            screenManager = manager;
            graphicsDevice = device;
            textures = textureManager;
            game = gameInstance;
        }

        public void Initialize()
        {
            displayTimer = 0;

            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            // Replay button (bovenste)
            replayButtonBounds = new Rectangle(
                screenWidth / 2 - 210,
                screenHeight / 2 - 180,
                455,
                118
            );

            // Main Menu button (onderste)
            mainMenuButtonBounds = new Rectangle(
                screenWidth / 2 - 210,
                screenHeight / 2 + 39,
                455,
                116
            );

            // Exit button (rechtsboven cirkel met X)
            exitButtonBounds = new Rectangle(
                screenWidth - 252,
                88,
                126,
                116
            );
        }

        public void LoadContent()
        {
            // Textures are already loaded by TextureManager
        }

        public void Update(GameTime gameTime)
        {
            displayTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (displayTimer < MinDisplayTime)
                return;

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            // Check hover states
            Point mousePos = new Point(mouseState.X, mouseState.Y);
            hoveringReplay = replayButtonBounds.Contains(mousePos);
            hoveringMainMenu = mainMenuButtonBounds.Contains(mousePos);
            hoveringExit = exitButtonBounds.Contains(mousePos);

            // Handle mouse clicks
            if (mouseState.LeftButton == ButtonState.Released &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                if (hoveringReplay)
                {
                    var gameplayScreen = screenManager.GetScreen<GamePlayScreen>(GameState.Playing);
                    gameplayScreen?.RestartCurrentLevel();
                    screenManager.ChangeState(GameState.Playing);
                }
                else if (hoveringMainMenu)
                {
                    screenManager.ChangeState(GameState.MainMenu);
                }
                else if (hoveringExit)
                {
                    game.Exit();
                }
            }

            // Escape to menu
            if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
            {
                screenManager.ChangeState(GameState.MainMenu);
            }

            previousKeyState = keyState;
            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            graphicsDevice.Clear(new Color(101, 67, 33));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw background
            if (textures.GameOverBackground != null)
            {
                Rectangle destRect = new Rectangle(
                    0, 0,
                    graphicsDevice.Viewport.Width,
                    graphicsDevice.Viewport.Height
                );
                spriteBatch.Draw(textures.GameOverBackground, destRect, Color.White);
            }
            else
            {
                DrawFallbackMenu(spriteBatch);
            }

            // Draw hover effects
            //if (hoveringReplay)
            //    spriteBatch.Draw(textures.Pixel, replayButtonBounds, Color.Yellow * 0.3f);

            //if (hoveringMainMenu)
            //    spriteBatch.Draw(textures.Pixel, mainMenuButtonBounds, Color.Yellow * 0.3f);

            //if (hoveringExit)
            //    spriteBatch.Draw(textures.Pixel, exitButtonBounds, Color.Red * 0.3f);

            spriteBatch.End();
        }

        private void DrawFallbackMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures.Pixel, replayButtonBounds,
                hoveringReplay ? Color.Gold : Color.DarkRed);

            spriteBatch.Draw(textures.Pixel, mainMenuButtonBounds,
                hoveringMainMenu ? Color.Gold : Color.DarkRed);

            spriteBatch.Draw(textures.Pixel, exitButtonBounds,
                hoveringExit ? Color.Red : Color.Brown);
        }

        public void UnloadContent()
        {
            displayTimer = 0;
        }
    }
}