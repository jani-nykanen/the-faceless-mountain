using System;

using monogame_experiment.Desktop.Core;

namespace monogame_experiment.Desktop.Field.Enemies
{
    // A vertical "thwomp"
    public class VerticalThwomp : Enemy
    {
        // Wait time
        const float WAIT_TIME = 60.0f;

        // Is active
        private bool thwomping;
        // Thwomping mode
        private int thwompMode;
        // Thwomp wait
        private float thwompWait;

        // Shake
        private bool shake;


        // Constructor
        public VerticalThwomp(float x, float y, Graphics.Flip flip = Graphics.Flip.None) : base(x, y, flip)
        {
            width /= 2;
            target.Y = 0.0f;
            speed.Y = 0.0f;

            waving = false;
            shake = false;
        }


        // Player event
        protected override void OnPlayerEvent(Player pl)
        {
            if (thwomping) return;

            const float MARGIN = 64.0f;

            float x = pl.GetPos().X;
            float w = pl.GetWidth();

            // Check if in the same column
            if(inCamera && 
               (  (flip == Graphics.Flip.None && pl.GetPos().Y > pos.Y-Stage.TILE_SIZE)
                || (flip == Graphics.Flip.Vertical && pl.GetPos().Y - pl.GetHeight() <= pos.Y)
               )

               && x+w/2 > pos.X - Stage.TILE_SIZE/2 - MARGIN
               && x-w/2 < pos.X + Stage.TILE_SIZE/2 + MARGIN)
            {
                thwomping = true;
                thwompMode = 0;
            }
        }


        // Camera event
        protected override void OnCameraEvent(Camera cam)
        {
            const float SHAKE_TIME = 60.0f;
            const float SHAKE_VAL = 16.0f;

            if(shake)
            {
                cam.Shake(SHAKE_TIME, SHAKE_VAL);
                shake = false;
            }
        }


        // Update AI
        protected override void UpdateAI(float tm)
        {
            const float GRAVITY = 24.0f;
            const float GRAV_ACC = 0.50f;

            const float RETURN_SPEED = 2.0f;

            // "Thwomp"
            if(thwomping)
            {
                // Going down
                if(thwompMode == 0)
                {
                    target.Y = GRAVITY;
                    acc.Y = GRAV_ACC;
                }
                // Waiting
                else if(thwompMode == 1)
                {
                    acc.Y = 0.0f;
                    target.Y = 0.0f;
                    if((thwompWait -= 1.0f * tm) <= 0.0f)
                    {
                        thwompMode = 2;
                        thwompWait = 0.0f;
                    }
                }
                // Or returning up
                else 
                {
                    target.Y = -RETURN_SPEED;
                    acc.Y = RETURN_SPEED;

                    // If returned
                    if ((flip == Graphics.Flip.None && pos.Y < startPos.Y) ||
                       (flip == Graphics.Flip.Vertical && pos.Y > startPos.Y))
                    {
                        pos.Y = startPos.Y;
                        speed.Y = 0.0f;
                        target.Y = 0.0f;

                        thwomping = false;
                        thwompMode = 0;
                    }
                }
            }
            else
            {
                acc.Y = 0.0f;
                target.Y = 0.0f;
            }

            // If flipped, flip speed
            if(flip == Graphics.Flip.Vertical)
            {
                target.Y *= -1;
            }
        }


        // Animate
        protected override void Animate(float tm)
        {
            int frame = (thwomping && thwompMode < 2) ? 1 : 0;
            spr.Animate(6, frame, frame, 0, tm);
        }


        // On floor collision
        protected override void OnFloorCollision(float x, float y)
        {
            if (speed.Y < 0.0f) return;

            pos.Y = y;
            speed.Y = 0.0f;

            // Switch to the wait mode
            if(thwompMode == 0 && flip == Graphics.Flip.None)
            {
                thwompMode = 1;
                thwompWait = WAIT_TIME;
                shake = true;
            }
            // Or if flipped, stop
            else if (thwompMode == 2 && flip == Graphics.Flip.Vertical)
            {
                thwompMode = 0;
                thwomping = false;
            }
        }


        // On ceiling collision
        protected override void OnCeilingCollision(float x, float y)
        {
            if (speed.Y > 0.0f) return;

            pos.Y = y;
            speed.Y = 0.0f;

            // If "going down" thwomp, then stop if hit a ceiling
            if (thwompMode == 2 && flip == Graphics.Flip.None)
            {
                thwompMode = 0;
                thwomping = false;
            }
            // Or switch to the wait mode
            else if(thwompMode == 0 && flip == Graphics.Flip.Vertical)
            {
                thwompMode = 1;
                thwompWait = WAIT_TIME;
                shake = true;
            }
        }
    }
}
