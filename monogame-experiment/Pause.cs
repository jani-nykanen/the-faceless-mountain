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
                    null, 
                    // Quit
                    delegate(Object self) 
                    {
                        GameField gf = (GameField)baseScene;
                        gf.StartQuitting();
                    }, 
                }
            );

            settings = new Menu(
                new String[] {
                    "Toggle Fullscreen",
                    "Audio: On",
                    "Back"
                },
                new Menu.Callback[] {
                    null, null, null, null,
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
        public void Update(InputManager input)
        {
            if (!active) return;

            baseMenu.Update(input, (Object)this);
        }


        // Draw
        public void Draw(Graphics g)
        {
            const int PAUSE_WIDTH = 480;
            const int PAUSE_HEIGHT = 320;

            if (!active) return;

            Vector2 view = g.GetViewport();

            // Draw box
            DrawBox(g, PAUSE_WIDTH, PAUSE_HEIGHT);

            // Draw menu
            baseMenu.Draw(g, view.X / 2, view.Y / 2);
        }
    }
}
