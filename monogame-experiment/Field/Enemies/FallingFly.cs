using System;

using monogame_experiment.Desktop.Core;

namespace monogame_experiment.Desktop.Field.Enemies
{
    // A falling fly
    public class FallingFly : Enemy
    {

        // Constructor
        public FallingFly(float x, float y, Graphics.Flip flip = Graphics.Flip.None) : base(x, y, flip)
        {
            width /= 2;
            target.Y = 0.0f;
            speed.Y = 0.0f;
        }


        // Update AI
        protected override void UpdateAI(float tm)
        {
            const float GRAV_ACC_Y = 0.15f;
            const float RET_ACC_Y = 0.10f;

            const float GRAVITY = 6.0f;
            const float RETURN_SPEED = -8.0f;

            // If hooked, get down, otherwise back
            // to the origin
            if(hooked)
            {
                target.Y = GRAVITY;
                acc.Y = GRAV_ACC_Y;
            }
            else 
            {
                target.Y = RETURN_SPEED;
                acc.Y = RET_ACC_Y;
            }
            // If flipped, flip the speed
            if(flip == Graphics.Flip.Vertical)
            {
                target.Y *= -1;
            }


            // If too high or too low
            if( (flip == Graphics.Flip.None && pos.Y < startPos.Y) ||
               (flip == Graphics.Flip.Vertical && pos.Y > startPos.Y) )
            {
                pos.Y = startPos.Y;
                speed.Y = 0.0f;
                target.Y = 0.0f;
            }
        }


        // Animate
        protected override void Animate(float tm)
        {
            const float ANIM_SPEED = 6.0f;

            if(hooked)
                spr.Animate(2, 4, 4, 0, tm);
            else
                spr.Animate(2, 0, 3, ANIM_SPEED, tm);
        }


        // On floor collision
        protected override void OnFloorCollision(float x, float y)
        {
            if (speed.Y < 0.0f) return;

            pos.Y = y;
            speed.Y = 0.0f;
        }


        // On ceiling collision
        protected override void OnCeilingCollision(float x, float y)
        {
            if (speed.Y > 0.0f) return;

            pos.Y = y;
            speed.Y = 0.0f;
        }
    }
}
