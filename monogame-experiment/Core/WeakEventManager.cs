using System;


namespace monogame_experiment.Desktop.Core
{
    // Weak event manager. Pass some event signals
    // to the base application class, like terminating
    // the program or toggling the fullscreen mode
	public class WeakEventManager
    {
        // Reference to the application base class
		private Application appBase;

		// Constructor
		public WeakEventManager(Application a)
		{
   			appBase = a;
		}


        // Terminate
        public void Terminate()
		{
			appBase.Terminate();
		}


        // Toggle fullscreen
        public void ToggleFullscreen()
		{
			appBase.ToggleFullscreen();
		}
    }
}
