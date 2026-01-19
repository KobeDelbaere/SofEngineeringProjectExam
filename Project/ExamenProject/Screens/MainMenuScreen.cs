using ExamenProject.Interfaces;
using ExamenProject.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExamenProject.Screens
{
    /// <summary>
    /// MainMenuScreen - Menu interface
    /// Volgt Single Responsibility Principle - alleen menu logic
    /// Volgt Dependency Inversion Principle - gebruikt TextureManager
    /// </summary>
    public class MainMenuScreen : IScreen
    {
        private ScreenManager screenManager;
        private GraphicsDevice graphicsDevice;
        private TextureManager textures;
        private Game game;

        private KeyboardState previousKeyState;
        private MouseState previousMouseState;

        // Clickable areas
        private Rectangle normalButtonBounds;
        private Rectangle godModeButtonBounds;
        private Rectangle exitButtonBounds;

        // Hover states
        private bool hoveringNormal;
        private bool hoveringGodMode;
        private bool hoveringExit;

        public MainMenuScreen(
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
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            normalButtonBounds = new Rectangle(
                screenWidth / 2 - 210,
                screenHeight / 2 - 180,
                455,
                118
            );

            godModeButtonBounds = new Rectangle(
                screenWidth / 2 - 210,
                screenHeight / 2 + 39,
                455,
                116
            );

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
            MouseState mouseState = Mouse.GetState();
            Point mousePos = new Point(mouseState.X, mouseState.Y);

            // Check hover states
            hoveringNormal = normalButtonBounds.Contains(mousePos);
            hoveringGodMode = godModeButtonBounds.Contains(mousePos);
            hoveringExit = exitButtonBounds.Contains(mousePos);

            // Handle mouse clicks
            if (mouseState.LeftButton == ButtonState.Released &&
                previousMouseState.LeftButton == ButtonState.Pressed)
            {
                HandleSelection();
            }

            previousMouseState = mouseState;
        }

        private void HandleSelection()
        {
            if (hoveringNormal)
            {
                StartNormal();
            }
            else if (hoveringGodMode)
            {
                StartGod();
            }
            else if (hoveringExit)
            {
                game.Exit();
            }
        }

        private void StartNormal()
        {
            var gameplay = screenManager.GetScreen<GamePlayScreen>(GameState.Playing);
            gameplay?.StartNormalMode();
            screenManager.ChangeState(GameState.Playing);
        }

        private void StartGod()
        {
            var gameplay = screenManager.GetScreen<GamePlayScreen>(GameState.Playing);
            gameplay?.StartGodMode();
            screenManager.ChangeState(GameState.Playing);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            graphicsDevice.Clear(new Color(101, 67, 33));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw background
            if (textures.MenuBackground != null)
            {
                spriteBatch.Draw(
                    textures.MenuBackground,
                    new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
                    Color.White
                );
            }
            else
            {
                DrawFallbackMenu(spriteBatch);
            }

            // Draw hover effects
            if (hoveringNormal)
                spriteBatch.Draw(textures.Pixel, normalButtonBounds, Color.Yellow * 0.3f);

            if (hoveringGodMode)
                spriteBatch.Draw(textures.Pixel, godModeButtonBounds, Color.Yellow * 0.3f);

            if (hoveringExit)
                spriteBatch.Draw(textures.Pixel, exitButtonBounds, Color.Red * 0.3f);

            spriteBatch.End();
        }

        private void DrawFallbackMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures.Pixel, normalButtonBounds,
                hoveringNormal ? Color.Gold : Color.SaddleBrown);

            spriteBatch.Draw(textures.Pixel, godModeButtonBounds,
                hoveringGodMode ? Color.Gold : Color.SaddleBrown);

            spriteBatch.Draw(textures.Pixel, exitButtonBounds,
                hoveringExit ? Color.DarkRed : Color.Brown);
        }

        public void UnloadContent() { }
    }
}