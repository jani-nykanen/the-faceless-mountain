using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace monogame_experiment.Desktop.Core
{
	// Graphics manager
    public class Graphics
    {

		// Graphics device manager
		GraphicsDeviceManager gman;
		// Graphics device
		GraphicsDevice gdev;

		
		// Constructor
		public Graphics(GraphicsDevice gdev, GraphicsDeviceManager gman)
        {

			this.gdev = gdev;
			this.gman = gman;         
        }


        // Clear screen
		public void ClearScreen(byte r, byte g, byte b) 
		{

			gdev.Clear(new Color(r, g, b));
		}

    }
}
