using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace monogame_experiment.Desktop.Core
{
	// Graphics manager
	public class Graphics : Transformation
    {
		// Flipping flag
		public enum Flip
		{
			None = 0,
			Horizontal = 1,
			Vertical = 2,
			Both = 3
		};
        

		// Graphics device manager
		private GraphicsDeviceManager gman;
      
		// Base sprite batch
		private SpriteBatch baseBatch;
		// Base sprite effects
		private SpriteEffects baseEffects;

		// Global color
		private Vector4 globalColor = new Vector4(1, 1, 1, 1);

		// White texture/bitmap
		private Bitmap bmpWhite;

		// Automatic begin/end
		private bool autoBeginEnd = true;

		// Rasterizer state
		private RasterizerState rstate;
      

        // Creates a white texture
        public void CreateWhiteTexture() 
		{
			bmpWhite = new Bitmap(new byte[] { 255, 255, 255, 255 }, 1, 1);
		}
        
		
		// Constructor
		public Graphics(GraphicsDevice gdev, GraphicsDeviceManager gman) : base(gdev)
        {         
            // Store stuff
			this.gdev = gdev;
			this.gman = gman;

			// Initialize graphics-related objects
			Bitmap.Init(gdev);

			// Create sprite batches
			baseBatch = new SpriteBatch(gdev);
			// ... and effects
			baseEffects = new SpriteEffects();

			// Create a white texture
			CreateWhiteTexture();

			rstate = new RasterizerState();
			rstate.CullMode = CullMode.None;
        }


        // Clear screen
		public void ClearScreen(byte r, byte g, byte b) 
		{

			gdev.Clear(new Color(r, g, b));
		}


        // Set global color
		public void SetColor(float r = 1.0f, float g = 1.0f, float b = 1.0f, 
		                     float a = 1.0f) 
		{

			globalColor = new Vector4(r, g, b, a);
		}


		// Draw a bitmap
        public void DrawBitmap(Bitmap bmp, int dx, int dy, Flip flip = Flip.None)
        {

            DrawBitmapRegion(bmp, 0, 0, bmp.GetWidth(), bmp.GetHeight(), dx, dy, flip);
        }


		// Draw a scaled bitmap
        public void DrawScaledBitmap(Bitmap bmp, int dx, int dy, int dw, int dh, Flip flip = Flip.None)
        {

			DrawScaledBitmapRegion(bmp, 0, 0, bmp.GetWidth(), bmp.GetHeight(), dx, dy, dw, dh, flip);
        }


        // Draw a bitmap region
        public void DrawBitmapRegion(Bitmap bmp, int sx, int sy, int sw, int sh,
		                             int dx, int dy, Flip flip = Flip.None)
		{

			DrawScaledBitmapRegion(bmp, sx, sy, sw, sh, dx, dy, sw, sh, flip);
		}


        // Draw a scaled bitmap region
        public void DrawScaledBitmapRegion(Bitmap bmp, int sx, int sy, int sw, int sh,
		                                   float dx, float dy, float dw, float dh, 
		                                   Flip flip = Flip.None) 
		{
			baseEffects = SpriteEffects.None;

            // Check flipping
			if (((int)flip & (int)Flip.Horizontal) != 0)
            {            
				baseEffects = baseEffects | SpriteEffects.FlipHorizontally;
            }
			if (((int)flip & (int)Flip.Vertical) != 0)
            {
                        
				baseEffects = baseEffects | SpriteEffects.FlipVertically;
            }

			if(autoBeginEnd)
				baseBatch.Begin(SpriteSortMode.Deferred);

            // Draw
			baseBatch.Draw(bmp.GetTexture(), 
			               new Rectangle((int)dx, (int)dy, (int)dw, (int)dh),
			               new Rectangle(sx, sy, sw, sh), 
			               (new Color(globalColor.X, globalColor.Y, globalColor.Z)) 
			               * globalColor.W, 0.0f, new Vector2(),
			               baseEffects, 0);

			if(autoBeginEnd)
			    baseBatch.End();
		}
        

        // Draw text
        public void DrawText(Bitmap bmp, String text, int dx, int dy, int xoff, int yoff, 
		                     float scale = 1.0f, bool center = false)
		{   
			int cw = (bmp.GetWidth()) / 16;
            int ch = cw;
			int len = text.Length;

			char[] chr = text.ToCharArray();

            float x = (float)dx;
            float y = (float)dy;
            char c;

            int sx, sy;

            // Disable auto begin/end if enabled
			bool oldState = autoBeginEnd;
			if(oldState)
			{
				ToggleAutoBeginEnd(false);
				baseBatch.Begin(SpriteSortMode.Deferred);
			}

            // Center the text
            if (center)
			{
                dx -= (int) ((len + 1) / 2.0f * (cw + xoff) * scale);
                x = dx;
            }

			// Draw every character
			for (int i = 0; i < len; ++i)
			{

				c = chr[i];
				if (c == '\n')
				{

					x = dx;
					y += (yoff + ch) * scale;
					continue;
				}

				sx = c % 16;
				sy = (c / 16);

				DrawScaledBitmapRegion(bmp, sx * cw, sy * ch, cw, ch,
					(int)x, (int)y,
					(int)(cw * scale), (int)(ch * scale),
					Flip.None);

				x += (cw + xoff) * scale;
			}         

            // End & re-enable
			if(oldState)
			{
				baseBatch.End();
				ToggleAutoBeginEnd(true);
			}
		}


        // Draw a filled rectangle
        public void FillRect(int x, int y, int w, int h) 
		{
			if (autoBeginEnd)
                baseBatch.Begin(SpriteSortMode.Deferred);

            // Draw
            baseBatch.Draw(bmpWhite.GetTexture(),
                           new Rectangle((int)x, (int)y, (int)w, (int)h),
                           new Rectangle(0, 0, 1, 1),
                           (new Color(globalColor.X, globalColor.Y, globalColor.Z))
                           * globalColor.W);

            if (autoBeginEnd)
                baseBatch.End();
		}


        // Enable/disable automatic begin/end control
        public void ToggleAutoBeginEnd(bool state = true) 
		{
			autoBeginEnd = state;
		}


        // Begin drawing
        public void BeginDrawing()
		{         
			baseBatch.Begin(SpriteSortMode.Deferred,null,null,null,rstate,null, GetResultMatrix());
		}


        // End drawing
        public void EndDrawing()
		{

			baseBatch.End();
		}
    }
}
