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

		// Camera
		private Camera cam;
		// Player object
		private Player player;
        // Game stage
        private Stage stage;
        // HUD
        private HUD hud;

		// Constructor
		public GameField() { /* ... */ }


		// Initialize scene
		override public void Init()
        {
			Global gs = (Global)globalScene;
			AssetPack assets = gs.GetAssets();

			// Load assets
			bmpFont = assets.GetBitmap("font");
         
			// Get key configuration
			keyConf = gs.GetKeyConfig();

            // Initialize global content for objects
            AnimatedFigure.Init(assets);
            Tongue.Init(assets);

			// Create game objects
            player = new Player(new Vector2(6*64,-2*64-1));
			cam = new Camera();
            stage = new Stage(assets, 1);
            hud = new HUD(assets);

            // Set initial camera scale
            cam.Scale(1.5f, 1.5f);
            cam.MoveTo(player.GetPos().X, player.GetPos().Y - 32);

        }


        // Update scene
		override public void Update(float tm)
        {
			// Update player
			player.Update(tm, input);
            // Set camera following
            player.SetCameraFollowing(cam, tm);

            // Update stage
            stage.Update(tm);
            // Player collisions
            stage.GetObjectCollision(player, tm, false);
            // Player tongue collisions
            stage.GetObjectCollision(player.GetTongue(), tm);

            // Update camera
            cam.Update(tm);

            // Update HUD (and time!)
            hud.Update(tm);
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

    }
}
