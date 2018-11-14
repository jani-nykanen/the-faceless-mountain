using System;


namespace monogame_experiment.Desktop.Core
{
	// Scene interface
	public abstract class Scene
    {
		// Input manager
		protected InputManager input;
		// Weak event manager
		protected WeakEventManager eventMan;
		// Configuration data
		protected Configuration conf;

		// A refrence to a global scene
		protected Scene globalScene;


        // Make ready for use
		public void Ready(InputManager input, WeakEventManager eventMan, Configuration conf) {

			this.input = input;
			this.eventMan = eventMan;
			this.conf = conf;
		}


        // Set global scene
        public void SetGlobalScene(Scene s)
		{
			globalScene = s;         
		}


        // Quit
        public void Quit()
        {
            eventMan.Terminate();
        }


        // Toggle fullscreen
        public void ToggleFullscreen()
        {
            eventMan.ToggleFullscreen();
        }


        // Initialize scene
		public abstract void Init();
		// Update scene
		public abstract void Update(float tm);
		// Draw scene
		public abstract void Draw(Graphics g);
		//Destroy scene
		public abstract void Destroy();

		// Get name
		public abstract String GetName();

    }
}
