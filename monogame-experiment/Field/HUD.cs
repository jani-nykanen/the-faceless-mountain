using System;

using monogame_experiment.Desktop.Core;

namespace monogame_experiment.Desktop.Field
{
    // Game HUD. Also handles time
    public class HUD
    {
        const float GUIDE_TIME = 480.0f;

        // Bitmaps
		private Bitmap bmpFont;
        private Bitmap bmpGuide;

        // Time
        private float time;
        // Guide timer 
        private float guideTimer;


        // Draw metres
        private void DrawMetres(Graphics g, float metres)
        {
            const int XOFF = -20;
            const float SCALE = 0.70f;
            const int YPOS = 56;

            float m = (float)Math.Floor(metres * 10) / 10.0f;
            String s = "DISTANCE: " +  m.ToString();
            if(s[s.Length-2] != '.')
            {
                s += ".0";
            }
            s += "m";

            g.DrawText(bmpFont, s,
                       (int)g.GetViewport().X / 2,
                       (int)g.GetViewport().Y - YPOS,
                       XOFF, 0, SCALE, true);
        }


        // Draw guide
        private void DrawGuide(Graphics g, bool forceDrawGuide = false)
        {

            if (!forceDrawGuide && guideTimer <= 0.0f) return;

            const int XOFF = 32;
            const int YPOS_LEFT = 80;
            const int YPOS_RIGHT = 144;
            const int YOFF = 192;
            const int SCALE = 144;

            const float FADE_TIME = 60.0f;
            const float BASE_ALPHA = 0.85f;

            float alpha = BASE_ALPHA;
            float x = XOFF;
            if(!forceDrawGuide && guideTimer < FADE_TIME)
            {
                float t = guideTimer / FADE_TIME;
                alpha *= t;
                x = XOFF * t;
            }

            g.SetColor(1, 1, 1, alpha);

            // Left side
            for (int i = 0; i < 3; ++ i)
            {
                g.DrawScaledBitmapRegion(bmpGuide, i * 128, 0, 128, 128,
                                   (int)x, YPOS_LEFT + i * YOFF, SCALE, SCALE);
            }


            // Right side
            float vw = g.GetViewport().X;
            for (int i = 0; i < 2; ++i)
            {
                g.DrawScaledBitmapRegion(bmpGuide, i * 128, 128, 128, 128,
                                   vw-SCALE- (int)x, YPOS_RIGHT + i * YOFF, SCALE, SCALE);
            }
        }


        // Constructor
        public HUD(AssetPack assets)
        {
            // Get bitmaps
			bmpFont = assets.GetBitmap("font");
            bmpGuide = assets.GetBitmap("guide");

            guideTimer = GUIDE_TIME;
        }


        // Reset
        public void Reset()
        {
            guideTimer = GUIDE_TIME;
        }


        // Update
        public void Update(float tm, bool updateTime = true)
        {
            // Update time
            if (updateTime)
            {
                time += 1.0f * tm;
            }

            // Update guide timer
            if(guideTimer > 0.0f)
                guideTimer -= 1.0f * tm;
        }


        // Draw
        public void Draw(Graphics g, float starDistance = 0.0f, bool forceDrawGuide = false)
        {
            const float ALPHA = 0.80f;

            const int XOFF = -20;
            const float TIME_SCALE = 1.0f;
            const float TEXT_SCALE = 0.75f;
            const int TIME_YPOS = 48;
            const int TEXT_YPOS = 8;

            g.BeginDrawing();

            float vw = g.GetViewport().X;

            // Draw time texts
            g.SetColor(1.0f, 1.0f, 0.5f, ALPHA);
            g.DrawText(bmpFont, "TIME", (int)(vw / 2), TEXT_YPOS, XOFF, 0, TEXT_SCALE, true);
            g.DrawText(bmpFont, GetTimeString(), (int)(vw / 2), TIME_YPOS, XOFF, 0, TIME_SCALE, true);
            // Draw metres
            DrawMetres(g, starDistance);

            // Draw guide
            g.SetColor();
            DrawGuide(g, forceDrawGuide);

            g.EndDrawing();
        }


        // Get time string
        public String GetTimeString()
        {
            // Compute minutes, seconds & 1/100 seconds
            int t = (int)Math.Floor(time / 60.0);
            int sec = t % 60;
            int min = (int)Math.Floor(t / 60.0f);
            int cent = (int)(time % 60);
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
    }
}
