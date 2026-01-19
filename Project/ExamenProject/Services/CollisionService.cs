using ExamenProject.Entities.Player;
using ExamenProject.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ExamenProject.Services
{
    public class CollisionService
    {
        public void ResolvePlatforms(Hero hero, IEnumerable<IGameObject> platforms)
        {
            if (hero.DropDown)
                return;

            Rectangle nextBounds = new Rectangle(
                (int)hero.Position.X,
                (int)(hero.Position.Y + hero.Velocity.Y),
                hero.Bounds.Width,
                hero.Bounds.Height
            );

            if (hero.Velocity.Y > 0)
            {
                foreach (var p in platforms)
                {
                    if (hero.Bounds.Bottom <= p.Bounds.Top && nextBounds.Bottom >= p.Bounds.Top && nextBounds.Intersects(p.Bounds))
                    {
                        hero.Position = new Vector2(hero.Position.X, p.Bounds.Top - hero.Bounds.Height);
                        hero.Velocity = new Vector2(hero.Velocity.X, 0);
                        hero.IsGrounded = true;
                    }
                }
            }
        }


        public void ResolveFloors(Hero hero, IEnumerable<IGameObject> floors)
        {
            //hero.IsGrounded = false;

            // -------- X-AS COLLISION --------
            Rectangle nextX = new Rectangle(
                (int)(hero.Position.X + hero.Velocity.X),
                (int)hero.Position.Y,
                hero.Bounds.Width,
                hero.Bounds.Height
            );

            foreach (var floor in floors)
            {
                if (nextX.Intersects(floor.Bounds))
                {
                    if (hero.Velocity.X > 0)
                        hero.Position = new Vector2(
                            floor.Bounds.Left - hero.Bounds.Width,
                            hero.Position.Y
                        );
                    else if (hero.Velocity.X < 0)
                        hero.Position = new Vector2(
                            floor.Bounds.Right,
                            hero.Position.Y
                        );

                    hero.Velocity = new Vector2(0, hero.Velocity.Y);
                }
            }

            // -------- Y-AS COLLISION --------
            Rectangle nextY = new Rectangle(
                (int)hero.Position.X,
                (int)(hero.Position.Y + hero.Velocity.Y),
                hero.Bounds.Width,
                hero.Bounds.Height
            );

            foreach (var floor in floors)
            {
                if (nextY.Intersects(floor.Bounds))
                {
                    if (hero.Velocity.Y > 0)
                    {
                        // Landen
                        hero.Position = new Vector2(
                            hero.Position.X,
                            floor.Bounds.Top - hero.Bounds.Height
                        );
                        hero.IsGrounded = true;
                    }
                    else if (hero.Velocity.Y < 0)
                    {
                        // Hoofd stoot
                        hero.Position = new Vector2(
                            hero.Position.X,
                            floor.Bounds.Bottom
                        );
                    }

                    hero.Velocity = new Vector2(hero.Velocity.X, 0);
                }
            }
        }



        public void ResolveHazards(Hero hero, IEnumerable<IHazard> hazards)
        {
            foreach (var h in hazards)
            {
                if (hero.Bounds.Intersects(h.Bounds))
                {
                    h.OnHit(hero);
                }
            }      
        }
        public void ResolveLevelEnd(Hero hero, IEnumerable<ILevelEnd> levelEnds)
        {
            foreach (var l in levelEnds)
            {
                if (hero.Bounds.Intersects(l.Bounds))
                {
                    l.OnHit(hero);
                }
            }
        }
    }
}
