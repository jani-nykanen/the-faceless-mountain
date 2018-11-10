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
        private Sprite spr;

        // Movement direction
        protected int moveDir;


        // Constructor
        public Enemy(float x, float y)
        {
            pos.X = x;
            pos.Y = y;
            exist = true;
            inCamera = false;

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

            // Draw sprite
            int s = Stage.TILE_SIZE;
            g.SetColor(1, 0, 0);
            g.FillRect((int)pos.X - s/2, (int)pos.Y - s, s, s);
            g.SetColor();
        }


        // Check camera
        public void CheckCamera(Camera cam)
        {
            // Check if in the camera
            inCamera = true;
        }


        // Player collision
        public void GetPlayerCollision(Player pl)
        {
            const float HURT_SIZE = 48.0f;

            pl.GetHurtCollision(pos.X - HURT_SIZE / 2, 
                                pos.Y - Stage.TILE_SIZE + HURT_SIZE / 2,
                                HURT_SIZE, HURT_SIZE);
        }


    }
}
