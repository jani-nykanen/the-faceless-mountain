using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace monogame_experiment.Desktop.Core
{
	// Application core
    public class Application
    {

		// Graphics manager
		private GraphicsDeviceManager gman;
		// Graphics object
		private Graphics graphics;

		// Runner
		private Runner runner;

		// Configuration
        private Configuration conf;
        
		// Time sum
		private float timeSum = 0.0f;

		// Scene manager
		private SceneManager sceneMan;
		// Input manager
		private InputManager input;
		// Weak event manager
		private WeakEventManager eventMan;


		// Configurate application
        private void Configure()
        {
			// Change window size
            gman.PreferredBackBufferWidth = conf.winWidth;
            gman.PreferredBackBufferHeight = conf.winHeight;

			// Set fullscreen
			gman.HardwareModeSwitch = false;
			gman.IsFullScreen = conf.fullscreen;

            // Apply changes
            gman.ApplyChanges();

			// Set window title
            runner.Window.Title = conf.caption;
			// Enable resizing (not a user-option)
			runner.Window.AllowUserResizing = true;
        }

		
		// Constructor
		public Application(Configuration conf, GraphicsDeviceManager gman, Runner r)
        {
            // Store info         
			this.gman = gman;
			runner = r;

			// Configurate
			this.conf = conf;
			Configure();

			// Create input manager
			input = new InputManager();
			// Create event manager
			eventMan = new WeakEventManager(this);
			// Create scene manager
			sceneMan = new SceneManager(input, eventMan, conf);
        }


        // Add a scene
		public void AddScene(Scene scene, bool makeCurrent = false, bool makeGlobal = false) {

			sceneMan.AddScene(scene, makeCurrent, makeGlobal);
		}

        
        // Initialize scenes
		public void InitScenes() {

			sceneMan.Init();
		}


        // Initialize graphics
		public void InitGraphics(GraphicsDevice gdev) 
		{         
			// Create graphics object
            graphics = new Graphics(gdev, gman);

			// Pass reference to the input manager
			input.PassGraphics(graphics);
		}


        // Load content
		public void LoadAssets() 
		{
		
            // ...
		}


        // Update
		public void Update(float delta)
		{         
            /**
             * Reasoning behind this:
             * Consider we had physics simulation
             * with non-fixed framerate. Causes chaos.
             * But we still render the game every frame,
             * otherwise we could get flickering.
             */

			const int MAX_FRAME_UPDATE = 5;
            const int COMPARABLE_FRAME_RATE = 60;

            float frameWait = 1.0f / conf.frameRate;

            // Wait until enough time has passed, 
            // then update the frame
            timeSum += delta / 1000.0f;

            // Update until all the frames in queue
            // are updated
            int updateCount = 0;
            while (timeSum >= frameWait)
            {            
                // Update frame and set frame to be redrawable
				UpdateFrame((float)COMPARABLE_FRAME_RATE / (float)conf.frameRate);

                timeSum -= frameWait;

                if (++updateCount >= MAX_FRAME_UPDATE)
                    break;
            }
		}


        // Update frame
		public void UpdateFrame(float tm) {
                 
			// Update input (pre)
			input.Update();

			// Update scenes
			sceneMan.Update(tm);

			// Update input (post)
			input.PostUpdate();
		}


        // Draw
		public void Draw() 
		{
			// Draw scenes
			sceneMan.Draw(graphics);
		}


        // Destroy
		public void Destroy() 
		{

			// Destroy scenes
			sceneMan.Destroy();
		}


        // Terminate
		public void Terminate() 
		{

			runner.Exit();
		}


        // Toggle fullscreen
        public void ToggleFullscreen() 
		{
			gman.ToggleFullScreen();
		}
    }
}
