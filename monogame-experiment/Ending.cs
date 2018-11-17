using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop
{
    // Ending scene
    public class Ending : Scene
    {
        // Time
        const float TIME = 300.0f;

        // Bitmaps
        private Bitmap bmpFont;
        // Samples
        private Sample sAccept;

        // Time string
        private String timeString;
        // Ending timer
        private float timer;

        // Transition
        private Transition trans;


        // Get alpha
        private float GetAlpha(float limit, float fadeTime)
        {
            float alpha = 1.0f;
            if (timer >= TIME - fadeTime * limit)
            {
                alpha = 1.0f - (timer - (TIME - fadeTime * limit)) / fadeTime;
            }
            return alpha;
        }


        // Initialize
        public override void Init()
        {
            // Get global components
            Global gs = (Global)globalScene;
            AssetPack assets = gs.GetAssets();
            trans = gs.GetTransition();

            // Get assets
            bmpFont = assets.GetBitmap("font");
            sAccept = assets.GetSample("accept");

            timeString = "";
        }


        // Update
        public override void Update(float tm)
        {
            if (trans.IsActive()) return;

            // Update timer
            if(timer >= 0.0f)
            {
                timer -= 1.0f * tm;
            }
            else
            {
                // If something pressed, terminate
                if(input.WasSomethingPressed())
                {
                    audio.PlaySample(sAccept, 0.90f);
                    trans.Activate(Transition.Mode.In, 0.5f,
                                   eventMan.Terminate, 0, 0, 0);
                }
            }
        }


        // Draw
        public override void Draw(Graphics g)
        {
            const float FADE_TIME = 60.0f;

            const int XOFF = -26;
            const int BIG_YOFF = -128;
            const int SMALL_YOFF = 0;
            const int TIME_YOFF = 64;
            const int PRESS_BUTTON_YOFF = 192;
            const float BIG_SCALE = 1.5f;
            const float SMALL_SCALE = 0.75f;
            const float TIME_SCALE = 1.0f;

            // Clear to black
            g.ClearScreen(0, 0, 0);
            g.FitViewHeight(720.0f);
            g.Identity();
            g.IdentityWorld();

            g.BeginDrawing();

            // Draw "Congratulations"
            Vector2 view = g.GetViewport();
            g.SetColor(1, 1, 0.5f);
            g.DrawText(bmpFont, "CONGRATULATIONS!", 
                       (int)view.X / 2, (int)view.Y / 2 + BIG_YOFF,
                       XOFF, 0, BIG_SCALE, true);

            // Draw "you beat the game in" blah blah blah
            g.SetColor();
            if (timer <= TIME - FADE_TIME)
            {
                g.SetColor(1, 1, 1, GetAlpha(2, FADE_TIME));
                g.DrawText(bmpFont, "You conquered the mountain in",
                       (int)view.X / 2, (int)view.Y / 2 + SMALL_YOFF,
                       XOFF, 0, SMALL_SCALE, true);
            }

            // Draw time
            if (timer <= TIME - FADE_TIME * 2)
            {
                g.SetColor(1, 1, 1, GetAlpha(3, FADE_TIME));
                g.DrawText(bmpFont, timeString,
                       (int)view.X / 2, (int)view.Y / 2 + TIME_YOFF,
                       XOFF, 0, TIME_SCALE, true);
            }


            // Draw "press any button"
            if (timer <= TIME - FADE_TIME * 4)
            {
                g.SetColor(1, 1, 0, GetAlpha(5, FADE_TIME));
                g.DrawText(bmpFont, "Press something to continue",
                       (int)view.X / 2, (int)view.Y / 2 + PRESS_BUTTON_YOFF,
                           XOFF, 0, SMALL_SCALE, true);
            }

            g.EndDrawing();
        }


        // Destroy
        public override void Destroy()
        {
            // ...
        }


        // Get name
        public override string GetName()
        {
            return "ending";
        }


        // On change
        public override void OnChange(String target, Object data = null)
        {
            timeString = (String)data;
            timer = TIME;

            trans.Activate(Transition.Mode.Out, 1.0f, null, 1, 1, 1);
        }
    }
}
