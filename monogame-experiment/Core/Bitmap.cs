using System;
using System.IO;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace monogame_experiment.Desktop.Core
{
	// A bitmap class
    public class Bitmap
    {
		// Graphics device
		static private GraphicsDevice gdev;


        // Initialize global content
		public static void Init(GraphicsDevice gdev) 
		{
			Bitmap.gdev = gdev;
		}


		// Texture
		private Texture2D texture;


		// Constructor (with path)
		public Bitmap(String path)
        {
			// Open the file
			FileStream fs = new FileStream(path, FileMode.Open);

            // Create a texture
			texture = Texture2D.FromStream(gdev, fs);

			// Dispose file stream
			fs.Dispose();
        }


		// Constructor (with data)
        public Bitmap(byte[] data, int w, int h) 
		{

            // Check if data length is good
			if (data.Length % 4 != 0)
				throw new Exception("Data length must be divisible by 4!");

			//initialize a texture
            texture = new Texture2D(gdev, w, h);

			//the array holds the color for each pixel in the texture
			Color[] pdata = new Color[w * h];
			int arrIndex = 0;
			for (int i = 0; i < pdata.Length; ++ i)
            {
				pdata[i] = new Color(
					data[arrIndex], data[arrIndex + 1], 
					data[arrIndex + 2], data[arrIndex + 3]);
				
				arrIndex += 4;
            }

            //set the color
            texture.SetData(pdata);
		}

        
        // Get a texture
        public Texture2D GetTexture() 
		{
			return texture;
		}


        // Get width
        public int GetWidth()
		{
			return texture.Width;
		}

        
        // Get height
        public int GetHeight()
		{
			return texture.Height;
		}
    }
}
