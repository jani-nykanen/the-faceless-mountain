using System;
namespace monogame_experiment.Desktop.Core
{
	// Simple configuration structure
	public class Configuration
    {

		// Initial window size
		public int winWidth = 800;
		public int winHeight = 600;

		// Is fullscreen
		public bool fullscreen = false;

		// Window caption
		public String caption = "Game";

		// Framerate
		public int frameRate = 60;      
    }
}
