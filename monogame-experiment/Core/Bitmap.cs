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


		// Constructor
		public Bitmap(String path)
        {
			// Open the file
			FileStream fs = new FileStream(path, FileMode.Open);

            // Create a texture
			texture = Texture2D.FromStream(gdev, fs);

			// Dispose file stream
			fs.Dispose();
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
