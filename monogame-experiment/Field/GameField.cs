﻿using System;

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

			// Create game objects
			player = new Player();
			cam = new Camera();
            stage = new Stage(assets.GetTilemap("test"));

            // Set initial camera scale
            cam.Scale(1.5f, 1.5f);

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
        }
      

        // Draw scene
		override public void Draw(Graphics g)
		{
			g.ToggleAutoBeginEnd(false);

			// Clear screen
            g.ClearScreen(170, 170, 170);

			// Set matrices
			g.Identity();
			g.FitViewHeight(720.0f);
         
			// Use camera
			cam.Use(g);

            // Draw stage
            stage.Draw(g, cam);

			// Draw player
			player.Draw(g);

			// Draw HUD elements etc
			g.IdentityWorld();
			g.Identity();

			g.BeginDrawing();

			// Draw some text
            g.DrawText(bmpFont, "Testing Area", 16, 16, -16, 0, 0.75f, false);

			g.EndDrawing();
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
