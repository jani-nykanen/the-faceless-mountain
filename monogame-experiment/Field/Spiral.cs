using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;


namespace monogame_experiment.Desktop.Field
{
    // Spiral
    public class Spiral
    {
        // Bitmaps
        private Bitmap bmpSpiral;

        // Position
        private Vector2 pos;
        // Angle
        private float angle;
        // Scale
        private float scale;
        // Does exists
        private bool exist;


        // Constructor
        public Spiral(AssetPack assets)
        {
            bmpSpiral = assets.GetBitmap("spiral");

            exist = false;
            scale = 1.0f;
            angle = 0.0f;
            pos = Vector2.Zero;
        }


        // Create
        public void Create(Vector2 pos)
        {
            this.pos = pos;

            scale = 1.0f;
            angle = 0.0f;
            exist = true;
        }


        // Update
        public void Update(float tm, bool die = false)
        {
            const float ROTATE_SPEED = 0.05f;
            const float SINK_SPEED = 0.033f;

            if (!exist)
                return;

            angle += ROTATE_SPEED * tm;
            // If dying, sink
            if (die)
            {
                scale -= SINK_SPEED * tm;
                if(scale <= 0.0f)
                {
                    scale = 0.0f;
                    exist = false;
                }
            }
        }


        // Draw
        public void Draw(Graphics g)
        {
            const int BASE_SCALE = 144;

            if (!exist) return;

            // Set color
            g.SetColor(1, 1, 1, scale);

            g.Push();
            g.Translate(pos.X, pos.Y);
            g.Rotate(angle);
            g.Scale(scale, scale);

            g.BeginDrawing();
            g.DrawScaledBitmap(bmpSpiral, -BASE_SCALE / 2, -BASE_SCALE / 2, 
                               BASE_SCALE, BASE_SCALE);
            g.EndDrawing();
            g.Pop();

            g.SetColor();
        }
    }
}
