using System;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop
{
    // A simple menu
    public class Menu
    {
        // Global bitmaps
        static private Bitmap bmpFont;

        // Initialize global content
        static public void Init(AssetPack assets)
        {
            bmpFont = assets.GetBitmap("font");
        }


        // Callback type
        public delegate void Callback(Object o);

        // Button texts
        private String[] text;
        // Button callbacks
        private Callback[] cbs;
        // Length 
        private int length;

        // Cursor position
        private int cursorPos;


        // Constructor
        public Menu(String[] text, Callback[] cbs)
        {
            this.text = text;
            this.cbs = cbs;

            cursorPos = 0;
            length = text.Length < cbs.Length ? text.Length : cbs.Length;
        }


        // Update
        public void Update(InputManager input, Object self = null)
        {
            const float DELTA = 0.25f;

            float sdelta = input.GetStickDelta().Y;
            float stick = input.GetStick().Y;

            // Up
            if(sdelta < -DELTA && stick < -DELTA)
            {
                if (--cursorPos < 0)
                    cursorPos += length;

            }
            // Down
            else if(sdelta > DELTA && stick > DELTA)
            {
                ++ cursorPos;
                cursorPos %= length;
            }

            // Check button down
            if(input.GetKey("start") == State.Pressed)
            {
                if(cbs[cursorPos] != null)
                {
                    cbs[cursorPos](self);
                }
            }
        }


        // Draw
        public void Draw(Graphics g, float middle, float top, float scale = 1.0f)
        {
            const int XOFF = -20;
            const int YOFF = 64; // Yes it's good idea to fix this shut up
            const float TEXT_SCALE = 1.0f;
            const float TEXT_SCALE_SELECTED = 1.15f;

            g.Push();
            g.Translate(middle, top - length*YOFF / 2);
            g.Scale(scale, scale);
            g.BeginDrawing();

            // Draw button text
            float s = 1.0f;
            for (int i = 0; i < length; ++ i) 
            {
                if (i == cursorPos)
                {
                    g.SetColor(1, 1, 0);
                    s = TEXT_SCALE_SELECTED;
                }
                else
                {
                    g.SetColor(0.9f, 0.9f, 0.9f);
                    s = TEXT_SCALE;
                }

                g.DrawText(bmpFont, text[i], 0, i * YOFF, XOFF, 0, s, true);
            }

            g.EndDrawing();
            g.Pop();
        }


        // Set cursor pos
        public void SetCursorPos(int p)
        {
            if (p < 0) p = 0;
            p %= length;

            cursorPos = p;
        }
    }
}
