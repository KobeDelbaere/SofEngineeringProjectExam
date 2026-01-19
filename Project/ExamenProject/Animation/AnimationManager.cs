using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ExamenProject.Animation
{
    public enum AnimationState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Attack,
        Death
    }

    public class AnimationManager
    {
        private Dictionary<AnimationState, SpriteAnimation> animations = new();
        private AnimationState currentState;
        private AnimationState previousState;

        public SpriteAnimation Current => animations[currentState];
        public AnimationState State => currentState;
        public bool IsAnimationFinished => Current.IsFinished;

        public void AddAnimation(AnimationState state, SpriteAnimation animation)
        {
            animations[state] = animation;
        }

        public void SetState(AnimationState newState, bool forceReset = false)
        {
            if (currentState != newState || forceReset)
            {
                previousState = currentState;
                currentState = newState;
                animations[currentState].Reset();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (animations.ContainsKey(currentState))
            {
                animations[currentState].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects flip, int baseFrameWidth, float scale = 2f)
        {
            var anim = animations[currentState];
            Vector2 drawPosition = position;
            Vector2 origin = Vector2.Zero;

            // Bereken het verschil tussen de huidige frame breedte en de base breedte
            int currentFrameWidth = anim.Current.Source.Width;
            int widthDifference = currentFrameWidth - baseFrameWidth;

            if (flip == SpriteEffects.FlipHorizontally)
            {
                // Naar links: origin op de rechterkant van de base frame
                origin = new Vector2(currentFrameWidth, 0);
                // Compenseer voor de extra breedte naar rechts
                drawPosition.X += baseFrameWidth * scale;
            }
            else
            {
                // Naar rechts: origin links, maar compenseer voor extra breedte
                origin = Vector2.Zero;
                // Verschuif naar links met de helft van het verschil om te centreren
                // (attack frames zijn breder, dus verschuif ze een beetje naar links)
            }

            spriteBatch.Draw(
                anim.Texture,
                drawPosition,
                anim.Current.Source,
                Color.White,
                0f,
                origin,
                scale,
                flip,
                0f
            );
        }
    }
}