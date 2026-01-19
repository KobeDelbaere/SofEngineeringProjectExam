using Microsoft.Xna.Framework;

namespace ExamenProject.Animation
{
    /// <summary>
    /// AnimationFrame - Represents a single frame in an animation
    /// 
    /// SOLID Principles Applied:
    /// - Single Responsibility Principle: Only stores frame data (source rectangle).
    /// - Immutability: Frame data cannot change after creation (defensive programming).
    /// </summary>
    public class AnimationFrame
    {
        /// <summary>
        /// Source rectangle in the sprite sheet
        /// </summary>
        public Rectangle Source { get; }

        /// <summary>
        /// Constructor - creates immutable frame
        /// </summary>
        public AnimationFrame(Rectangle source)
        {
            Source = source;
        }
    }
}
