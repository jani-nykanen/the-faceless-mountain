using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
	// Player object
    public class Player : GameObject
    {
		const float SCALE = 32.0f;
        const float WIDTH = 24.0f;
        const float HEIGHT = 64.0f;

        const float TARGET_X = 2.5f;
        const float ACC_X = 0.100f;
        const float ACC_Y = 0.50f;

        const float GRAVITY = 7.0f;
        const float JUMP_HEIGHT = 6.0f;
        const float JUMP_TIME_MAX = 20.0f;

		// Animated figure ("skeleton")
		private AnimatedFigure skeleton;

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

        // Player's tongue
        private Tongue tongue;


        // Get tongue movement
        private void GetTongueMovement(float tm)
        {
            const float SPEED_X = 0.375f;
            const float SPEED_Y = SPEED_X + (ACC_Y - ACC_X);

            if (!tongue.IsStuck()) return;

            Vector2 p = tongue.GetPos();
            float angle = (float)Math.Atan2(p.Y - pos.Y, p.X - pos.X);

            speed.X += (float)Math.Cos(angle) * SPEED_X * tm;
            speed.Y += (float)Math.Sin(angle) * SPEED_Y * tm;
        }


        // Control
		private void Control(InputManager input, float tm)
		{
			const float DELTA = 0.01f;

            const float RUN_PLUS = 2.0f;
            const float TONGUE_SPEED = 24.0f;
            const float JUMP_SPEED_BONUS = 6.0f;

            float jumpHeight = JUMP_HEIGHT + (float)Math.Abs(speed.X) / JUMP_SPEED_BONUS;

			// Get gamepad stick
			Vector2 stick = input.GetStick();
			// Make sure too small movements are ignored
			if ((float)Math.Abs(stick.X) < DELTA)
				stick.X = 0.0f;

            // Movement direction (we cannot use
            // direction since it must be either -1
            // or 1)
            moveDir = 0;

            // Is running
            running = input.GetButton("fire2") == State.Down;

            // Get direction
			if (stick.X < -DELTA)
			{
				moveDir = -1;
				direction = -1;
			}
			else if(stick.X > DELTA)
			{
				moveDir = 1;
				direction = 1;
			}

			// Set speed target
			// target.X = moveDir * TARGET_X;
			target.X = stick.X * TARGET_X;
			target.Y = GRAVITY;

            // Apply running multiplier
            if(running)
            {
                target.X *= RUN_PLUS;
            }

			// Jump
			State fire1 = input.GetButton("fire1");
			if(canJump && fire1 == State.Pressed)
			{
				speed.Y = -jumpHeight;
			}
            else if(!canJump && jumpTimer > 0.0f && fire1 == State.Down)
			{
                speed.Y = -jumpHeight;
                jumpTimer -= 1.0f * tm;
            }
            else if(fire1 == State.Up)
            {
                jumpTimer = 0.0f;
            }

            headDir = 0;

            // Looking up or down
			if (stick.Y < -DELTA)
                headDir = 1;
			else if (stick.Y > DELTA)
                headDir = -1;

            // Create tongue
            if(!tongue.DoesExist() && input.GetButton("fire3") == State.Pressed)
            {
                Vector2 speed = new Vector2(0, 0);

                if(headDir == 0)
                {
                    speed.X = direction * TONGUE_SPEED;
                }
                else
                {
                    speed.Y = -headDir * TONGUE_SPEED;
                }

                tongue.Create(pos - new Vector2(0, SCALE), speed);
            }
        }


        // Animate
        private void Animate(float tm)
		{
			const float ANIM_SPEED_STEP = 0.060f;
			const float DELTA = 0.5f;

			float animSpeed = 0.0f;

			AnimatedFigure.AnimationMode mode = AnimatedFigure.AnimationMode.Stand;

			// Jumping
			if (!canJump)
			{
				mode = AnimatedFigure.AnimationMode.Jump;
				animSpeed = speed.Y / JUMP_HEIGHT;
				if (animSpeed < -1.0f) animSpeed = -1.0f;
				if (animSpeed > 1.0f) animSpeed = 1.0f;
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
            tongue = new Tongue();

			this.pos = pos;
            this.oldPos = pos;

            this.width = WIDTH;
            this.height = HEIGHT;

            acc.X = ACC_X;
            acc.Y = ACC_Y;
		}
		public Player() : this(Vector2.Zero) { }


        // Update player
        override public void Update(float tm, InputManager input = null)
		{

            // Update player stuff
			Control(input, tm);
			Move(tm);
			Animate(tm);

            // Update tongue
            tongue.Update(tm, input);
            tongue.UpdateStartPos(pos - new Vector2(0, SCALE));
            GetTongueMovement(tm);

            canJump = false;
        }


        // Floor collision
        override protected void OnFloorCollision(float x, float y)
        {

             pos.Y = y;
             speed.Y = 0.0f;
             canJump = true;
             jumpTimer = JUMP_TIME_MAX;

             skeleton.ResetTimer();
        }


        // Ceiling collision
        override protected void OnCeilingCollision(float x, float y)
        {

             pos.Y = y;
             speed.Y = 0.0f;
             jumpTimer = 0.0f;
        }


        // Wall collision
        override protected void OnWallCollision(float x, float y, int dir)
        {
            const float BOUNCE_DELTA = 2.0f;
            const float BOUNCE_FACTOR = 1.25f;

            if((float)Math.Abs(speed.X) < BOUNCE_DELTA)
            {
                speed.X = 0.0f;
            }
            else
            {
                speed.X /= -BOUNCE_FACTOR;
            }
            pos.X = x;
        }


        // Get tongue
        public Tongue GetTongue()
        {
            return tongue;
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
            const float MIN_SCALE = 1.25f;
            const float SPEED_SCALE_FACTOR = 7.0f;
            const float SPEED_SCALE = BASE_SCALE - MIN_SCALE;
            const float SCALE_SPEED = 0.0015f;

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
            if (scale < MIN_SCALE) scale = MIN_SCALE;

            if (cam.GetScale().X > scale || moveDir == 0)
                cam.SetScaleTarget(scale, scale, SCALE_SPEED, SCALE_SPEED);

            cam.Translate(tx, ty);
        }


        // Draw player
        override public void Draw(Graphics g)
		{
			g.Push();

            g.Identity();
			g.Translate(pos.X, pos.Y);         
			g.Scale(direction, 1.0f);
         
			// Draw figure
			skeleton.Draw(g);

			g.Pop();

            // Draw tongue
            tongue.Draw(g);
		}

    }
}
