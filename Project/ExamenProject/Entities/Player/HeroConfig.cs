namespace ExamenProject.Entities.Player
{
    /// <summary>
    /// Hero Configuration - All hero constants in one place
    /// Volgt Single Responsibility Principle - alleen configuration
    /// </summary>
    public static class HeroConfig
    {
        // Hitbox dimensions
        public const int HitboxWidth = 28 * 2;
        public const int HitboxHeight = 40 * 2;

        // Sprite dimensions
        public const int BaseFrameWidth = 28;
        public const int SpriteOffsetY = -80;

        // Combat
        public const int AttackRange = 100;
    }
}