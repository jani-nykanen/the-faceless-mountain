using System;

using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
	// Game field scene
	public class GameField : Scene
    {

        // TEMP
		private float val = 0.0f;


		// Constructor
		public GameField() { /* ... */ }


		// Initialize scene
		override public void Init()
        {
            
        }


        // Update scene
		override public void Update(float tm)
        {
			val += 5.0f * tm;
			if (val > 255.0f*2)
				val -= 255.0f*2;
        }
      

        // Draw scene
		override public void Draw(Graphics g)
		{
			byte b = val <= 255.0f ? (byte)(val) : (byte)(255.0f*2-val);
			g.ClearScreen(b, 170, 170);
		}


        //Destroy scene
		override public void Destroy()
		{
			
		}


        // Get name
		override public String getName()
		{
			return "game";
		}

    }
}
