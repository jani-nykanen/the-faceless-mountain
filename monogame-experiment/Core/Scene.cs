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
        // Audio manager
        protected AudioManager audio;
        // Scene manager reference
        protected SceneManager sceneMan;

        // A refrence to a global scene
        protected Scene globalScene;


        // Make ready for use
        public void Ready(InputManager input, WeakEventManager eventMan, 
                          Configuration conf, AudioManager audio,
                         SceneManager sceneMan) 
        {

			this.input = input;
			this.eventMan = eventMan;
			this.conf = conf;
            this.audio = audio;
            this.sceneMan = sceneMan;
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


        // Toggle audio
        public void ToggleAudio()
        {
            audio.ToggleAudio();
        }


        // Is audio renabled
        public bool IsAudioEnabled()
        {
            return audio.IsEnabled();
        }

        // Initialize scene
		public abstract void Init();
		// Update scene
		public abstract void Update(float tm);
		// Draw scene
		public abstract void Draw(Graphics g);
		//Destroy scene
		public abstract void Destroy();
        // Changed to other scene
        public virtual void OnChange(String target, Object data=null) {}

		// Get name
		public abstract String GetName();
    }
}
