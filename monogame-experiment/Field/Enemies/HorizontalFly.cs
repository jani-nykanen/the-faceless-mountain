using System;

using Microsoft.Xna.Framework;
using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field.Enemies
{
    // A horizontally flying fly
    public class HorizontalFly : Enemy
    {

        // Update AI
        protected override void UpdateAI(float tm)
        {
            // ...
        }


        // Animate
        protected override void Animate(float tm)
        {
            const float ANIM_SPEED = 6.0f;

            spr.Animate(0, 0, 3, ANIM_SPEED, tm);
        }


        // Constructor
        public HorizontalFly(float x, float y) : base(x, y)
        {
            const float SPEED_X = 2.5f;
            const float ACC_X = 0.15f;

            // Set speed
            target.X = ((int)(y / Stage.TILE_SIZE) % 2 == 0 ? -1 : 1)
                        * SPEED_X;
            speed.X = target.X;

            acc.X = ACC_X;
            height /= 2;
        }


        // On wall collision
        protected override void OnWallCollision(float x, float y, int dir)
        {
            if (moveDir != dir) return;

            pos.X = x;
            target.X *= -1;
            speed.X = 0.0f;

        }
    }
}
