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

        // Scale timer
        private float scaleTimer;
        // Shine timer
        private float shineTimer;


        // Constructor
        public Star(Vector2 pos)
        {
            this.pos = pos;
            scaleTimer = 0.0f;
            shineTimer = 0.0f;
        }


        // Update
        public void Update(float tm)
        {
            const float SCALE_SPEED = 0.05f;
            const float SHINE_SPEED = 0.025f;

            scaleTimer += SCALE_SPEED * tm;
            shineTimer += SHINE_SPEED * tm;
        }


        // Draw
        public void Draw(Graphics g, Camera cam)
        {
            const float SAFE_VALUE = 128;
            const float WAVE = 8.0f;
            const float SCALE_FACTOR = 0.5f;

            // Check if in camera
            if (cam.GetTopLeftCorner().Y > pos.Y + SAFE_VALUE)
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
            g.Scale(scale, scale);
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
    }
}
