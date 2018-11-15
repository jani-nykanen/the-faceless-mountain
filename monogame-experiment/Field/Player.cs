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
        const float HURT_MAX = 60.0f;

		const float SCALE = 32.0f;
        const float WIDTH = 24.0f;
        const float HEIGHT = 64.0f;

        const float TARGET_X = 3.0f;
        const float ACC_X = 0.100f;
        const float ACC_Y = 0.50f;

        const float GRAVITY = 7.0f;
        const float JUMP_HEIGHT = 6.0f;
        const float JUMP_TIME_MAX = 20.0f;

        const float MOUTH_X = 8.0f;
        const float MOUTH_Y = -60.0f;

        // Starting position
        private Vector2 startPos;

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

        // Hurt timer
        private float hurtTimer = 0.0f;
        // Is dead
        private bool dead;
        // Angle
        private float angle;
        // Special scale
        private float spcScale;

        // A reference to the game object
        private GameField gameRef;


        // Get tongue movement
        private void GetTongueMovement(float tm)
        {
            const float SPEED_X = 0.40f;
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

            // Set gravity here
            target.Y = GRAVITY;

            // No controlling while hurt
            if (hurtTimer > 0.0f) return;

            // Compute jump height
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

                tongue.Create(pos + new Vector2(MOUTH_X*direction, MOUTH_Y), speed);
                tongue.SetTip(SCALE, -skeleton.GetHeadAngle() * direction, direction != 1);
            }
        }


        // Animate
        private void Animate(float tm)
		{
			const float ANIM_SPEED_STEP = 0.05f;
			const float DELTA = 0.5f;

			float animSpeed = 0.0f;

			AnimatedFigure.AnimationMode mode = AnimatedFigure.AnimationMode.Stand;

            // If hurt
            if (hurtTimer > 0.0f)
            {
                mode = AnimatedFigure.AnimationMode.Hurt;
            }
            else
            {

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

            }

			// Animate skeleton
            skeleton.Animate(mode, animSpeed, tm, headDir);
		}


        // Kill
        private void Kill()
        {
            dead = true;

            // Reset game
            if (gameRef != null)
                gameRef.Reset();
        }


        // Constructor
        public Player(Vector2 pos, GameField rf = null)
        {
			skeleton = new AnimatedFigure(SCALE);
            tongue = new Tongue();

			this.pos = pos;
            this.oldPos = pos;

            this.width = WIDTH;
            this.height = HEIGHT;

            acc.X = ACC_X;
            acc.Y = ACC_Y;

            dead = false;

            gameRef = rf;

            // Store starting position
            startPos = pos;

            // Set space animation
            skeleton.Animate(AnimatedFigure.AnimationMode.Stand, 0.0f, 1.0f, 0);

        }
		public Player() : this(Vector2.Zero) { }


        // Update player
        override public void Update(float tm, InputManager input = null)
		{
            // Set transition values to default
            spcScale = 1.0f;
            angle = 0.0f;

            // Update player stuff
			Control(input, tm);
			Move(tm);
			Animate(tm);

            // Update tongue
            tongue.Update(tm, input);
            tongue.UpdateStartPos(pos + new Vector2(MOUTH_X*direction, MOUTH_Y));
            GetTongueMovement(tm);

            // Update timers
            if (hurtTimer > 0.0f)
                hurtTimer -= 1.0f * tm;

            // Check if outside the game area
            if(pos.Y-SCALE*3.0f > 0.0f)
            {
                Kill();
            }

            canJump = false;
        }


        // Floor collision
        override protected void OnFloorCollision(float x, float y)
        {

            pos.Y = y;

            // If hurt, just bounce
            if (hurtTimer > 0.0f)
                speed.Y /= -1;
            else
            {
                speed.Y = 0.0f;
                canJump = true;
                jumpTimer = JUMP_TIME_MAX;

                skeleton.ResetTimer();
            }

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
            const float BOUNCE_DELTA = 3.0f;
            const float BOUNCE_FACTOR = 1.25f;

            float delta = hurtTimer > 0.0f ? 0.0f : BOUNCE_DELTA;
            float bounce = hurtTimer > 0.0f ? 1.0f : BOUNCE_FACTOR;

            if((float)Math.Abs(speed.X) < delta)
            {
                speed.X = 0.0f;
            }
            else
            {
                speed.X /= -bounce;
            }
            pos.X = x;
        }


        // Hurt collision
        override public void GetHurtCollision(float x, float y, float w, float h)
        {
            const float KNOCKBACK_SPEED_X = 8.0f;
            const float KNOCKBACK_SPEED_Y = 8.0f;
            const float BASE_JUMP = 5.0f;
            const float BASE_H = 2.0f;

            if (hurtTimer <= 0.0f &&
               pos.X + width/2 > x && pos.X - width/2 < x + w &&
               pos.Y > y && pos.Y-height < y+h)
            {
                // Hurt & make the tongue return
                hurtTimer = HURT_MAX;
                tongue.Kill(true);

                // Knockback
                float mx = x + w / 2;
                float my = y + h / 2;

                float dx = (pos.X - mx) / w;
                float dy = (pos.Y-height/2 - my) / h;

                speed.X = dx * KNOCKBACK_SPEED_X;
                speed.Y = dy * KNOCKBACK_SPEED_Y;

                // Make sure the player has at least some horizontal & vertical speed
                if (speed.X > 0.0f && speed.X < BASE_H)
                    speed.Y = BASE_H;
                else if (speed.X < 0.0f && speed.X > -BASE_H)
                    speed.X = -BASE_H;

                if (speed.Y > 0.0f && speed.Y < BASE_JUMP)
                    speed.Y = BASE_JUMP;
                else if (speed.Y < 0.0f && speed.Y > -BASE_JUMP)
                    speed.Y = -BASE_JUMP;

                target.X = speed.X;
                target.Y = speed.Y;

            }
        }


        // Get tongue
        public Tongue GetTongue()
        {
            return tongue;
        }


        // Set camera following
        public void SetCameraFollowing(Camera cam, float tm)
        {
            if (dead) return;

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
            const float HURT_FACTOR = 0.5f;
            const float MIDDLE = 32.0f;

			g.Push();

            g.Identity();
            // Translate to the point
			g.Translate(pos.X, pos.Y);
            // Rotate
            g.Translate(0.0f, -MIDDLE);
            g.Rotate(angle);
            g.Translate(0.0f, MIDDLE);
            // Scale
            g.Scale(direction, 1.0f);
            g.Scale(spcScale, spcScale);

            // Draw figure
            if (hurtTimer <= 0.0f)
                g.SetColor();
            else
            {
                float v = 1.0f - (0.5f + (float)Math.Sin(hurtTimer* HURT_FACTOR));
                g.SetColor(1, v, v);
            }

            skeleton.Draw(g, tongue.DoesExist());

			g.Pop();

            // Draw tongue
            tongue.Draw(g);
		}


        // Update transition events
        public void TransitionEvents(float t)
        {
            angle = (float)(Math.PI * 2) * (1.0f-t);
            spcScale = 1.0f-t;
        }

    }
}
