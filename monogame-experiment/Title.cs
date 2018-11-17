using System;

using monogame_experiment.Desktop.Core;
using monogame_experiment.Desktop.Field;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop
{
    // Ttile screen scene
    public class Title : Scene
    {
        // Bitmaps
        private Bitmap bmpFont;
        private Bitmap bmpLogo;
        // Samples
        private Sample sPause;

        // Scale
        private float scale;

        // Stage ("shallow")
        private Stage stage;


        // Transition
        private Transition trans;

        // Initialize
        public override void Init()
        {
            // Get global components
            Global gs = (Global)globalScene;
            AssetPack assets = gs.GetAssets();
            trans = gs.GetTransition();

            // Get assets
            bmpFont = assets.GetBitmap("font");
            bmpLogo = assets.GetBitmap("logo");
            sPause = assets.GetSample("pause");

            // Create a "shallow" stage
            stage = new Stage(assets, true);

            scale = 1.0f;
        }


        // Update
        public override void Update(float tm)
        {
            // Update stage background
            stage.UpdateBackground(tm);
        }


        // Draw
        public override void Draw(Graphics g)
        {
            const float LOGO_SCALE = 1.5f;
            const int LOGO_Y = 64;

            // Clear to black
            g.ClearScreen(0, 0, 0);

            // Set view
            g.FitViewHeight(720.0f);
            Vector2 view = g.GetViewport();
            g.Identity();
            g.IdentityWorld();
            g.Translate(view.X / 2, view.Y / 2);
            g.Scale(scale, scale);
            g.Translate(-view.X / 2, -view.Y / 2);

            // Draw background
            stage.DrawBackground(g);

            g.BeginDrawing();

            // Draw logo
            int w = (int)(bmpLogo.GetWidth() * LOGO_SCALE);
            int h = (int)(bmpLogo.GetHeight()*LOGO_SCALE);
            g.DrawScaledBitmap(bmpLogo, (int)view.X / 2 - w/2, LOGO_Y, w, h);

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
            return "title";
        }


        // On change
        public override void OnChange(String target, Object data = null)
        {
            scale = 1.0f;
            trans.Activate(Transition.Mode.Out, 2.0f, null);
        }
    }
}
