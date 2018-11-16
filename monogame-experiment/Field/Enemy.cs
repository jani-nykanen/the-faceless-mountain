using System;

using Microsoft.Xna.Framework;
using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // Base enemy class
    public class Enemy : GameObject
    {
        // Global bitmaps
        static private Bitmap bmpEnemy;
        // Global samples
        static protected Sample sThwomp;

        // Audio reference
        static protected AudioManager audio;


        // Initialize global content
        static public void Init(AssetPack assets, AudioManager audio)
        {
            // Bitmaps
            bmpEnemy = assets.GetBitmap("enemy");
            // Samples
            sThwomp = assets.GetSample("thwomp");

            Enemy.audio = audio;
        }


        // Sprite dimensions
        const int SPRITE_WIDTH = 96;
        const int SPRITE_HEIGHT = 96;

        // Does exists
        private bool exist;
        // If in the camera
        protected bool inCamera;

        // Sprite
        protected Sprite spr;
        // Flip
        protected Graphics.Flip flip;

        // Movement direction
        protected int moveDir;
        // Is "hooked"
        protected bool hooked;

        // Start position
        protected Vector2 startPos;

        // Wave timer
        protected float wave;
        // Wave speed
        protected float waveSpeed;
        // Is waving enabled
        protected bool waving;


        // Constructor
        public Enemy(float x, float y, Graphics.Flip flip = Graphics.Flip.None)
        {
            const float DEFAULT_WAVE_SPEED = 0.15f;

            pos.X = x;
            pos.Y = y;
            startPos = pos;

            wave = (float)((new Random()).NextDouble() * Math.PI * 2);
            waveSpeed = DEFAULT_WAVE_SPEED;
            waving = true;

            exist = true;
            inCamera = true;
            getCollision = true;

            width = Stage.TILE_SIZE;
            height = Stage.TILE_SIZE;

            speed.X = 0.0f;
            speed.Y = 0.0f;
            target.X = 0.0f;
            target.Y = 0.0f;
            acc.X = 1.0f;
            acc.Y = 1.0f;

            moveDir = 1;
           
            // Create sprite
            spr = new Sprite(SPRITE_WIDTH, SPRITE_HEIGHT);
            this.flip = flip;

            // Animate (to get the correct frame for rendering
            // before updated)
            Animate(0.0f);
        }


        // Update "AI"
        virtual protected void UpdateAI(float tm) { }

        // Animate
        virtual protected void Animate(float tm) {}

        // Player event
        virtual protected void OnPlayerEvent(Player pl) {}

        // Camera event
        virtual protected void OnCameraEvent(Camera cam) {}


        // Update
        override public void Update(float tm, InputManager input = null)
        {
            if (!exist) return;

            // Update AI
            UpdateAI(tm);
            // Move
            Move(tm);
            // Animate
            Animate(tm);

            // Update wave
            if (waving)
            {
                wave += waveSpeed * tm;
                wave %= (float)(Math.PI * 2.0f);
            }
            else
            {
                wave = 0.0f;
            }

            // Determine movement direction
            moveDir = target.X >= 0.0f ? 1 : -1;
        }


        // Draw
        override public void Draw(Graphics g)
        {
            const float AMPLITUDE = 5.0f;

            if (!exist || !inCamera) return;

            float w = waving ? (float)Math.Sin(wave) * AMPLITUDE : 0.0f;

            g.Push();
            g.Translate(pos.X, pos.Y + w);
            g.BeginDrawing();

            // Draw sprite
            // g.FillRect(-s / 2, -s, s, s);
            spr.Draw(g, bmpEnemy, -spr.GetWidth() / 2, 
                     -Stage.TILE_SIZE - (spr.GetHeight()-Stage.TILE_SIZE)/2,
                     flip);

            g.SetColor();
            g.EndDrawing();

            g.Pop();

        }


        // Check camera
        public void CheckCamera(Camera cam)
        {

            // Custom camera event
            OnCameraEvent(cam);

            // Check if in the camera
            Vector2 topLeft = cam.GetTopLeftCorner();
            Vector2 bottomRight = cam.GetBottomRightCorner();

            float middleY = pos.Y - Stage.TILE_SIZE / 2;

            inCamera = pos.X + spr.GetWidth() / 2 > topLeft.X
                          && pos.X - spr.GetWidth() / 2 < bottomRight.X
                          && middleY + spr.GetHeight() / 2 > topLeft.Y
                          && middleY - spr.GetHeight() / 2 < bottomRight.Y;
        }


        // Player collision
        public void GetPlayerCollision(Player pl, float tm)
        {
            // Custom event
            OnPlayerEvent(pl);

            // Note: we do not use widht, height here
            const float HURT_SIZE = 40.0f;

            // Tongue collision
            GetTongueCollision(pl.GetTongue(), tm);

            // Player collision
            pl.GetHurtCollision(pos.X - HURT_SIZE / 2, 
                                pos.Y - Stage.TILE_SIZE + HURT_SIZE / 2,
                                HURT_SIZE, HURT_SIZE);

        }


        // Tongue collision
        public void GetTongueCollision(Tongue t, float tm)
        {
            const float TONGUE_COL_SIZE = Stage.TILE_SIZE;

            // Top left
            Vector2 tl = new Vector2(pos.X - TONGUE_COL_SIZE / 2, 
                                     pos.Y - Stage.TILE_SIZE/2 - TONGUE_COL_SIZE / 2);
            // Bottom right
            Vector2 br = new Vector2(pos.X + TONGUE_COL_SIZE / 2, 
                                     pos.Y - Stage.TILE_SIZE / 2 + TONGUE_COL_SIZE / 2);

            // Check if hooked
            hooked = t.GetCollisionObject() == this;

            t.SetCollisionObject(this);

            // Collide
            t.GetFloorCollision(tl.X, tl.Y, TONGUE_COL_SIZE, tm);
            t.GetCeilingCollision(tl.X, br.Y, TONGUE_COL_SIZE, tm);
            t.GetWallCollision(tl.X, tl.Y, TONGUE_COL_SIZE, 1, tm);
            t.GetWallCollision(br.X, tl.Y, TONGUE_COL_SIZE, -1, tm);

            t.SetCollisionObject(null);

        }
    }
}
