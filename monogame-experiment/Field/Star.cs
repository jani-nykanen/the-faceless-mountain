using System;

using Microsoft.Xna.Framework;
using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // The goal star
    public class Star
    {
        // Global bitmaps
        static private Bitmap bmpStar;

        // Initialize global content
        public static void Init(AssetPack assets)
        {
            bmpStar = assets.GetBitmap("star");
        }


        // Position
        private Vector2 pos;
        // Start pos
        private Vector2 startPos;

        // Scale timer
        private float scaleTimer;
        // Shine timer
        private float shineTimer;

        // If ending activated
        private bool activated;
        // Scale when ending activated
        private float endingScale;
        // Ending timers
        private float endingTimer1;
        private float endingTimer2;
        // Has stopped
        private bool stopped;


        // Constructor
        public Star(Vector2 pos)
        {
            this.pos = pos;
            startPos = pos;

            scaleTimer = 0.0f;
            shineTimer = 0.0f;
            endingScale = 1.0f;

            activated = false;
        }


        // Update
        public void Update(float tm)
        {
            const float SCALE_SPEED = 0.05f;
            const float SHINE_SPEED = 0.025f;

            const float ENDING_TIMER1_SPEED = 0.05f;
            const float ENDING_TIMER2_SPEED = 0.025f;

            const float STOP = (float)Math.PI * 3.0f;

            if (activated)
            {
                scaleTimer = 0.0f;
                shineTimer = 0.0f;

                // Update ending timers
                endingTimer1 += ENDING_TIMER1_SPEED * tm;
                endingTimer2 += ENDING_TIMER2_SPEED * tm;

                // Stop animation here
                if (endingTimer1 > STOP)
                {
                    stopped = true;
                    return;
                }

                // Compute position & scale
                float s1 = (float)Math.Sin(endingTimer1);
                float s2 = (float)Math.Sin(endingTimer2);

                pos.Y = startPos.Y + s1 * s2 * 192.0f;
                pos.X = startPos.X + s1 * 128.0f;

                endingScale = 1.0f + s2;
            }
            else
            {
                // Update scale timer
                scaleTimer += SCALE_SPEED * tm;
            }

            // Update shine timer
            shineTimer += SHINE_SPEED * tm;
        }


        // Player collision
        public void GetPlayerCollision(Player pl)
        {
            // If activate, teleport player to the position
            if(activated)
            {
                pl.Sit(pos, stopped);
                pl.SetSpecialScale(endingScale);
                return;
            }

            const float WIDTH = 192.0f;
            const float HEIGHT = 192.0f;

            Vector2 p = pl.GetPos();

            // Check if inside the collision box
            if(p.X > pos.X - WIDTH/2 && p.X < pos.X + WIDTH/2
               && p.Y > pos.Y - HEIGHT/2 && p.Y < pos.Y + HEIGHT/2)
            {
                activated = true;

                pl.Sit(pos);
            }
        }


        // Gives distance from the player in metres
        public float GetDistance(Player pl) 
        {
            // No distance if the player has
            // touched the star
            if (activated) return 0.0f;

            float dx = pl.GetPos().X - pos.X;
            float dy = pl.GetPos().Y - pl.GetHeight() / 2 - pos.Y;

            // Distance in "pixels"
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);

            // Distance in metres, assuming each tile is one metre
            // high
            return dist / (float)Stage.TILE_SIZE;
        }


        // Draw
        public void Draw(Graphics g, Camera cam)
        {
            const float SAFE_VALUE = 128;
            const float WAVE = 8.0f;
            const float SCALE_FACTOR = 0.5f;

            // Check if in camera & not negative ending scale
            if (cam.GetTopLeftCorner().Y > pos.Y + SAFE_VALUE
                || endingScale <= 0.0f)
                return;

            // Compute scale
            float scale = 1.0f +
                SCALE_FACTOR / 2.0f + (float)Math.Sin(scaleTimer) * SCALE_FACTOR/2.0f;

            // Compute wave
            float s = (float)Math.Sin(shineTimer);
            float wave = s * WAVE;

            // Draw
            g.Push();
            g.Translate(pos.X, pos.Y + wave);
            g.Scale(scale * endingScale, scale * endingScale);
            g.BeginDrawing();

            // Draw shining
            float shine = s * 0.25f + 0.25f;
            g.SetColor(1, 1, 1, shine);
            g.DrawBitmapRegion(bmpStar, 128, 0, 128, 128, -64, -64);

            // Draw base
            g.SetColor();
            g.DrawBitmapRegion(bmpStar, 0, 0, 128, 128, -64, -64);

            g.Pop();
            g.EndDrawing();
        }


        // Get ending scale
        public float GetEndingScale()
        {
            return endingScale;
        }
    }
}
