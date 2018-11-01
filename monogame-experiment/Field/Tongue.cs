using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;


namespace monogame_experiment.Desktop.Field
{
    // Player tongue
    public class Tongue : GameObject
    {

        // After this time the tongue will disappear
        const float DISAPPEAR_TIMER = 30.0f;


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


        // Constructor
        public Tongue()
        {
            exist = false;
            stuck = false;

            width = 2.0f;
            height = 2.0f;

            centerY = 1.0f;
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

            g.SetColor(1, 0.5f, 0.5f);
            g.FillRect(0, -HEIGHT / 2, (int)length, HEIGHT);
            g.SetColor();

            g.EndDrawing();

            g.Pop();

            // Draw rectangle in the true position
            g.Push();

            g.Translate(pos.X, pos.Y);
            g.BeginDrawing();

            g.SetColor(1, 0, 0);
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
    }
}
