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

		// Animation mode
		private AnimationMode animMode;

		// Animation timer
		private float animTimer;

		// Feet positions
		private Vector2 rightFoot;
		private Vector2 leftFoot;

		// Feet angles
		private float rightAngle;
		private float leftAngle;

		// Size (=scale)
		private float size;


		// Get leg position (in [-1,1]×[0,-1] box)
        private Vector2 GetFootPosition(float t)
		{
			const float pi_f = (float)Math.PI;
                     
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

			Vector2 ret = new Vector2();         
            // Calculate coordinates
            if(t <= pi_f)
			{
				ret.X = -(t / pi_f) * 2.0f + 1.0f;
				ret.Y = 0.0f;
			}
			else 
			{
				ret.X = (float)Math.Cos(t);
				ret.Y = (float)Math.Sin(t);
			}
                     
			return ret;
		}


        // Get foot angle
        private float GetFootAngle(float t)
		{
			const float MAX_ANGLE = (float)Math.PI / 3.0f;
			const float pi_f = (float)Math.PI;

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

			// Calculate angle
			float angle = 0.0f;
            if (t <= pi_f)
            {
				angle = 0.0f;
            }
            else
            {
				angle = (1.0f - (float)Math.Abs((t - pi_f) - pi_f / 2.0f) / (pi_f / 2.0f) ) 
					* MAX_ANGLE;
            }

			return angle;
		}


        // Draw a foot
		private void DrawFoot(Graphics g, Vector2 foot, float angle = 0.0f)
		{
			const float FOOT_WIDTH = 0.75f;
            const float FOOT_HEIGHT = 0.50f;

			Vector2 p = foot * size;
         
            // Draw right foot
            int w = (int)(FOOT_WIDTH * size);
            int h = (int)(FOOT_HEIGHT * size);

			g.Push();
			g.Translate(p.X, p.Y);
			g.Rotate(angle);
            g.BeginDrawing();
            
            g.FillRect(-w / 2, -h, w, h);
            g.SetColor();

            g.EndDrawing();
			g.Pop();
		}


		// Constructor
        public AnimatedFigure(float size)
        {
			// Create required vectors
			leftFoot = new Vector2();
			rightFoot = new Vector2();
            
			animMode = AnimationMode.Run;
			animTimer = 0.0f;
			this.size = size;
        }


        // Animate
        public void Animate(float animSpeed, float tm)
		{
			// Update animation timer
			animTimer += animSpeed * tm;
            if(animTimer >= (float)Math.PI*2)
			{
				animTimer -= (float)Math.PI * 2;
			}

            // Animate different modes
			switch(animMode)
			{
				// Running animation
				case AnimationMode.Run:

					// Animate feet
					rightFoot = GetFootPosition(animTimer);
					leftFoot = GetFootPosition(animTimer + (float)Math.PI);
					rightAngle = GetFootAngle(animTimer);
					leftAngle = GetFootAngle(animTimer + (float)Math.PI);

					break;

                // Ignore the rest for now
				default:
					break;
			}
		}


        // Draw
        public void Draw(Graphics g)
		{
			// Draw feet
			g.SetColor(1, 0, 0);
			DrawFoot(g, leftFoot, leftAngle);

			g.SetColor(0, 1, 0);
			DrawFoot(g, rightFoot, rightAngle);

		}
    }
}
