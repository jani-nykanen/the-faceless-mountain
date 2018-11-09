using System;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop
{
    // Transition manager
    public class Transition
    {
        // Max time
        const float TIME_MAX = 60.0f;

        // Fading mode
        public enum Mode
        {
            In = 0,
            Out = 1
        };

        // Fade mode
        private Mode mode;
        // Fade speed
        private float speed;
        // Is active
        private bool active;
        // Color
        private float[] color;
        // Time
        private float timer;

        // Callback type
        public delegate void Callback();
        // Callback
        private Callback cb;


        // Constructor
        public Transition()
        {
            mode = Mode.In;
            active = false;
            speed = 1.0f;
            cb = null;
            timer = 0.0f;

            // Create color array
            color = new float[] {
                0, 0, 0
            };
        }


        // Update
        public void Update(float tm)
        {
            if (!active) return;

            // Update timer
            timer -= speed * tm;
            if(timer <= 0.0f)
            {
                // If in
                if(mode == Mode.In)
                {
                    // Call callback
                    if (cb != null)
                        cb();

                    mode = Mode.Out;
                    timer = TIME_MAX;
                }
                else 
                {
                    active = false;
                }
            }
        }


        // Is active
        public bool IsActive()
        {
            return active;
        }


        // Activate
        public void Activate(Mode mode, float speed, Callback cb, 
                             float r = 0, float g = 0, float b = 0)
        {
            active = true;
            this.speed = speed;
            this.mode = mode;
            this.cb = cb;

            this.color[0] = r;
            this.color[1] = g;
            this.color[2] = b;

            timer = TIME_MAX;
        }


        // Draw transition
        public void Draw(Graphics g)
        {
            if (!active) return;

            float t = 1.0f / TIME_MAX * timer;
            if(mode == Mode.In)
            {
                t = 1.0f - t;
            }

            // Fill screen with the color
            g.SetView(1, 1);
            g.BeginDrawing();

            g.SetColor(color[0], color[1], color[2], t);
            g.FillRect(0, 0, 1, 1);

            g.SetColor();
            g.EndDrawing();
        }
    }
}
