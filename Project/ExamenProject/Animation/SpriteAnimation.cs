using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ExamenProject.Animation
{
    /// <summary>
    /// SpriteAnimation - Manages a sequence of animation frames
    /// 
    /// SOLID Principles Applied:
    /// - Single Responsibility Principle: Only manages frame sequencing and timing.
    ///   Does not handle rendering or state management.
    /// - Open/Closed Principle: Can be extended for custom animation behaviors
    ///   without modifying existing code.
    /// 
    /// Design Patterns:
    /// - Iterator Pattern: Provides access to frames in sequence
    /// </summary>
    public class SpriteAnimation
    {
        /// <summary>
        /// List of animation frames
        /// Encapsulation: Private list prevents external modification
        /// </summary>
        private readonly List<AnimationFrame> frames = new();

        /// <summary>
        /// Current frame index
        /// </summary>
        private int index = 0;

        /// <summary>
        /// Timer for frame progression
        /// </summary>
        private double timer = 0;

        /// <summary>
        /// Time per frame in milliseconds
        /// Default: 100ms (10 FPS)
        /// </summary>
        public double FrameTime { get; set; } = 100;

        /// <summary>
        /// Sprite sheet texture
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Current animation frame
        /// </summary>
        public AnimationFrame Current => frames[index];

        /// <summary>
        /// Whether animation should loop
        /// </summary>
        public bool Loop { get; set; } = true;

        /// <summary>
        /// Whether animation has finished (only relevant for non-looping animations)
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Constructor - Dependency Injection of texture
        /// 
        /// SOLID Principle: Dependency Inversion - depends on Texture2D abstraction
        /// </summary>
        public SpriteAnimation(Texture2D texture)
        {
            Texture = texture;
        }

        /// <summary>
        /// Add a frame to the animation
        /// Single Responsibility Principle: Only manages frame collection
        /// </summary>
        public void AddFrame(Rectangle source)
        {
            frames.Add(new AnimationFrame(source));
        }

        /// <summary>
        /// Reset animation to first frame
        /// Single Responsibility Principle: Only handles reset logic
        /// </summary>
        public void Reset()
        {
            index = 0;
            timer = 0;
            IsFinished = false;
        }

        /// <summary>
        /// Update animation timing and progress frames
        /// Single Responsibility Principle: Only handles frame progression logic
        /// 
        /// Handles both looping and non-looping animations
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Skip if no frames or animation is finished
            if (frames.Count <= 1)
                return;

            if (IsFinished && !Loop)
                return;

            // Update timer
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Progress to next frame when time is up
            if (timer >= FrameTime)
            {
                timer = 0;
                index++;

                // Handle end of animation
                if (index >= frames.Count)
                {
                    if (Loop)
                    {
                        // Loop back to beginning
                        index = 0;
                    }
                    else
                    {
                        // Stop on last frame
                        index = frames.Count - 1;
                        IsFinished = true;
                    }
                }
            }
        }
    }
}