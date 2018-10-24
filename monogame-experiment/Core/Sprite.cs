using System;
namespace monogame_experiment.Desktop.Core
{
	// Sprite class
    public class Sprite
    {
		// Dimensions
		private int w;
		private int h;

		// Current frame
		private int frame;
        // Current row
		private int row;

        // Frame count
        float count;


        // Constructor
        public Sprite(int width, int height)
        {
			w = width;
			h = height;

			frame = 0;
			row = 0;
			count = 0.0f;
        }


        // Animate
        public void Animate(int row, int start, int end, float speed, float tm)
		{
			// If starting & ending frame are the same
			if (start == end)
            {

                count = 0;
                frame = start;
                this.row = row;
                return;
            }

            // Set to the current frame and start
            // animation from the beginning
            if (this.row != row)
            {

                count = 0;
                frame = end > start ? start : end;
                this.row = row;
            }

            if (start < end && frame < start)
            {            
                frame = start;
            }
            else if (end < start && frame < end)
            {

                frame = end;
            }

            // Animate
            count += 1.0f * tm;
            if (count > speed)
            {

                if (start < end)
                {

                    if (++frame > end)
                    {
                        frame = start;
                    }
                }
                else
                {

                    if (--frame < end)
                    {

                        frame = start;
                    }
                }

                count -= speed;
            }
		}


		// Draw a scaled frame
		public void DrawFrame(Graphics g, Bitmap bmp, int frame, int row, 
		                      int dx, int dy, int dw, int dh, 
		                      Graphics.Flip flip = Graphics.Flip.None)
		{
			g.DrawScaledBitmapRegion(bmp, w * frame, h * row, w, h, dx, dy,  dw, dh, flip);
		}


		// Draw a frame
        public void DrawFrame(Graphics g, Bitmap bmp, int frame, int row,
		                      int dx, int dy, Graphics.Flip flip)
        {
			DrawFrame(g, bmp, frame, row, dx, dy, w, h, flip);
        }


        // Draw a scaled sprite
		public void Draw(Graphics g, Bitmap bmp, int dx, int dy, int dw, int dh, 
		                 Graphics.Flip flip = Graphics.Flip.None)
        {
			DrawFrame(g, bmp, frame, row, dx, dy, dw, dh, flip);
        }


		// Draw a sprite
		public void Draw(Graphics g, Bitmap bmp, int dx, int dy, 
		                 Graphics.Flip flip = Graphics.Flip.None)
        {
            DrawFrame(g, bmp, frame, row, dx, dy, flip);
        }
    }
}
