using System;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop
{
    // A simple menu
    public class Menu
    {
        // Global bitmaps
        static private Bitmap bmpFont;
        // Global samples
        static private Sample sSelect;
        static private Sample sAccept;


        // Initialize global content
        static public void Init(AssetPack assets)
        {
            bmpFont = assets.GetBitmap("font");

            sSelect = assets.GetSample("select");
            sAccept = assets.GetSample("accept");
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
        public void Update(InputManager input, AudioManager audio, Object self = null)
        {
            const float DELTA = 0.25f;

            float sdelta = input.GetStickDelta().Y;
            float stick = input.GetStick().Y;

            int old = cursorPos;
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

            // If cursor pos changed, play sound
            if(old != cursorPos)
            {
                audio.PlaySample(sSelect, 1.0f);
            }

            // Check button down
            if(input.GetButton("start") == State.Pressed ||
               input.GetButton("fire1") == State.Pressed)
            {
                // Callback
                if(cbs[cursorPos] != null)
                {
                    cbs[cursorPos](self);
                }

                // Sound
                audio.PlaySample(sAccept, 0.90f);
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


        // Rename a button
        public void RenameButton(int index, String name)
        {
            if (index < 0 || index >= text.Length) return;

            text[index] = name;
        }
    }
}
