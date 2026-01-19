using Microsoft.Xna.Framework.Input;

namespace ExamenProject.Interfaces
{
    /// <summary>
    /// IInputService - Input abstraction
    /// Volgt Dependency Inversion Principle - Hero is afhankelijk van abstractie
    /// Volgt Interface Segregation Principle - alleen essentiële input methods
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Update input state - moet elke frame aangeroepen worden
        /// </summary>
        void Update();

        /// <summary>
        /// Horizontal movement input (-1 voor links, 0 voor geen input, 1 voor rechts)
        /// </summary>
        float Horizontal { get; }

        /// <summary>
        /// Jump button (continuous - kan ingehouden worden)
        /// </summary>
        bool JumpPressed { get; }

        /// <summary>
        /// Drop through platform button
        /// </summary>
        bool DropPressed { get; }

        /// <summary>
        /// Attack button (single press - niet continuous)
        /// </summary>
        bool AttackPressed { get; }

        /// <summary>
        /// Check if specific key was just pressed this frame
        /// </summary>
        bool IsKeyPressed(Keys key);

        /// <summary>
        /// Check if specific key is currently held down
        /// </summary>
        bool IsKeyDown(Keys key);

        /// <summary>
        /// Check if specific key was just released this frame
        /// </summary>
        bool IsKeyReleased(Keys key);
    }
}