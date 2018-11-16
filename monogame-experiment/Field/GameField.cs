using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
	// Game field scene
	public class GameField : Scene
    {
      
		// Bitmaps
		private Bitmap bmpFont;
      
		// Key configuration
		private KeyConfig keyConf;
        // Transitions
        private Transition trans;
        // Assets
        private AssetPack assets;

		// Camera
		private Camera cam;
        // Object manager
        private ObjectManager objMan;
        // Game stage
        private Stage stage;
        // HUD
        private HUD hud;
        // Pause screen
        private Pause pause;

        // Reset scene
        private void ResetGame()
        {
            const float INITIAL_CAM_SCALE = 1.75f;
            const float CAM_SCALE_TARGET = 1.5f;
            const float CAM_SCALE_SPEED = 0.0033f;
            const float TRANS_SPEED = 2.0f;

            // Create game objects
            cam = new Camera();
            stage = new Stage(assets);
            hud = new HUD(assets);

            // Create object manager
            objMan = new ObjectManager(assets);
            // Add objects
            stage.ParseObjects(objMan);
            // Set player
            objMan.SetPlayer(cam, stage, this);

            // Set initial camera scale
            cam.Scale(INITIAL_CAM_SCALE, INITIAL_CAM_SCALE);
            cam.SetScaleTarget(CAM_SCALE_TARGET, CAM_SCALE_TARGET,
                               CAM_SCALE_SPEED* TRANS_SPEED, 
                               CAM_SCALE_SPEED* TRANS_SPEED);

            // Disable pause
            pause.Disable();

            // Set transition
            trans.Activate(Transition.Mode.Out, TRANS_SPEED, null);
        }


		// Constructor
		public GameField() { /* ... */ }


		// Initialize scene
		override public void Init()
        {

            Global gs = (Global)globalScene;
			assets = gs.GetAssets();

			// Load assets
			bmpFont = assets.GetBitmap("font");
         
			// Get key configuration
			keyConf = gs.GetKeyConfig();
            // Get transition
            trans = gs.GetTransition();

            // Initialize global content for objects
            AnimatedFigure.Init(assets);
            Tongue.Init(assets);
            Enemy.Init(assets);

            // Play music
            Sample sMusic = assets.GetSample("theme");
            audio.PlaySample(sMusic, 0.65f, true);

            // Create pause
            pause = new Pause(this);

            // (Re)set game objects & stuff
            ResetGame();

        }


        // Update scene
		override public void Update(float tm)
        {
            // TODO: Unnecessary pause.IsActive calls?

            // Skip certain things if transitioning
            if (!trans.IsActive())
            {
                // If paused, skip the rest
                if (pause.IsActive())
                {
                    pause.Update(input);
                    return;
                }
                // Make active
                else 
                {
                    if(input.GetButton("start") == State.Pressed
                       || input.GetButton("cancel") == State.Pressed)
                    {
                        pause.Activate();
                        return;
                    }
                }

                // Update game objects
                objMan.Update(stage, cam, input, tm);

                // Update HUD (and time!)
                hud.Update(tm);
            }
            else if(!pause.IsActive())
            {
                objMan.TransitionEvents(trans.GetValue(), tm);
            }

            // In the case transitioning & pause active
            if (!pause.IsActive())
            {
                // Update stage
                stage.Update(tm);

                // Update camera
                cam.Update(tm);
            }


        }
      

        // Draw scene
		override public void Draw(Graphics g)
		{
			g.ToggleAutoBeginEnd(false);

			// Clear screen
            g.ClearScreen(8, 48, 96);

			// Set matrices
			g.Identity();
			g.FitViewHeight(720.0f);

            // Draw background
            stage.DrawBackground(g, cam);

            // Use camera
            cam.Use(g, pause.IsActive());

            // Draw stage
            stage.Draw(g, cam);

            // Draw game objects
            objMan.Draw(g, cam);

            // Post-draw stage
            stage.PostDraw(g, cam);

            // Draw HUD elements etc
            g.IdentityWorld();
			g.Identity();

            // Draw HUD
            hud.Draw(g);

            // Draw pause
            pause.Draw(g);
		}


        //Destroy scene
		override public void Destroy()
		{
			// ...
		}


        // Get name
		override public String GetName()
		{
			return "game";
		}


        // "Set resetting"
        public void Reset()
        {
            const float TRANS_SPEED = 2.0f;
            const float CAM_TARGET = 2.0f;
            const float CAM_SPEED = 0.0050f * TRANS_SPEED;

            if(!pause.IsActive())
                cam.SetScaleTarget(CAM_TARGET, CAM_TARGET, CAM_SPEED, CAM_SPEED);

            trans.Activate(Transition.Mode.In, TRANS_SPEED, ResetGame);
           
        }


        // Start quitting
        public void StartQuitting()
        {
            const float TRANS_SPEED = 2.0f;

            trans.Activate(Transition.Mode.In, TRANS_SPEED, Quit);
        }
    }
}
