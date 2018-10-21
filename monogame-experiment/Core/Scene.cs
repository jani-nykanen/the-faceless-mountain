using System;


namespace monogame_experiment.Desktop.Core
{
	// Scene interface
    public interface Scene
    {
           
        // Initialize scene
		void Init();
		// Update scene
		void Update(float tm);
		// Draw scene
		void Draw(Graphics g);
		//Destroy scene
		void Destroy();

		// Get name
		String getName();

    }
}
