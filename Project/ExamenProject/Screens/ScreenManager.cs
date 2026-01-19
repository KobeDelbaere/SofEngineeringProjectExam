using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ExamenProject.Screens
{
    /// <summary>
    /// ScreenManager - Manages game screens and state transitions
    /// Volgt Single Responsibility Principle - alleen screen management
    /// </summary>
    public class ScreenManager
    {
        private Dictionary<GameState, IScreen> screens;
        private GameState currentState;
        private IScreen currentScreen;

        public GameState CurrentState => currentState;

        public ScreenManager()
        {
            screens = new Dictionary<GameState, IScreen>();
            currentState = GameState.MainMenu;
        }

        /// <summary>
        /// Register a screen for a specific game state
        /// </summary>
        public void AddScreen(GameState state, IScreen screen)
        {
            if (screens.ContainsKey(state))
            {
                screens[state] = screen;
            }
            else
            {
                screens.Add(state, screen);
            }
        }

        /// <summary>
        /// Get a specific screen by its state
        /// Gebruikt voor cross-screen communication (bv. restart level)
        /// </summary>
        public T GetScreen<T>(GameState state) where T : class, IScreen
        {
            if (screens.ContainsKey(state))
            {
                return screens[state] as T;
            }
            return null;
        }

        /// <summary>
        /// Change to a different game state
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (!screens.ContainsKey(newState))
            {
                return;
            }

            // UnloadContent van oude screen (optioneel)
            currentScreen?.UnloadContent();

            // Switch naar nieuwe screen
            currentState = newState;
            currentScreen = screens[currentState];
        }

        /// <summary>
        /// Update current active screen
        /// </summary>
        public void Update(GameTime gameTime)
        {
            currentScreen?.Update(gameTime);
        }

        /// <summary>
        /// Draw current active screen
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen?.Draw(spriteBatch);
        }
    }

    /// <summary>
    /// Game state enum
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver
    }
}