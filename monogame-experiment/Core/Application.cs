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

		// Whether redraw the frame
		private bool redraw = false;
		// Time sum
		private float timeSum;

		// Scene manager
		private SceneManager sceneMan;


		// Configurate application
        private void Configurate()
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
        }

		
		// Constructor
		public Application(Configuration conf, GraphicsDeviceManager gman, Runner r)
        {
            // Store info         
			this.gman = gman;
			runner = r;

			// Configurate
			this.conf = conf;
			Configurate();

			// Create scene manager
			sceneMan = new SceneManager();
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
		}


        // Load content
		public void LoadAssets() 
		{
		
            // ...
		}


        // Update
		public void Update(float delta)
		{
			
			const int MAX_FRAME_UPDATE = 5;
            const int COMPARABLE_FRAME_RATE = 60;

            float frameWait = 1.0f / conf.frameRate;

            // Wait until enough time has passed, 
            // then update the frame
            timeSum += delta;

            // Update until all the frames in queue
            // are updated
            int updateCount = 0;
            while (timeSum >= frameWait)
            {

                // Update frame and set frame to be redrawable
				UpdateFrame((float)COMPARABLE_FRAME_RATE / (float)conf.frameRate);
                redraw = true;

                timeSum -= frameWait;

                if (++updateCount >= MAX_FRAME_UPDATE)
                    break;
            }
		}


        // Update frame
		public void UpdateFrame(float tm) {

			// Quit (TEMP)
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                runner.Exit();

			// Update scenes
			sceneMan.Update(tm);
		}


        // Draw
		public void Draw() 
		{
			if (!redraw) return;

			// Clear screen
			graphics.ClearScreen(170, 170, 170);

			// Draw scenes
			sceneMan.Draw(graphics);
		}


        // Destroy
		public void Destroy() 
		{

			// Destroy scenes
			sceneMan.Destroy();
		}
    }
}
