using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;


namespace monogame_experiment.Desktop.Field
{
    // Player tongue
    public class Tongue : GameObject
    {

        // After this time the tongue will disappear
        const float DISAPPEAR_TIMER = 17.5f;


        // Does exists
        private bool exist;
        // Is stuck
        private bool stuck;
        // Is returning
        private bool returning;
        // Existence timer
        private float timer;
        // Returning speed
        private float retSpeed;

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
            returning = false;

            width = 2.0f;
            height = 2.0f;

            centerY = 1.0f;
            flicker = 0.0f;
        }


        // On any collision
        override protected void OnAnyCollision(float x, float y)
        {
            if (!exist || returning || stuck) return;

            stuck = true;
            returning = false;

            pos.X = x;
            pos.Y = y;

            speed = Vector2.Zero;
            target = Vector2.Zero;
        }


        // Return back to the player
        private void ReturnBack(float tm)
        {
            float angle = (float)Math.Atan2(pos.Y - startPos.Y, pos.X - startPos.X);

            float delta = retSpeed*tm;

            speed.X = -(float)Math.Cos(angle) * retSpeed;
            speed.Y = -(float)Math.Sin(angle) * retSpeed;

            if(length < delta)
            {
                exist = false;
                stuck = false;
                returning = false;
            }
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

            // Calculate length
            length = (float)Math.Sqrt(
                  Math.Pow(pos.X - startPos.X, 2)
                + Math.Pow(pos.Y - startPos.Y, 2));

            // Return back to the player
            if (returning)
            {
                ReturnBack(tm);
                Move(tm);
                return;
            }
                
            // If not stuck, update timer & move
            if(!stuck)
            {
                // Store total speed for future
                retSpeed = totalSpeed;

                // Move
                Move(tm);

                // Update timer
                timer += 1.0f * tm;
                if(timer >= DISAPPEAR_TIMER)
                {
                    returning = true;
                }
            }
            // Or if stuck and the tongue button released,
            // disappear
            else
            {
                if(input.GetButton("fire3") == State.Up)
                {
                    returning = true;
                    stuck = false;
                }
            }

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

            const int HEIGHT = 8;
            const int TIP_SIZE = 24;

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
            g.FillRect(-TIP_SIZE/2, -TIP_SIZE/2, TIP_SIZE, TIP_SIZE);
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
