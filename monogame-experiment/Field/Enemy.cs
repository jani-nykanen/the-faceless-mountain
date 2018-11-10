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

        // Initialize global content
        static public void Init(AssetPack assets)
        {
            bmpEnemy = assets.GetBitmap("enemy");
        }


        // Sprite dimensions
        const int SPRITE_WIDTH = 96;
        const int SPRITE_HEIGHT = 96;

        // Does exists
        private bool exist;
        // If in the camera
        private bool inCamera;

        // Sprite
        protected Sprite spr;

        // Movement direction
        protected int moveDir;


        // Constructor
        public Enemy(float x, float y)
        {
            pos.X = x;
            pos.Y = y;
            exist = true;
            inCamera = true;

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
        }


        // Update "AI"
        virtual protected void UpdateAI(float tm) { }

        // Animate
        virtual protected void Animate(float tm) {}


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

            // Determine movement direction
            moveDir = target.X >= 0.0f ? 1 : -1;
        }


        // Draw
        override public void Draw(Graphics g)
        {
            if (!exist || !inCamera) return;

            g.Push();
            g.Translate(pos.X, pos.Y);

            g.BeginDrawing();

            // Draw sprite
            // g.FillRect(-s / 2, -s, s, s);
            spr.Draw(g, bmpEnemy, -spr.GetWidth() / 2, -Stage.TILE_SIZE - (spr.GetHeight()-Stage.TILE_SIZE)/2);

            g.SetColor();
            g.EndDrawing();

            g.Pop();

        }


        // Check camera
        public void CheckCamera(Camera cam)
        {

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
