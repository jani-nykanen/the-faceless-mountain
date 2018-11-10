using System;

using Microsoft.Xna.Framework;
using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field.Enemies
{
    // A vertically flying fly
    public class VerticalFly : Enemy
    {

        // Update AI
        protected override void UpdateAI(float tm)
        {
            // ...
        }


        // Animate
        protected override void Animate(float tm)
        {
            // ...
        }


        // Constructor
        public VerticalFly(float x, float y) : base(x, y)
        {
            const float SPEED_Y = 2.5f;
            const float ACC_Y = 0.50f;

            // Set speed
            target.Y= ((int)(y / Stage.TILE_SIZE) % 2 == 0 ? -1 : 1)
                        * SPEED_Y;
            speed.Y = target.Y;

            acc.Y = ACC_Y;
            width /= 2;

        }


        // On floor collision
        protected override void OnFloorCollision(float x, float y)
        {
            if (speed.Y < 0.0f) return;

            pos.Y = y;
            target.Y *= -1;
            speed.Y = 0.0f;
        }


        // On ceiling collision
        protected override void OnCeilingCollision(float x, float y)
        {
            if (speed.Y > 0.0f) return;

            pos.Y = y;
            target.Y *= -1;
            speed.Y = 0.0f;
        }
    }
}
