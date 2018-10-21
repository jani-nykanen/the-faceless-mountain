using System;

using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop
{
    // Global scene
	public class Global : Scene
    {
		// Constructor
		public Global() { /* ... */ }


		// Initialize scene
        override public void Init()
        {

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
        }


        // Draw scene
		override public void Draw(Graphics g)
        {

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
    }
}
