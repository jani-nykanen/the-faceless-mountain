using System;

using monogame_experiment.Desktop.Core;
using monogame_experiment.Desktop.Field;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop
{
    // Ttile screen scene
    public class Title : Scene
    {
        // Intro time
        private const float INTRO_TIME = 150.0f;
        // Logo fade time
        private const float LOGO_FADE = 60.0f;
        // Title music vol
        private const float MUSIC_VOL = 0.30f;

        // Bitmaps
        private Bitmap bmpFont;
        private Bitmap bmpLogo;
        private Bitmap bmpIntro;
        // Samples
        private Sample sPause;
        private Sample sMenuMusic;

        // Scale
        private float scale;
        // Is changing to game
        private bool changingToGame;
        // Phase
        private int phase;
        // Pause alpha timer
        private float alphaTimer;
        // Phase timer
        private float phaseTimer;

        // Stage ("shallow")
        private Stage stage;

        // Menu
        private Menu menu;
        // Pause (for settings only)
        private Pause settings;

        // Transition
        private Transition trans;


        // Initialize
        public override void Init()
        {
            const float FADE_SPEED = 1.0f;

            // Get global components
            Global gs = (Global)globalScene;
            AssetPack assets = gs.GetAssets();
            trans = gs.GetTransition();

            // Get assets
            bmpFont = assets.GetBitmap("font");
            bmpLogo = assets.GetBitmap("logo");
            bmpIntro = assets.GetBitmap("intro");
            sPause = assets.GetSample("pause");
            sMenuMusic = assets.GetSample("menu");

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

                        // Fade out music
                        audio.FadeCurrentLoopedSample(500, 0.0f);

                        // Change to game
                        trans.Activate(Transition.Mode.In, 2.0f, delegate {
                            sceneMan.ChangeScene("game");
                    });
                    },
                    // Settings
                    delegate(Object self)
                    {
                        settings.Activate(true);
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

            // Create "settings"
            settings = new Pause(this);

            // Set some default values
            scale = 1.0f;
            phase = -1;
            alphaTimer = (float)Math.PI / 2.0f;

            // Fade out
            trans.Activate(Transition.Mode.Out, FADE_SPEED, null);
        }


        // Update
        public override void Update(float tm)
        {
            const float SCALE_SPEED = 0.02f;
            const float ALPHA_SPEED = 0.05f;

            // Update settings if active
            if(settings.IsActive())
            {
                settings.Update(input, audio);
                return;
            }

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

            if(phase == -1)
            {
                // If enter/x is down, the intro is speeded up
                float phaseSpeed = (input.GetButton("start") == State.Down
                                    || input.GetButton("fire1") == State.Down)
                    ? 2.0f : 1.0f;

                // Update intro time
                if((phaseTimer += phaseSpeed * tm) >= INTRO_TIME*2)
                {
                    ++ phase;
                    phaseTimer = LOGO_FADE;

                    // Fade in music
                    audio.FadeSample(sMenuMusic, 1000, 0.0f, MUSIC_VOL, true);
                }
            }
            else if (phase == 0)
            {
                // Update text alpha
                alphaTimer += ALPHA_SPEED * tm;

                // Update logo fading
                if (phaseTimer > 0.0f)
                {
                    phaseTimer -= 1.0f * tm;
                }
                else
                {

                    // Wait for enter/start pressed
                    if (input.GetButton("start") == State.Pressed)
                    {
                        ++phase;
                        audio.PlaySample(sPause, 0.90f);
                    }
                }
            }
            else
            {

                // Update menu
                menu.Update(input, audio, this);
            }

            // Escape pressed
            if(phase != -1 && input.GetButton("cancel") == State.Pressed)
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

            const float INTRO_SCALE = 1.5f;
            const float INTRO_FADE = 30.0f;

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

            // Draw logo & copyright
            int w, h;
            if (phase > -1)
            {
                float alpha = 1.0f;
                if(phaseTimer > 0.0f)
                {
                    alpha = 1.0f - phaseTimer / LOGO_FADE;
                }

                g.BeginDrawing();

                // Draw logo
                w = (int)(bmpLogo.GetWidth() * LOGO_SCALE);
                h = (int)(bmpLogo.GetHeight() * LOGO_SCALE);

                g.SetColor(1, 1, 1, alpha);
                g.DrawScaledBitmap(bmpLogo, (int)view.X / 2 - w / 2, LOGO_Y, w, h);

                // Draw copyright
                g.SetColor(1, 1, 0.5f, alpha);
                g.DrawText(bmpFont, "(c)2018 Jani Nyk~nen",
                           (int)view.X / 2, (int)view.Y - COPYRIGHT_YPOS, -26, 0, COPYRIGHT_SCALE, true);

                g.EndDrawing();
            }

            // Intro phase
            // TODO: Split to smaller methods?
            if(phase == -1)
            {
                int p = phaseTimer <= INTRO_TIME ? 0 : 256;

                w = bmpIntro.GetWidth() / 2;
                h = bmpIntro.GetHeight();

                int sw = (int)(w * INTRO_SCALE);
                int sh = (int)(h * INTRO_SCALE);

                // Compute alpha
                float t = phaseTimer % INTRO_TIME;
                float alpha = 1.0f;
                if(t < INTRO_FADE)
                {
                    alpha = t / INTRO_FADE;
                }
                else if(t >= INTRO_TIME - INTRO_FADE)
                {
                    alpha = 1.0f - (t - (INTRO_TIME-INTRO_FADE)) / INTRO_FADE;
                }

                g.BeginDrawing();

                // Draw intro bitmap
                g.SetColor(1, 1, 1, alpha);
                g.DrawScaledBitmapRegion(bmpIntro, p, 0, w, h, (int)view.X / 2 - sw / 2,
                                   (int)view.Y / 2 - sh / 2, sw, sh);

                g.EndDrawing();

            }
            // "Press something" phase
            else if (phase == 0 && phaseTimer <= 0.0f)
            {
                g.BeginDrawing();

                g.SetColor(1, 1,0,
                           (float)Math.Sin(alphaTimer) * 0.5f + 0.5f);
                g.DrawText(bmpFont, "Press Enter to Start",
                           (int)view.X / 2, PRESS_ENTER_Y, -26, 0, MENU_SCALE, true);

                g.SetColor();
                g.EndDrawing();
            }
            // Menu phase
            else if(phase == 1)
            {
                // Draw menu
                menu.Draw(g, view.X / 2, view.Y * MENU_Y, MENU_SCALE);
            }

            // Draw settings
            settings.Draw(g);
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

            // Fade in music
            audio.FadeSample(sMenuMusic, 1000, 0.0f, MUSIC_VOL, true);
        }
    }
}
