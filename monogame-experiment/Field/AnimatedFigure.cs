using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // Procedurally animated figure
	public class AnimatedFigure
    {
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

			Joint ret = new Joint();
                     
  			// Make sure not negative or bigger than 2PI
			// NOTE: we do this only once since we assume
            // the value is mostly bounded already
            if (t < 0.0f)
            {
                t += pi_f * 2.0f;
            }
			if (t >= pi_f * 2.0f)
            {
                t -= pi_f * 2.0f;
            }
                  
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
		private void DrawFoot(Graphics g, Joint foot, float xoff = 0.0f)
		{
			const float FOOT_WIDTH = 0.75f;
            const float FOOT_HEIGHT = 0.50f;
            
            // Compute size & pos
			Vector2 p = foot.pos * size;
            int w = (int)(FOOT_WIDTH * size);
            int h = (int)(FOOT_HEIGHT * size);

            // Draw foot
			g.Push();
			g.Translate(p.X + xoff*size, p.Y);
			g.Rotate(foot.angle);
            g.BeginDrawing();
            
            g.FillRect(-w / 2, -h, w, h);

            g.EndDrawing();
			g.Pop();
		}


        // Draw a hand
		private void DrawHand(Graphics g, Joint hand, float x = 0.0f, float y = 0.0f)
		{
			const float HAND_SIZE = 0.50f;

            // Position
			int dx = (int)((hand.pos.X + x) * size);
			int dy = (int)((hand.pos.Y + y) * size);
            
			// Size
			int w = (int)(HAND_SIZE * size);
			int h = w;

            // Draw hand
			g.Push();
            g.Translate(dx, dy);
            g.Rotate(hand.angle);
            g.BeginDrawing();

            g.FillRect(-w / 2, -h / 2, w, h);

            g.EndDrawing();
            g.Pop();
		}


        // Draw torso
		private void DrawTorso(Graphics g, float width, float height, float yoff)
		{
			g.BeginDrawing();

			int w = (int)(width * size);
			int h = (int)(height* size);
            int y = (int)( (yoff + torsoPos) * size);

            g.FillRect(-w / 2, -h + y, w, h);
            g.EndDrawing();
		}


		// Draw head
        private void DrawHead(Graphics g, float x, float y, float dim, float angle)
        {
            int w = (int)(dim * size);
			int h = (int)(dim * size);
                        
			g.Push();
            g.Translate(x*size, (y+ torsoPos) * size);
            g.Rotate(-angle);
            g.BeginDrawing();

            g.FillRect(-w / 2, -h / 2, w, h);

			// TEMP eyes
			g.SetColor(0, 0, 0);
			g.FillRect(0, -12, 6, 6);
			g.FillRect(16, -12, 6, 6);

            g.EndDrawing();
            g.Pop();
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
            const float HEAD_SPEED = 0.05f;

			// Update animation timer
			animTimer += animSpeed * tm;
            if(animTimer >= (float)Math.PI*2)
			{
				animTimer -= (float)Math.PI * 2;
			}

            torsoPos = 0.0f;

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
					
					// Set head angle
					headAngle = -animSpeed * HEAD_TARGET;

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
            if((animMode == AnimationMode.Stand ||
                           animMode == AnimationMode.Run))
            {
                float headTarget = headDir * HEAD_TARGET;
                if (headAngle < headTarget)
                {
                    headAngle += HEAD_SPEED * tm;
                    if(headAngle > HEAD_TARGET)
                    {
                        headAngle = HEAD_TARGET;
                    }
                }
                else if(headAngle > headTarget)
                {
                    headAngle -= HEAD_SPEED * tm;
                    if (headAngle < headTarget)
                    {
                        headAngle = headTarget;
                    }
                }
            }

		}


        // Draw
        public void Draw(Graphics g)
		{
			const float FEET_OFF = 0.25f;

			const float HAND_Y = -1.25f;

			const float TORSO_WIDTH = 1.0f;
			const float TORSO_HEIGHT = 1.5f;
			const float TORSO_YOFF = -0.5f;

			const float HEAD_X = 0.25f;
			const float HEAD_Y = -TORSO_HEIGHT + TORSO_YOFF / 2.0f;
			const float HEAD_DIM = 1.0f;
            
			// Draw left foot
			g.SetColor(0, 0.625f, 0);
			DrawFoot(g, leftFoot, FEET_OFF);
			// Draw left hand
            g.SetColor(0.625f, 0, 0);
			DrawHand(g, leftHand, FEET_OFF, HAND_Y);

			// Draw torso
			g.SetColor(0.0f, 0.25f, 0.75f);
			DrawTorso(g, TORSO_WIDTH, TORSO_HEIGHT, TORSO_YOFF);

			// Draw head
			g.SetColor(1.0f, 0.75f, 0.0f);
			DrawHead(g, HEAD_X, HEAD_Y, HEAD_DIM, headAngle);

            // Draw right foot
			g.SetColor(0, 1.0f, 0);
			DrawFoot(g, rightFoot, -FEET_OFF);
			// Draw right hand
			g.SetColor(1.0f, 0.0f, 0);
			DrawHand(g, rightHand, -FEET_OFF, HAND_Y);
         
            // Reset color
			g.SetColor();
		}
    }
}
