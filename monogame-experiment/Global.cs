using System;

using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop
{
    // Global scene
	public class Global : Scene
    {
		// Global asset pack
		private AssetPack assets;

		// Global key configuration
		private KeyConfig keyConf;

        // Transition
        private Transition trans;
            

		// Constructor
		public Global() { /* ... */ }


		// Initialize scene
        override public void Init()
        {
			// Get asset & key configuration paths
			String assetPath = conf.GetOtherParam("asset_path");
			String keyconfPath = conf.GetOtherParam("keyconfig_path");

			// Load global assets
			assets = new AssetPack(assetPath);

			// Parse key configuration & pass
            // it to input manager
			keyConf = new KeyConfig(keyconfPath);
			input.BindKeyConfig(keyConf);

            // Create transition
            trans = new Transition();
        }


        // Update scene
		override public void Update(float tm)
        {

            // Terminate
			if(input.GetKey(Keys.LeftControl) == State.Down
			   && input.GetKey(Keys.Q) == State.Pressed) 
			{
				eventMan.Terminate();
			}

            // Fullscreen
			if ( (input.GetKey(Keys.LeftAlt) == State.Down
			    && input.GetKey(Keys.Enter) == State.Pressed)
			    || input.GetKey(Keys.F4) == State.Pressed)
            {
				eventMan.ToggleFullscreen();
            }

            // Update transition
            trans.Update(tm);
        }


        // Draw scene
		override public void Draw(Graphics g)
        {
            // Draw transition
            trans.Draw(g);
        }


        //Destroy scene
		override public void Destroy()
        {

        }


        // Get name
		override public String getName()
        {
            return "global";
        }


        // Get the global asset pack
		public AssetPack GetAssets()
		{
			return assets;
		}


        // Get the global key configuration
		public KeyConfig GetKeyConfig()
		{
			return keyConf;
		}


        // Get transition manager
        public Transition GetTransition()
        {
            return trans;
        }
    }
}
