using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // Procedurally animated figure
	public class AnimatedFigure
    {
        // Global bitmaps
        static private Bitmap bmpBody;

        // Initialize global content
        static public void Init(AssetPack assets)
        {
            bmpBody = assets.GetBitmap("body");
        }


		// Animation mode for the figure
		public enum AnimationMode
		{
			Stand = 0,
			Run = 1,
			Jump = 2,
		};

        // Joint
		private struct Joint
		{
			public Vector2 pos;
			public float angle;

            // Constructors
			public Joint(Vector2 p, float a)
			{
				pos = p;
				angle = a;
			}
		};
      
		// Animation timer
		private float animTimer;
        // Is jumping
        private bool jumping;
              
		// Feet
		private Joint rightFoot;
		private Joint leftFoot;

		// Hands
		private Joint rightHand;
		private Joint leftHand;

		// Head angle
		private float headAngle;
        // Torso position
        private float torsoPos;

		// Size (=scale)
		private float size;


		// Get leg position (in [-1,1]×[0,-1] box) and angle
        private Joint GetFootJoint(float t)
		{
			const float pi_f = (float)Math.PI;

			const float WIDTH_MUL = 0.5f;
			const float HEIGHT_MUL = 0.33f;
			const float ANGLE_TARGET = pi_f / 5.0f;         

            Joint ret = new Joint(Vector2.Zero, 0.0f);

            // Make sure not negative or bigger than 2PI
            // NOTE: t shouldn't be negative in the first place
            if (t < 0.0f) t = 0.0f;
            t %= (pi_f * 2.0f);
                  
            // Calculate coordinates & angles
            if(t <= pi_f)
			{
				// Coordinates
				ret.pos.X = -(t / pi_f) * 2.0f + 1.0f;
				ret.pos.Y = 0.0f;

                // Angle
				ret.angle = (t - pi_f / 2.0f) / (pi_f / 2.0f)
                    * ANGLE_TARGET;
			}
			else 
			{
				// Coordinates
				ret.pos.X = (float)Math.Cos(t);
				ret.pos.Y = (float)Math.Sin(t);

                // Angles
				ret.angle = -((t - pi_f) - pi_f / 2.0f) / (pi_f / 2.0f)
                    * ANGLE_TARGET;
			}
			ret.pos.X *= WIDTH_MUL;
			ret.pos.Y *= HEIGHT_MUL;
                     
			return ret;
		}


        // Get hand param
		private Joint GetHandJoint(float t, bool isLeft = false)
		{
			const float pi_f = (float)Math.PI;
			const float DIST = 0.5f;
         
			Joint ret;

			// Calculate hand angle
			ret.angle = isLeft ? -(float)Math.Abs(pi_f - (t - pi_f))
					: (float)Math.Abs(pi_f - t);

			// Calculate position angle
			float a = t - pi_f;

			// Use angle to calculate positions
			ret.pos.X = (float)Math.Cos(a) * DIST;
			ret.pos.Y = (float)Math.Abs(Math.Sin(a)) * DIST;

			return ret;
		}


        // Get foot joint when jumping
        private Joint GetFootJointJumping(float speed)
        {
            const float pi_f = (float)Math.PI;
            const float FOOT_HEIGHT = -0.5f;
           
            Joint ret;

            float t = (speed + 1.0f) * pi_f / 2.0f + pi_f / 2.0f;

            ret.angle = -t;
            ret.pos.X = 0.0f;
            ret.pos.Y = FOOT_HEIGHT;

            return ret;
        }


        // Draw a foot
		private void DrawFoot(Graphics g, Joint foot, float xoff = 0.0f, bool black = false)
		{
			const float FOOT_WIDTH = 1.1f;
            const float FOOT_HEIGHT = 1.1f;
            
            // Compute size & pos
			Vector2 p = foot.pos * size;
            int w = (int)(FOOT_WIDTH * size);
            int h = (int)(FOOT_HEIGHT * size);

            // Draw foot
            g.Push();

			g.Translate(p.X + xoff*size, p.Y);
			g.Rotate(foot.angle);
            g.BeginDrawing();

            g.SetColor();
            g.DrawScaledBitmapRegion(bmpBody, 128 + (jumping ? 64 : 0), 
                                     128, 64, 64,
                                     -w / 2, -h, w, h);

            g.EndDrawing();
			g.Pop();
		}


        // Draw a hand
		private void DrawHand(Graphics g, Joint hand, float x = 0.0f, float y = 0.0f, bool left = true)
		{
			const float HAND_SIZE = 1.1f;

            // Position
			int dx = (int)((hand.pos.X + x) * size);
			int dy = (int)((hand.pos.Y + y) * size);
            
			// Size
			int w = (int)(HAND_SIZE * size);
			int h = w;

            // Draw hand
			g.Push();
            g.Translate(dx, dy);
            g.Rotate(hand.angle - (float)Math.PI/2.0f);
            g.BeginDrawing();

            g.SetColor();
            g.DrawScaledBitmapRegion(bmpBody, 128 + (left ? 64 : 0), 64, 64, 64,
                                     -w / 2, -h / 2, w, h);

            g.EndDrawing();
            g.Pop();
		}


        // Draw torso
		private void DrawTorso(Graphics g, float width, float height, float yoff)
		{
			g.BeginDrawing();

			int w = (int)(width * size * 2.0f);
			int h = (int)(height* size * 2.0f);
            int y = (int)( (yoff + torsoPos) * size);

            g.SetColor();
            g.DrawScaledBitmapRegion(bmpBody, 0, 64, 128, 128,
                                     -w / 2, -h, w, h);

            g.EndDrawing();
		}


		// Draw head
        private void DrawHead(Graphics g, float x, float y, float dim, float angle, bool black = false)
        {
            const float SCALE_FACTOR = 1.5f;

            int w = (int)(dim * size * SCALE_FACTOR);
			int h = (int)(dim * size * SCALE_FACTOR);
                        
			g.Push();
            g.Translate(x*size, (y+ torsoPos) * size);
            g.Rotate(-angle);
            g.BeginDrawing();

            g.SetColor();
            g.DrawScaledBitmapRegion(bmpBody, black ? 128 : 0, 0, 64, 64,
                                     -w / 2, -h / 2, w, h);

            g.EndDrawing();
            g.Pop();
        }


        // Update head angle
		private void UpdateHeadAngle(float headTarget, float tm)
        {
            const float HEAD_SPEED = 0.05f;

            if (headAngle < headTarget)
            {
                headAngle += HEAD_SPEED * tm;
                if (headAngle > headTarget)
                {
                    headAngle = headTarget;
                }
            }
            else if (headAngle > headTarget)
            {
                headAngle -= HEAD_SPEED * tm;
                if (headAngle < headTarget)
                {
                    headAngle = headTarget;
                }
            }
        }


        // Constructor
        public AnimatedFigure(float size)
        {
			animTimer = 0.0f;
			this.size = size;

			headAngle = 0.0f;
            torsoPos = 0.0f;
        }

        
        // Animate
        public void Animate(AnimationMode animMode, float animSpeed, float tm, int headDir = 0)
		{
			const float pi_f = (float)Math.PI;
            const float HEAD_TARGET = pi_f / 4.0f;

            // Speed limit when jumping
            const float SPEED_LIMIT = 0.75f;

            jumping = animMode == AnimationMode.Jump;

            // Should never change
            torsoPos = 0.0f;

			// Head target
			float headTarget = HEAD_TARGET * headDir;

            // Update animation timer
            animTimer += animSpeed * tm;
            if (animTimer >= (float)Math.PI * 2)
            {
                animTimer -= (float)Math.PI * 2;
            }

            // Animate different modes
            switch (animMode)
			{
                // Standing
				case AnimationMode.Stand:

					rightFoot = GetFootJoint(pi_f/2.0f);
					leftFoot = rightFoot;

					rightHand = GetHandJoint(pi_f / 2.0f);
					leftHand = rightHand;

					break;

				// Running animation
				case AnimationMode.Run:

                    // Animate feet
                    rightFoot = GetFootJoint(animTimer);
					leftFoot = GetFootJoint(animTimer + pi_f);

					// Animate hands
					rightHand = GetHandJoint(animTimer);
					leftHand = GetHandJoint(animTimer + pi_f, true);
               
					break;

                // Jumping
				case AnimationMode.Jump:
                
                    if (animSpeed > SPEED_LIMIT)
                        animSpeed = SPEED_LIMIT;

                    else if (animSpeed < -SPEED_LIMIT)
                        animSpeed = -SPEED_LIMIT;


                    // Set head angle
                    if (headDir == 0)
						headTarget = -HEAD_TARGET * (animSpeed < 0.0f ? -1 : 1);

                    // Set hand positions
                    rightHand = GetHandJoint((animSpeed+1.0f) * pi_f/2.0f);
                    leftHand = rightHand;

                    // Set foot positions
                    rightFoot = GetFootJointJumping(animSpeed);
                    leftFoot = rightFoot;

                    break;

                // Ignore the rest for now
				default:
					break;
			}

            // Update head angle
            //if ((animMode == AnimationMode.Stand ||
            //               animMode == AnimationMode.Run))
            //{
			UpdateHeadAngle(headTarget, tm);
            //}

		}


        // Reset animation timer (if negative)
        public void ResetTimer()
        {
            if(animTimer < 0.0f)
                animTimer = 0.0f;
        }


        // Draw
        public void Draw(Graphics g)
		{
			const float FEET_OFF = 0.25f;

			const float HAND_Y = -1.25f;

			const float TORSO_WIDTH = 1.0f;
			const float TORSO_HEIGHT = 1.0f;
			const float TORSO_YOFF = -0.75f;

			const float HEAD_X = 0.25f;
			const float HEAD_Y = -TORSO_HEIGHT - 0.5f + TORSO_YOFF / 2.0f;
			const float HEAD_DIM = 1.0f;

            // Draw black head background
            DrawHead(g, HEAD_X, HEAD_Y, HEAD_DIM, headAngle, true);

            // Draw left hand & foot
            DrawFoot(g, leftFoot, FEET_OFF);
			DrawHand(g, leftHand, FEET_OFF, HAND_Y);

            // Draw torso
            DrawTorso(g, TORSO_WIDTH, TORSO_HEIGHT, TORSO_YOFF);
			// Draw head
			DrawHead(g, HEAD_X, HEAD_Y, HEAD_DIM, headAngle);

            // Draw right hand & foot
			DrawFoot(g, rightFoot, -FEET_OFF);
			DrawHand(g, rightHand, -FEET_OFF, HAND_Y, false);
         
            // Reset color
			g.SetColor();
		}
    }
}
