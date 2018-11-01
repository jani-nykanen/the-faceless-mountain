using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
	// Player object
    public class Player
    {
		const float SCALE = 48.0f;
        const float WIDTH = 40.0f;
        const float HEIGHT = 80.0f;

        const float TARGET_X = 3.0f;
        const float ACC_X = 0.25f;
        const float ACC_Y = 0.5f;

        const float GRAVITY = 6.0f;
        const float JUMP_HEIGHT = 8.0f;
        const float JUMP_TIME_MAX = 25.0f;

		// Animated figure ("skeleton")
		private AnimatedFigure skeleton;

		// Position
		private Vector2 pos;
        // Previous position
        private Vector2 oldPos;
		// Speed
		private Vector2 speed;
		// Target speed
		private Vector2 target;
        // Total speed
        private float totalSpeed;

        // Dimensions
        private float width;
        private float height;

		// Direction (animation)
		private int direction =1;
		// Direction (movement)
		private int moveDir = 0;

		// Can jump
		private bool canJump;
        // Jump timer
        private float jumpTimer;
        // Is running
        private bool running;

        // Head direction
        private int headDir;


        // Update speed
		private float UpdateSpeed(float speed, float acc, float target, float tm)
		{
			if(speed > target)
			{
				speed -= acc * tm;
				if (speed <= target)
					speed = target;
			}
			else if(speed < target)
			{
				speed += acc * tm;
				if (speed >= target)
					speed = target;
			}

			return speed;
		}


        // Control
		private void Control(InputManager input, float tm)
		{
            const float RUN_PLUS = 2.0f;

            // Movement direction (we cannot use
            // direction since it must be either -1
			// or 1)
			moveDir = 0;

            // Is running
            running = input.GetKey("fire2") == State.Down;

            // Get direction
            if (input.GetKey("left") == State.Down)
			{
				moveDir = -1;
				direction = -1;
			}
			else if(input.GetKey("right") == State.Down)
			{
				moveDir = 1;
				direction = 1;
			}

			// Set speed target
			target.X = moveDir * TARGET_X;
			target.Y = GRAVITY;

            // Apply running multiplier
            if(running)
            {
                target.X *= RUN_PLUS;
            }

			// Jump
			State fire1 = input.GetKey("fire1");
			if(canJump && fire1 == State.Pressed)
			{
				speed.Y = -JUMP_HEIGHT;
			}
            else if(jumpTimer > 0.0f && fire1 == State.Down)
			{
                speed.Y = -JUMP_HEIGHT;
                jumpTimer -= 1.0f * tm;
            }
            else if(fire1 == State.Up)
            {
                jumpTimer = 0.0f;
            }

            headDir = 0;
            // Looking up or down
            if (input.GetKey("up") == State.Down)
                headDir = 1;
            else if (input.GetKey("down") == State.Down)
                headDir = -1;

        }


        // Move
        private void Move(float tm)
		{
            // Store old position
            oldPos = pos;

            // Update speed
            speed.X = UpdateSpeed(speed.X, ACC_X, target.X, tm);
			speed.Y = UpdateSpeed(speed.Y, ACC_Y, target.Y, tm);

            // Calculate total speed
            totalSpeed = (float)Math.Sqrt(speed.X * speed.X + speed.Y * speed.Y);

			// Move
			pos.X += speed.X * tm;
			pos.Y += speed.Y * tm;
		}


        // Animate
        private void Animate(float tm)
		{
			const float ANIM_SPEED_STEP = 0.045f;
			const float DELTA = 0.5f;

			float animSpeed = 0.0f;

			AnimatedFigure.AnimationMode mode = AnimatedFigure.AnimationMode.Stand;

			// Jumping
			if (!canJump)
			{
				mode = AnimatedFigure.AnimationMode.Jump;
				animSpeed = speed.Y / JUMP_HEIGHT;
			}
			else
			{

				// Walking
				if (moveDir != 0 || (float)Math.Abs(speed.X) > DELTA)
				{
					animSpeed = (float)Math.Abs(speed.X) * ANIM_SPEED_STEP;
					mode = AnimatedFigure.AnimationMode.Run;
				}
				// Standing
				else
				{
					animSpeed = 0;
				}

			}

			// Animate skeleton
            skeleton.Animate(mode, animSpeed, tm, headDir);
		}


        // Constructor
        public Player(Vector2 pos)
        {
			skeleton = new AnimatedFigure(SCALE);

			this.pos = pos;
            this.oldPos = pos;

            this.width = WIDTH;
            this.height = HEIGHT;
		}
		public Player() : this(Vector2.Zero) { }


        // Update player
        public void Update(InputManager input, float tm)
		{

			Control(input, tm);
			Move(tm);
			Animate(tm);

			canJump = false;
        }


        // Floor collision
        public void GetFloorCollision(float x, float y, float w, float tm)
        {
            const float DELTA = 2.0f;
            const float DELTA_H = -0.01f;

            // Check if horizontal overlay
            if (pos.X + width / 2 < x || pos.X - width / 2 > x + w)
                return;

            // Check speed
            if (speed.Y < DELTA_H*tm)
                return;

            // Check if the surface is between old & new value
            if(pos.Y > y- DELTA*tm && oldPos.Y < y+ DELTA*tm)
            {
                pos.Y = y;
                speed.Y = 0.0f;
                canJump = true;
                jumpTimer = JUMP_TIME_MAX;

                skeleton.ResetTimer();
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
            if (pos.Y-height < y + DELTA*tm && oldPos.Y-height > y - DELTA*tm)
            {
                pos.Y = y + height;
                speed.Y = 0.0f;
                jumpTimer = 0.0f;
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
            if (pos.Y < y+ DELTA_H*tm || pos.Y - height > y + h - DELTA_H * tm)
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
                speed.X = 0.0f;
                pos.X = x - width / 2 * dir;
            }
        }


        // Set camera following
        public void SetCameraFollowing(Camera cam, float tm)
        {
            const float DELTA = 1.0f;

            const float CAM_SPEED_X = 32.0f;
            const float CAM_SPEED_Y = 12.0f;

            const float CAM_H_JUMP = 192.0f;
            const float CAM_V_JUMP = 128.0f;
            const float Y_CENTER_MUL = 1.5f;

            const float BASE_SCALE = 1.5f;
            const float SPEED_SCALE_FACTOR = 6.0f;
            const float SPEED_SCALE = 0.25f;
            const float SCALE_SPEED = 0.005f;

            float dx = pos.X;
            float dy = pos.Y - SCALE*Y_CENTER_MUL;

            // If moving somewhere
            if(moveDir != 0)
            {
                dx += CAM_H_JUMP * direction;
            }
            // If watching up/down
            if(headDir != 0)
            {
                dy -= CAM_V_JUMP * headDir;
            }

            // Get camera pos
            float cx = cam.GetPos().X;
            float cy = cam.GetPos().Y;

            // Get angle & distance
            float angle = (float)Math.Atan2(dy - cy, dx - cx);
            float dist = (float)Math.Sqrt( (dx - cx)*(dx - cx) + (dy - cy)*(dy - cy) );

            // Move camera
            float tx = 0.0f, ty = 0.0f;
            if(dist > DELTA)
            {
                tx = (float)Math.Cos(angle) * (dist / CAM_SPEED_X) * tm;
                ty = (float)Math.Sin(angle) * (dist / (CAM_SPEED_Y)) * tm;
            }

            // Set speed scale
            float scale = BASE_SCALE - totalSpeed / SPEED_SCALE_FACTOR * SPEED_SCALE;

            if(cam.GetScale().X > scale || moveDir == 0)
                cam.SetScaleTarget(scale, scale, SCALE_SPEED, SCALE_SPEED);

            cam.Translate(tx, ty);
        }


        // Draw player
        public void Draw(Graphics g)
		{
			g.Push();

            g.Identity();
			g.Translate(pos.X, pos.Y);         
			g.Scale(direction, 1.0f);
         
			// Draw figure
			skeleton.Draw(g);

			g.Pop();
		}


        // Get coordinates
        public Vector2 GetPos()
        {
            return pos;
        }
    }
}
