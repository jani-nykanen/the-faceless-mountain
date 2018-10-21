using System;
using System.Collections.Generic;


namespace monogame_experiment.Desktop.Core
{
    // Scene manager
	public class SceneManager
    {
		// Scenes
		private List<Scene> scenes;
		// Global scene
		private Scene globalScene;
		// Current scene
		private Scene currentScene;


		// Constructor
        public SceneManager()
        {
			scenes = new List<Scene>();
			globalScene = null;
			currentScene = null;
        }


        // Add a scene
		public void AddScene(Scene scene, bool makeCurrent = false, 
		                     bool makeGlobal = false) 
		{

            // Add to the list
			scenes.Add(scene);

            // Make current
			if(makeCurrent || currentScene == null) 
			{

				currentScene = scene;
			}
            // Make global
			if(makeGlobal) 
			{

				globalScene = scene;
			}
		}


        // Initialize scenes
		public void Init() {

            // Initialize every scene
			foreach(var s in scenes) {

				s.Init();
			}
		}


        // Update
		public void Update(float tm) {

			// Update current scene first
			if (currentScene != null)
				currentScene.Update(tm);
            
			// Then update the global scene (if any)
			if (globalScene != null)
				globalScene.Update(tm);
		}


        // Draw
		public void Draw(Graphics g)
        {

            // Update current scene first
            if (currentScene != null)
                currentScene.Draw(g);

            // Then update the global scene (if any)
            if (globalScene != null)
                globalScene.Draw(g);
        }


        // Destroy
		public void Destroy() {

			foreach(var s in scenes) {

				s.Destroy();
			}
		}
    }
}
