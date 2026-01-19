using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExamenProject.Entities.Player
{
    /// <summary>
    /// Hero Combat Component - Handles attack logic
    /// Volgt Single Responsibility Principle - alleen combat
    /// </summary>
    public class HeroCombat
    {
        private const int AttackRange = 100;
        private const int HitboxWidth = 28 * 2;
        private const int HitboxHeight = 40 * 2;

        public bool IsAttacking { get; private set; }

        public HeroCombat()
        {
            IsAttacking = false;
        }

        /// <summary>
        /// Start attack
        /// </summary>
        public void StartAttack()
        {
            IsAttacking = true;
        }

        /// <summary>
        /// End attack
        /// </summary>
        public void EndAttack()
        {
            IsAttacking = false;
        }

        /// <summary>
        /// Get attack hitbox
        /// </summary>
        public Rectangle GetAttackBounds(Vector2 position, SpriteEffects flip)
        {
            if (!IsAttacking)
                return Rectangle.Empty;

            int attackX = flip == SpriteEffects.FlipHorizontally
                ? (int)position.X - AttackRange
                : (int)position.X + HitboxWidth;

            return new Rectangle(
                attackX,
                (int)position.Y,
                AttackRange,
                HitboxHeight
            );
        }

        /// <summary>
        /// Reset combat state
        /// </summary>
        public void Reset()
        {
            IsAttacking = false;
        }
    }
}