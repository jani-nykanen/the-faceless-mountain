﻿using System;
using System.Collections.Generic;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop.Field
{
    // Game object manager
    public class ObjectManager
    {
        // Spawn sound
        private Sample sSpawn;

        // Player
        private Player player;
        // Spiral
        private Spiral spiral;
        // Star
        private Star star;

        // Enemies
        private List<Enemy> enemies;


        // Constructor
        public ObjectManager(AssetPack assets)
        {
            // Create spiral
            spiral = new Spiral(assets);

            // Create list of enemies
            enemies = new List<Enemy>();

            // Get assets
            sSpawn = assets.GetSample("spawn");
        }


        // Set player
        public void SetPlayer(Camera cam, Stage stage, GameField rf, AudioManager audio, AssetPack assets)
        {
            Vector2 plPos = stage.GetStartPos();

            // Create player
            player = new Player(plPos, rf, audio);
            // Set spiral position
            spiral.Create(plPos + new Vector2(0, -Stage.TILE_SIZE / 2 * 1.5f));

            // Set camera position
            cam.MoveTo(player.GetPos().X, player.GetPos().Y - Stage.TILE_SIZE / 2);

            // Play spawn sound
            audio.PlaySample(sSpawn, 0.90f);
        }


        // Create star
        public void CreateStar(Stage stage)
        {
            star = new Star(stage.GetStarPos());
        }


        // Update
        public void Update(Stage stage, Camera cam, InputManager input, float tm)
        {
            // Update spiral
            spiral.Update(tm, true);
            // Update star
            star.Update(tm);

            // Update player
            player.Update(tm, input);
            // Set camera following
            player.SetCameraFollowing(cam, tm);

            // Player collisions
            stage.GetObjectCollision(player, tm, true);
            // Player tongue collisions
            stage.GetObjectCollision(player.GetTongue(), tm);

            // Update enemies
            foreach(Enemy e in enemies)
            {
                e.Update(tm);
                e.CheckCamera(cam);

                // Collisions
                e.GetPlayerCollision(player, tm);
                stage.GetObjectCollision(e, tm, false);
            }

            // TEMPORARY!
            // TODO: DEBUG, remove
            if(input.GetButton("debug") == State.Pressed)
            {
                stage.ToNextCheckpoint(player);
            }
        }


        // Update transition events
        public void TransitionEvents(float t, float tm)
        {
            spiral.Update(tm);
            player.TransitionEvents(t);
        }


        // Draw
        public void Draw(Graphics g, Camera cam = null)
        {
            // Draw spiral
            spiral.Draw(g);

            // Draw star
            star.Draw(g, cam);

            // Draw enemies
            foreach(Enemy e in enemies)
            {
                e.Draw(g);
            }

            // Draw player
            player.Draw(g);
        }


        // Add an enemy
        public void AddEnemy(Enemy e)
        {
            enemies.Add(e);
        }
    }
}
