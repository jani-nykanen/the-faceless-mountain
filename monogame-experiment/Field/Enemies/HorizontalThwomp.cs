using System;

using monogame_experiment.Desktop.Core;

namespace monogame_experiment.Desktop.Field.Enemies
{
    // A horizontal "thwomp"
    public class HorizontalThwomp : Enemy
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


        // Play thwomp
        private void Thwomp()
        {
            audio.PlaySample(sThwomp, 0.70f);
        }


        // Constructor
        public HorizontalThwomp(float x, float y, Graphics.Flip flip = Graphics.Flip.None) : base(x, y, flip)
        {
            height /= 2;
            target.X = 0.0f;
            speed.X = 0.0f;

            waving = false;
            shake = false;
        }


        // Player event
        protected override void OnPlayerEvent(Player pl)
        {
            if (thwomping) return;

            const float MARGIN = 64.0f;

            float y = pl.GetPos().Y - pl.GetHeight() / 2;
            float h = pl.GetHeight();
            float w = pl.GetWidth();

            // Check if in the same row
            if(inCamera && 
               (  (flip == Graphics.Flip.None && pl.GetPos().X+w > pos.X-Stage.TILE_SIZE/2)
                || (flip == Graphics.Flip.Horizontal && pl.GetPos().X - w <= pos.X+Stage.TILE_SIZE/2)
               )

               && y+h/2 > pos.Y - Stage.TILE_SIZE - MARGIN
               && y-h/2 < pos.Y + MARGIN)
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

            if (shake)
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

            // Do not take collisions if not necessary
            getCollision = thwomping && thwompMode == 0;

            // "Thwomp"
            if (thwomping)
            {
                // Going down
                if(thwompMode == 0)
                {
                    target.X = GRAVITY;
                    acc.X = GRAV_ACC;
                }
                // Waiting
                else if(thwompMode == 1)
                {
                    acc.X = 0.0f;
                    target.X = 0.0f;
                    if((thwompWait -= 1.0f * tm) <= 0.0f)
                    {
                        thwompMode = 2;
                        thwompWait = 0.0f;
                    }
                }
                // Or returning up
                else 
                {
                    target.X = -RETURN_SPEED;
                    acc.X = RETURN_SPEED;

                    // If returned
                    if ((flip == Graphics.Flip.None && pos.X < startPos.X) ||
                        (flip == Graphics.Flip.Horizontal && pos.X > startPos.X))
                    {
                        pos.X = startPos.X;
                        speed.X = 0.0f;
                        target.X = 0.0f;

                        thwomping = false;
                        thwompMode = 0;
                    }
                }
            }
            else
            {
                acc.X = 0.0f;
                target.X = 0.0f;
            }

            // If flipped, flip speed
            if(flip == Graphics.Flip.Horizontal)
            {
                target.X *= -1;
            }
        }


        // Animate
        protected override void Animate(float tm)
        {
            int frame = (thwomping && thwompMode < 2) ? 1 : 0;
            spr.Animate(7, frame, frame, 0, tm);
        }


        // On wall collision
        protected override void OnWallCollision(float x, float y, int dir)
        {
            pos.X = x;
            speed.X = 0.0f;

            // Right
            if(dir == 1)
            {
                // Switch to the wait mode
                if (thwompMode == 0 && flip == Graphics.Flip.None)
                {
                    thwompMode = 1;
                    thwompWait = WAIT_TIME;
                    shake = true;

                    Thwomp();
                }
                // Or if flipped, stop
                else if (thwompMode == 2 && flip == Graphics.Flip.Horizontal)
                {
                    thwompMode = 0;
                    thwomping = false;
                }
            }
            // Left
            else 
            {
                // If "going down" thwomp, then stop if hit a ceiling
                if (thwompMode == 2 && flip == Graphics.Flip.None)
                {
                    thwompMode = 0;
                    thwomping = false;
                }
                // Or switch to the wait mode
                else if (thwompMode == 0 && flip == Graphics.Flip.Horizontal)
                {
                    thwompMode = 1;
                    thwompWait = WAIT_TIME;
                    shake = true;

                    Thwomp();
                }
            }
        }

    }
}
