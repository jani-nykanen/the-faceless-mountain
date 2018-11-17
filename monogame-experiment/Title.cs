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
        // Is changing to game
        private bool changingToGame;
        // Phase
        private int phase;
        // Pause alpha timer
        private float alphaTimer;

        // Stage ("shallow")
        private Stage stage;

        // Menu
        private Menu menu;

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

            // Create menu
            menu = new Menu(
                new String[]
                {
                    "Play Game",
                    "Settings", 
                "Quit"
                },
                new Menu.Callback[]
                {
                    // Resume
                    delegate(Object self)
                    {
                        changingToGame = true;
                        // Change to game
                        trans.Activate(Transition.Mode.In, 2.0f, delegate {
                            sceneMan.ChangeScene("game");
                    });
                    },
                    // Settings
                    delegate(Object self)
                    {
                        // ...
                    },
                    // Quit
                    delegate(Object self)
                    {
                        changingToGame = false;
                        // Quit
                        trans.Activate(Transition.Mode.In, 2.0f, eventMan.Terminate);
                    },
                }
            );

            // Set some default values
            scale = 1.0f;
            phase = 0;
            alphaTimer = 0.0f;
        }


        // Update
        public override void Update(float tm)
        {
            const float SCALE_SPEED = 0.02f;
            const float ALPHA_SPEED = 0.05f;

            // Update stage background
            stage.UpdateBackground(tm);

            if (trans.IsActive())
            {

                // If moving to game, scale
                if(trans.GetMode() == Transition.Mode.In 
                   && changingToGame)
                {
                    scale += SCALE_SPEED * tm;
                }

                return;
            }

            if (phase == 0)
            {
                // Update text alpha
                alphaTimer += ALPHA_SPEED * tm;

                // Wait for enter/start pressed
                if (input.GetButton("start") == State.Pressed)
                {
                    ++ phase;
                    audio.PlaySample(sPause, 0.90f);
                }
            }
            else
            {

                // Update menu
                menu.Update(input, audio, this);
            }

            // Escape pressed
            if(input.GetButton("cancel") == State.Pressed)
            {
                // Quit
                changingToGame = false;
                trans.Activate(Transition.Mode.In, 2.0f, eventMan.Terminate);
            }
        }


        // Draw
        public override void Draw(Graphics g)
        {
            g.ToggleAutoBeginEnd(false);

            const float LOGO_SCALE = 1.5f;
            const int LOGO_Y = 64;

            const int COPYRIGHT_YPOS = 40;
            const float COPYRIGHT_SCALE = 0.5f;

            const float MENU_Y = 0.80f;
            const float MENU_SCALE = 0.90f;

            const int PRESS_ENTER_Y = 528;

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

            // Draw copyright
            g.SetColor(1, 1, 0.5f);
            g.DrawText(bmpFont, "(c)2018 Jani Nyk~nen", 
                       (int)view.X / 2, (int)view.Y - COPYRIGHT_YPOS, -26, 0, COPYRIGHT_SCALE, true);

            g.EndDrawing();

            if (phase == 0)
            {
                g.BeginDrawing();

                g.SetColor(1, 1,0,
                           (float)Math.Sin(alphaTimer) * 0.5f + 0.5f);
                g.DrawText(bmpFont, "Press Enter to Start",
                           (int)view.X / 2, PRESS_ENTER_Y, -26, 0, MENU_SCALE, true);

                g.SetColor();
                g.EndDrawing();
            }
            else
            {
                // Draw menu
                menu.Draw(g, view.X / 2, view.Y * MENU_Y, MENU_SCALE);
            }
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
            changingToGame = false;
            trans.Activate(Transition.Mode.Out, 2.0f, null);
        }
    }
}
