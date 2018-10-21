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


        // Make ready for use
		public void Ready(InputManager input, WeakEventManager eventMan) {

			this.input = input;
			this.eventMan = eventMan;
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
		public abstract String getName();

    }
}
