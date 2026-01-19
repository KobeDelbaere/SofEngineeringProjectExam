using Microsoft.Xna.Framework;

namespace ExamenProject.Entities.Player
{
    /// <summary>
    /// Hero Physics Component - Handles all physics calculations
    /// Volgt Single Responsibility Principle - alleen physics
    /// </summary>
    public class HeroPhysics
    {
        // Physics constants
        private const float Gravity = 0.6f;
        private const float MaxFall = 12f;
        private const float Speed = 4f;
        private const float JumpPower = -16f;

        public Vector2 Velocity { get; set; }
        public bool IsGrounded { get; set; }

        public HeroPhysics()
        {
            Velocity = Vector2.Zero;
            IsGrounded = false;
        }

        /// <summary>
        /// Apply gravity and update velocity
        /// </summary>
        public void ApplyGravity()
        {
            float newVelocityY = MathHelper.Min(Velocity.Y + Gravity, MaxFall);
            Velocity = new Vector2(Velocity.X, newVelocityY);
        }

        /// <summary>
        /// Set horizontal velocity based on input
        /// </summary>
        public void SetHorizontalVelocity(float horizontal)
        {
            Velocity = new Vector2(horizontal * Speed, Velocity.Y);
        }

        /// <summary>
        /// Perform jump
        /// </summary>
        public void Jump()
        {
            if (IsGrounded)
            {
                Velocity = new Vector2(Velocity.X, JumpPower);
                IsGrounded = false;
            }
        }

        /// <summary>
        /// Update grounded state based on velocity
        /// </summary>
        public void UpdateGroundedState()
        {
            if (Velocity.Y != 0)
            {
                IsGrounded = false;
            }
        }

        /// <summary>
        /// Reset physics state
        /// </summary>
        public void Reset()
        {
            Velocity = Vector2.Zero;
            IsGrounded = false;
        }
    }
}