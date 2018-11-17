using System;

using monogame_experiment.Desktop.Core;
using monogame_experiment.Desktop.Field;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop
{
    // Pause screen. Note that this is not in GameField
    // since we can use the same Settings screen in
    // the title screen, too
    public class Pause
    {

        // Global samples
        private static Sample sCancel;


        // Initialize global content
        public static void Init(AssetPack assets)
        {
            sCancel = assets.GetSample("cancel");
        }


        // Is active
        private bool active;

        // Base menu
        private Menu baseMenu;
        // Settings menu
        private Menu settings;

        // Is in settings
        private bool inSettings;

        // Base scene
        private Scene baseScene;


        // Draw box
        private void DrawBox(Graphics g, int w, int h)
        {
            const float ALPHA = 0.75f;

            Vector2 view = g.GetViewport();
            int mx = (int)view.X / 2;
            int my = (int)view.Y / 2;

            g.BeginDrawing();
            g.SetColor(0, 0, 0, ALPHA);
            g.FillRect(mx - w / 2, my - h / 2, w, h);
            g.SetColor();
            g.EndDrawing();
        }


        // Constructor
        public Pause(Scene baseScene = null)
        {
            active = false;
            this.baseScene = baseScene;

            // Create menus
            baseMenu = new Menu(
                new String[]
                {
                    "Resume", "Restart",
                    "Settings", "Quit"
                },
                new Menu.Callback[]
                {
                    // Resume
                    delegate(Object self)
                    {
                        Pause p = (Pause)self;
                        p.active = false;
                    },
                    // Restart
                    delegate(Object self) 
                    {
                        GameField gf = (GameField)baseScene;
                        gf.Reset();
                    },
                    // Settings
                    delegate(Object self)
                    {
                        Pause p = (Pause)self;
                        p.inSettings = true;
                        p.settings.SetCursorPos(2);
                    },
                    // Quit
                    delegate(Object self) 
                    {
                        GameField gf = (GameField)baseScene;
                        gf.StartQuitting();
                    }, 
                }
            );

            bool audioState = baseScene.IsAudioEnabled();

            // Create settings
            settings = new Menu(
                new String[] {
                    "Fullscreen",
                    "Audio: " + (audioState ? "On" : "Off"),
                    "Back"
                },
                new Menu.Callback[] {
                    // Fullscreen
                    delegate(Object o) { baseScene.ToggleFullscreen(); }, 
                    // Audio
                    delegate(Object o) {
                    Pause p = (Pause)o;
                    p.baseScene.ToggleAudio();
                    bool state = p.baseScene.IsAudioEnabled();
                    p.settings.RenameButton(1,"Audio: " + (state ? "On" : "Off"));

                    }, 
                    // Quit
                    delegate(Object o) { ((Pause)o).inSettings = false; },
                }
            );
        }

        // Disable
        public void Disable()
        {

            active = false;
        }


        // Activate
        public void Activate()
        {
            inSettings = false;
            active = true;
            baseMenu.SetCursorPos(0);
        }


        // Is active
        public bool IsActive()
        {
            return active;
        }


        // Update
        public void Update(InputManager input, AudioManager audio)
        {
            if (!active) return;

            // Check cancel key (back or esc, really)
            if(input.GetButton("cancel") == State.Pressed)
            {
                // Play cancel sound
                audio.PlaySample(sCancel, 0.90f);

                Disable();
                return;
            }

            // Update either settings or the base pause menu
            if (inSettings)
                settings.Update(input, audio, (Object)this);
            else
                baseMenu.Update(input, audio, (Object)this);
        }


        // Draw
        public void Draw(Graphics g)
        {
            const int PAUSE_WIDTH = 480;
            const int PAUSE_HEIGHT = 320;

            const int SETTINGS_WIDTH = 576;
            const int SETTINGS_HEIGHT = 256;

            if (!active) return;

            Vector2 view = g.GetViewport();

            // Draw box
            DrawBox(g, 
                    inSettings ? SETTINGS_WIDTH : PAUSE_WIDTH, 
                    inSettings ? SETTINGS_HEIGHT : PAUSE_HEIGHT);

            // Draw menu or settings
            (inSettings ? settings : baseMenu).Draw(g, view.X / 2, view.Y / 2);
        }
    }
}
