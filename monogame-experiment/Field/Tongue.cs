using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;


namespace monogame_experiment.Desktop.Field
{
    // Player tongue
    public class Tongue : GameObject
    {
        // Global bitmaps
        static private Bitmap bmpTongue;

        // Initialize global content
        static public void Init(AssetPack assets)
        {
            bmpTongue = assets.GetBitmap("tongue");
        }


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
        // Rendering length
        private float renderLength;

        // Tip size
        private float tipSize;
        // Tip angle
        private float tipAngle;
        // Tip flip
        private bool flipTip;

        // Collision object
        private GameObject colObj = null;


        // Constructor
        public Tongue()
        {
            exist = false;
            stuck = false;
            returning = false;

            width = 2.0f;
            height = 2.0f;

            centerY = 1.0f;
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

            retSpeed = (float)Math.Sqrt(speed.X * speed.X + speed.Y * speed.Y);

            timer = 0.0f;

            exist = true;
            stuck = false;

        }


        // Update
        public override void Update(float tm, InputManager input = null)
        {
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

                // Calculate rendering length
                renderLength = (float)Math.Sqrt(
                     Math.Pow(pos.X - startPos.X, 2)
                   + Math.Pow(pos.Y - startPos.Y, 2));

                return;
            }

            // If not stuck, update timer & move
            if (!stuck)
            {
                // Store total speed for future
                retSpeed = totalSpeed;

                // Move
                Move(tm);

                // Update timer
                timer += 1.0f * tm;
                if(timer >= DISAPPEAR_TIMER || input.GetButton("fire3") == State.Up
                   || input.GetButton("fire3") == State.Released)
                {
                    returning = true;
                }
            }
            // Or if stuck and the tongue button released,
            // disappear
            else
            {

                if(input.GetButton("fire3") == State.Up || input.GetButton("fire3") == State.Released)
                {
                    returning = true;
                    stuck = false;
                }

                // Check if stuck on an object
                if(colObj != null)
                {
                    pos.X += colObj.GetSpeed().X * tm;
                    pos.Y += colObj.GetSpeed().Y * tm;
                }
            }

            // Calculate rendering length
            renderLength = (float)Math.Sqrt(
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

            const int HEIGHT = 24;
            const float SCALE_FACTOR = 1.5f;

            // Calculate angle
            float angle = (float)Math.Atan2(pos.Y - startPos.Y, pos.X - startPos.X);

            // Set transform
            g.Push();
            g.Translate(startPos.X, startPos.Y);
            g.Rotate(angle);

            g.BeginDrawing();

            // Draw integer parts
            int i = 0;
            for (; i < (int)(renderLength / 64); ++ i)
            {
                g.DrawScaledBitmapRegion(bmpTongue, 0, i == 0 ? 32 : 0, 64, 32,
                                         i * 64, -HEIGHT/2, 64, HEIGHT);
            }
            // Draw remainder
            int rem = (int)renderLength - (int)(renderLength / 64)*64;
            g.DrawScaledBitmapRegion(bmpTongue, 0, i == 0 ? 32 : 0, rem, 32,
                                         i * 64, -HEIGHT / 2, rem, HEIGHT);
                                     
            g.EndDrawing();
            g.Pop();

            // Draw "tip" in the true position
            g.Push();

            g.Translate(pos.X, pos.Y);
            g.Rotate(tipAngle);
            g.Scale(flipTip ? -1 : 1, 1);
            g.BeginDrawing();

            int w = (int)(tipSize * SCALE_FACTOR);
            int h = w;

            // "Tip"
            g.SetColor();
            g.DrawScaledBitmapRegion(bmpTongue, 64, 0, 64, 64,
                                     -w / 2, -h / 2, w, h);

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


        // Set tongue tip information
        public void SetTip(float size, float angle, bool flip)
        {
            tipSize = size;
            tipAngle = angle;
            flipTip = flip;
        }


        // Kill your tongue
        public void Kill(bool soft = false)
        {
            if(!soft)
            {
                exist = false;
                stuck = false;
                returning = false;
            }
            else if(exist)
            {
                stuck = false;
                returning = true;
            }
        }
    

        // Set collision object
        public void SetCollisionObject(GameObject o)
        {
            if (stuck) return;
            colObj = o;
        }


        // Get collision object
        public GameObject GetCollisionObject()
        {
            return colObj;
        }
    }
}
