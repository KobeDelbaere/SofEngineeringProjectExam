using ExamenProject.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace ExamenProject.Services
{
    /// <summary>
    /// KeyboardInputService - Handles keyboard input
    /// Volgt Single Responsibility Principle - alleen input handling
    /// Volgt Interface Segregation Principle - implementeert IInputService
    /// </summary>
    public class KeyboardInputService : IInputService
    {
        private KeyboardState previousState;
        private KeyboardState currentState;

        public KeyboardInputService()
        {
            currentState = Keyboard.GetState();
            previousState = currentState;
        }

        /// <summary>
        /// Update input state - moet ELKE frame worden aangeroepen!
        /// Dit zorgt voor correcte previousState tracking
        /// </summary>
        public void Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
        }

        /// <summary>
        /// Horizontal movement input (-1, 0, 1)
        /// </summary>
        public float Horizontal
        {
            get
            {
                if (currentState.IsKeyDown(Keys.D) || currentState.IsKeyDown(Keys.Right))
                    return 1;
                if (currentState.IsKeyDown(Keys.A) || currentState.IsKeyDown(Keys.Left))
                    return -1;
                return 0;
            }
        }

        /// <summary>
        /// Jump input - continuous (hold to jump higher)
        /// </summary>
        public bool JumpPressed
        {
            get
            {
                return currentState.IsKeyDown(Keys.W) ||
                       currentState.IsKeyDown(Keys.Up);
            }
        }

        /// <summary>
        /// Drop through platform input
        /// </summary>
        public bool DropPressed
        {
            get
            {
                return currentState.IsKeyDown(Keys.S) ||
                       currentState.IsKeyDown(Keys.Down);
            }
        }

        /// <summary>
        /// Attack input - single press (niet continuous)
        /// Returns true alleen op de EERSTE frame dat de key wordt ingedrukt
        /// </summary>
        public bool AttackPressed
        {
            get
            {
                return currentState.IsKeyDown(Keys.Space) &&
                       !previousState.IsKeyDown(Keys.Space);
            }
        }

        /// <summary>
        /// Check if a key was just pressed (single press detection)
        /// Handig voor menu navigation, pause, etc.
        /// </summary>
        public bool IsKeyPressed(Keys key)
        {
            return currentState.IsKeyDown(key) && !previousState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if a key is currently held down
        /// </summary>
        public bool IsKeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if a key was just released
        /// </summary>
        public bool IsKeyReleased(Keys key)
        {
            return !currentState.IsKeyDown(key) && previousState.IsKeyDown(key);
        }
    }
}