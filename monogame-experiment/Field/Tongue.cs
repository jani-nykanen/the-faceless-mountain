using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;


namespace monogame_experiment.Desktop.Field
{
    // Player tongue
    public class Tongue : GameObject
    {

        // After this time the tongue will disappear
        const float DISAPPEAR_TIMER = 20.0f;


        // Does exists
        private bool exist;
        // Is stuck
        private bool stuck;
        // Existence timer
        private float timer;

        // Starting position
        private Vector2 startPos;
        // Length
        private float length;

        // Flicker timer (it's beam, after all!)
        private float flicker;


        // Constructor
        public Tongue()
        {
            exist = false;
            stuck = false;

            width = 2.0f;
            height = 2.0f;

            centerY = 1.0f;
            flicker = 0.0f;
        }


        // On any collision
        override protected void OnAnyCollision(float x, float y)
        {
            if (!exist || stuck) return;

            stuck = true;

            this.pos.X = x;
            this.pos.Y = y;

            this.speed = Vector2.Zero;
            this.target = Vector2.Zero;
        }


        // Create
        public void Create(Vector2 pos, Vector2 speed)
        {
            this.pos = pos;
            startPos = pos;
            this.speed = speed;
            target = speed;

            timer = 0.0f;

            exist = true;
            stuck = false;

        }


        // Update
        public override void Update(float tm, InputManager input = null)
        {
            const float FLICKER_SPEED = 0.2f;

            if (!exist) return;

            // If not stuck, update timer & move
            if(!stuck)
            {
                // Move
                Move(tm);

                // Update timer
                timer += 1.0f * tm;
                if(timer >= DISAPPEAR_TIMER)
                {
                    exist = false;
                }
            }
            // Or if stuck and the tongue button released,
            // disappear
            else
            {

                if(input.GetKey("fire3") == State.Up)
                {
                    exist = false;
                }
            }

            // Calculate length
            length = (float)Math.Sqrt(
                  Math.Pow(pos.X - startPos.X, 2) 
                + Math.Pow(pos.Y - startPos.Y, 2));

            // Update flickering
            flicker += FLICKER_SPEED * tm;
        }


        // Update starting position
        public void UpdateStartPos(Vector2 p)
        {
            startPos = p;
        }


        // Draw
        public override void Draw(Graphics g)
        {
            if (!exist) return;

            const int HEIGHT = 12;

            // Calculate angle
            float angle = (float)Math.Atan2(pos.Y - startPos.Y, pos.X - startPos.X);

            // Set transform
            g.Push();
            g.Translate(startPos.X, startPos.Y);
            g.Rotate(angle);

            g.BeginDrawing();

            float alpha = (float)Math.Sin(flicker) * 0.25f + 0.75f;
            g.SetColor(0.66f, 0.40f, 1.0f, alpha);
            g.FillRect(0, -HEIGHT / 2, (int)length, HEIGHT);
            g.SetColor();

            g.EndDrawing();

            g.Pop();

            // Draw rectangle in the true position
            g.Push();

            g.Translate(pos.X, pos.Y);
            g.BeginDrawing();

            g.SetColor(0.75f, 0, 1);
            g.FillRect(-16, -16, 32, 32);
            g.SetColor();

            g.EndDrawing();

            g.Pop();
        }


        // Does exists
        public bool DoesExist()
        {
            return exist;
        }


        // Is the tongue stuck
        public bool IsStuck()
        {
            return exist && stuck;
        }
    }
}
