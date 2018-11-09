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
		// Player object
		private Player player;
        // Game stage
        private Stage stage;
        // HUD
        private HUD hud;


        // Reset scene
        private void ResetGame()
        {
            const float INITIAL_CAM_SCALE = 1.75f;
            const float CAM_SCALE_TARGET = 1.5f;
            const float CAM_SCALE_SPEED = 0.0033f;
            const float TRANS_SPEED = 2.0f;

            // Create game objects
            player = new Player(new Vector2(6 * 64, -2 * 64 - 1), this);
            cam = new Camera();
            stage = new Stage(assets, 1);
            hud = new HUD(assets);

            // Set initial camera scale
            cam.Scale(INITIAL_CAM_SCALE, INITIAL_CAM_SCALE);
            cam.SetScaleTarget(CAM_SCALE_TARGET, CAM_SCALE_TARGET,
                               CAM_SCALE_SPEED* TRANS_SPEED, 
                               CAM_SCALE_SPEED* TRANS_SPEED);

            cam.MoveTo(player.GetPos().X, player.GetPos().Y - 32);

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

            // (Re)set game objects & stuff
            ResetGame();

        }


        // Update scene
		override public void Update(float tm)
        {

            // Skip certain things if transitioning
            if (!trans.IsActive())
            {
                // Update player
                player.Update(tm, input);
                // Set camera following
                player.SetCameraFollowing(cam, tm);


                // Player collisions
                stage.GetObjectCollision(player, tm, false);
                // Player tongue collisions
                stage.GetObjectCollision(player.GetTongue(), tm);

                // Update HUD (and time!)
                hud.Update(tm);
            }

            // Update stage
            stage.Update(tm);

            // Update camera
            cam.Update(tm);


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
            stage.DrawBackground(g);

            // Use camera
            cam.Use(g);

            // Draw stage
            stage.Draw(g, cam);

			// Draw player
			player.Draw(g);

            // Post-draw stage
            stage.PostDraw(g, cam);

            // Draw HUD elements etc
            g.IdentityWorld();
			g.Identity();

            // Draw HUD
            hud.Draw(g);
		}


        //Destroy scene
		override public void Destroy()
		{
			// ...
		}


        // Get name
		override public String getName()
		{
			return "game";
		}


        // "Set resetting"
        public void Reset()
        {
            const float TRANS_SPEED = 2.0f;
            const float CAM_TARGET = 2.0f;
            const float CAM_SPEED = 0.0050f * TRANS_SPEED;

            cam.SetScaleTarget(CAM_TARGET, CAM_TARGET, CAM_SPEED, CAM_SPEED);
            trans.Activate(Transition.Mode.In, TRANS_SPEED, ResetGame);
           
        }
    }
}
