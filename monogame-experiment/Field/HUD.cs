using System;

using monogame_experiment.Desktop.Core;

namespace monogame_experiment.Desktop.Field
{
    // Game HUD. Also handles time
    public class HUD
    {
        // Bitmap
		private Bitmap bmpFont;

        // Time
        private float time;


        // Get time string
        private String GetTimeString()
        {

            // Compute minutes, seconds & 1/100 seconds
            int t = (int)Math.Floor(time / 60.0);
            int sec = t % 60;
            int min = (int)Math.Floor(t / 60.0f);
            int cent = (int) (time % 60);
            cent = (int)Math.Floor(100.0f / 60.0f * cent);

            // Create string
            String ret = min.ToString() + ":";
            if (sec < 10) 
                ret += "0";

            ret += sec.ToString() + ":";
            if (cent < 10) 
                ret += "0";
            ret += cent.ToString();

            return ret;
        }


        // Constructor
        public HUD(AssetPack assets)
        {
			bmpFont = assets.GetBitmap("font");
        }


        // Update
        public void Update(float tm)
        {
            // Update time
            time += 1.0f * tm;
        }


        // Draw
        public void Draw(Graphics g)
        {
            const float ALPHA = 0.80f;

            const int XOFF = -20;
            const float TIME_SCALE = 1.0f;
            const float TEXT_SCALE = 0.75f;
            const int TIME_YPOS = 48;
            const int TEXT_YPOS = 8;

            g.BeginDrawing();

            float vw = g.GetViewport().X;

            g.SetColor(1.0f, 1.0f, 0.5f, ALPHA);
            g.DrawText(bmpFont, "TIME", (int)(vw / 2), TEXT_YPOS, XOFF, 0, TEXT_SCALE, true);
            g.DrawText(bmpFont, GetTimeString(), (int)(vw / 2), TIME_YPOS, XOFF, 0, TIME_SCALE, true);


            g.SetColor();
            g.EndDrawing();
        }
    }
}
