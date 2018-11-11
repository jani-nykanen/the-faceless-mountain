using System;

using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop.Field
{

    // A collision object
    public class CollisionObject
    {
        // Position
        protected Vector2 pos;
        // Previous position
        protected Vector2 oldPos;
        // Speed
        protected Vector2 speed;
        // Target speed
        protected Vector2 target;
        // Acceleration
        protected Vector2 acc;

        // Dimensions
        protected float width;
        protected float height;
        protected float centerY = 0.0f;

        // Total speed
        protected float totalSpeed;


        // On floor collision
        virtual protected void OnFloorCollision(float x, float y) { }

        // On ceiling collision
        virtual protected void OnCeilingCollision(float x, float y) { }

        // On wall collision
        virtual protected void OnWallCollision(float x, float y, int dir) { }

        // Any collision (for projectiles etc.)
        virtual protected void OnAnyCollision(float x, float y) {}


        // Update speed
        protected float UpdateSpeed(float speed, float acc, float target, float tm)
        {
            if (speed > target)
            {
                speed -= acc * tm;
                if (speed <= target)
                    speed = target;
            }
            else if (speed < target)
            {
                speed += acc * tm;
                if (speed >= target)
                    speed = target;
            }

            return speed;
        }


        // Move
        protected void Move(float tm)
        {
            // Store old position
            oldPos = pos;

            // Update speed
            speed.X = UpdateSpeed(speed.X, acc.X, target.X, tm);
            speed.Y = UpdateSpeed(speed.Y, acc.Y, target.Y, tm);

            // Calculate total speed
            totalSpeed = (float)Math.Sqrt(speed.X * speed.X + speed.Y * speed.Y);

            // Move
            pos.X += speed.X * tm;
            pos.Y += speed.Y * tm;
        }


        // Floor collision
        public void GetFloorCollision(float x, float y, float w, float tm)
        {
            const float DELTA = 4.0f;
            const float DELTA_H = -0.01f;

            // Check if horizontal overlay
            if (pos.X + width / 2 < x || pos.X - width / 2 > x + w)
                return;

            // Check speed
            if (speed.Y < DELTA_H * tm)
                return;

            // Check if the surface is between old & new value
            if (pos.Y > y - DELTA * tm && oldPos.Y < y + DELTA*2 * tm)
            {
                OnFloorCollision(pos.X, y);
                OnAnyCollision(pos.X, y);
            }
        }


        // Ceiling collision
        public void GetCeilingCollision(float x, float y, float w, float tm)
        {
            const float DELTA = 2.0f;

            // Check if horizontal overlay
            if (pos.X + width / 2 < x || pos.X - width / 2 > x + w)
                return;

            // Check speed
            if (speed.Y > 0.0f)
                return;

            // Check if the surface is between old & new value
            if (pos.Y - height + centerY < y + DELTA * tm && oldPos.Y - height + centerY > y - DELTA * tm)
            {
                OnCeilingCollision(pos.X, y + height + centerY);
                OnAnyCollision(pos.X, y + height + centerY);
            }
        }


        // Wall collision
        public void GetWallCollision(float x, float y, float h, int dir, float tm)
        {
            const float DELTA = 2.0f;
            const float DELTA_H = 1.0f;

            // Other directions not allowed
            if (!(dir == 1 || dir == -1))
                return;

            // Check if vertical overlay
            if (pos.Y < y + DELTA_H * tm 
                || pos.Y - height + centerY > y + h - DELTA_H * tm)
                return;

            // Check if the surface is between old & new value
            if (
                (speed.X > 0.0f && dir == 1 && pos.X + width / 2 > x - DELTA * tm
                 && oldPos.X + width / 2 < x + DELTA * tm)
                ||
                (speed.X < 0.0f && dir == -1 && pos.X - width / 2 < x + DELTA * tm
                 && oldPos.X - width / 2 > x - DELTA * tm)
                )
            {
                OnWallCollision(x - width / 2 * dir, pos.Y, dir);
                OnAnyCollision(x - width / 2 * dir, pos.Y);
            }
        }


        // Hurt collision (not defined by default)
        virtual public void GetHurtCollision(float x, float y, float w, float h) { }


        // Get coordinates
        public Vector2 GetPos()
        {
            return pos;
        }
    

        // Get speed
        public Vector2 GetSpeed()
        {
            return speed;
        }


        // Get width
        public float GetWidth()
        {
            return width;
        }


        // Get height
        public float GetHeight()
        {
            return height;
        }
    }
}
