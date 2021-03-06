﻿using System;
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

		// Weak event manager
		private WeakEventManager eventMan;
		// Input manager
		private InputManager input;
		// Configuration
		private Configuration conf;
        // Audio manager
        private AudioManager audio;


		// Constructor
        public SceneManager(InputManager input, WeakEventManager eventMan, 
                            Configuration conf, AudioManager audio)
        {
			scenes = new List<Scene>();
			globalScene = null;
			currentScene = null;

			// Store references to certain objects
			this.input = input;
			this.eventMan = eventMan;
			this.conf = conf;
            this.audio = audio;
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

			// Pass references to certain objects to the
			// scene
			scene.Ready(input, eventMan, conf, audio, this);
		}


        // Initialize scenes
		public void Init() {

			// Initialize global scene first
			globalScene.Init();

            // Initialize other scenes
			foreach(var s in scenes) {

				if (s != globalScene)
				{
					s.SetGlobalScene(globalScene);
					s.Init();
				}
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


        // Change scene
        public void ChangeScene(String name, Object data = null)
        {
            // Find corresponding scene
            foreach(var s in scenes)
            {
                if(s.GetName().Equals(name))
                {
                    s.OnChange(name, data);
                    currentScene = s;
                }
            }
        }


        // Destroy
		public void Destroy() {

			foreach(var s in scenes) {

				s.Destroy();
			}
		}
    }
}
