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
		const float SCALE = 64.0f;
		const float JUMP_HEIGHT = 10.0f;
        const float JUMP_TIME_MAX = 15.0f;

		// Animated figure ("skeleton")
		private AnimatedFigure skeleton;

		// Position
		private Vector2 pos;
		// Speed
		private Vector2 speed;
		// Target speed
		private Vector2 target;

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
			const float TARGET_X = 4.0f;
            const float RUN_PLUS = 2.0f;
			const float GRAVITY = 6.0f;

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
			const float ACC_X = 0.5f;
			const float ACC_Y = 0.5f;

			// Update speed
			speed.X = UpdateSpeed(speed.X, ACC_X, target.X, tm);
			speed.Y = UpdateSpeed(speed.Y, ACC_Y, target.Y, tm);

			// Move
			pos.X += speed.X * tm;
			pos.Y += speed.Y * tm;
		}


        // Animate
        private void Animate(float tm)
		{
			const float ANIM_SPEED_STEP = 0.025f;
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
		}
		public Player() : this(Vector2.Zero) { }


        // Update player
        public void Update(InputManager input, float tm)
		{
			const float FLOOR_Y = 144.0f;

			Control(input, tm);
			Move(tm);
			Animate(tm);

			canJump = false;

            // Temp floor collision
			if(speed.Y > 0.0f && pos.Y > FLOOR_Y)
			{
				pos.Y = FLOOR_Y;
				speed.Y = 0.0f;
				canJump = true;
                jumpTimer = JUMP_TIME_MAX;

            }
		}


        // Set camera following
        // TEMPORARY!
        public void SetCameraFollowing(Camera cam, float tm)
        {
            const float DELTA = 1.0f;
            const float CAM_SPEED = 32.0f;

            float dx = pos.X;
            float dy = pos.Y - SCALE;

            // Get camera pos
            float cx = cam.GetPos().X;
            float cy = cam.GetPos().Y;

            // Get angle & distance
            float angle = (float)Math.Atan2(dy - cy, dx - cx);
            float dist = (float)Math.Sqrt( (dx - cx)*(dx - cx) + (dy - cy)*(dy - cy) );

            // Move camera
            float tx = 0.0f, ty = 0.0f;
            float viewRatio = cam.GetViewport().X / cam.GetViewport().Y;
            if(dist > DELTA)
            {
                tx = (float)Math.Cos(angle) * (dist / CAM_SPEED) * tm;
                ty = (float)Math.Sin(angle) * (dist / (CAM_SPEED / viewRatio)) * tm;
            }

            cam.Translate(tx, ty);
        }


        // Draw player
        public void Draw(Graphics g)
		{
			g.Push();
                     
			g.Translate(pos.X, pos.Y);         
			g.Scale(direction, 1.0f);
         
			// Draw figure
			skeleton.Draw(g);

          
			g.Pop();
		}
    }
}
