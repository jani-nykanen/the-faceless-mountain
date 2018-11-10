using System;
using System.Collections.Generic;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop.Field
{
    // Game object manager
    public class ObjectManager
    {
        // Player
        private Player player;

        // Enemies
        private List<Enemy> enemies;


        // Constructor
        public ObjectManager(Camera cam, AssetPack assets, GameField rf = null)
        {

            // Create player
            // TODO: Get position from the maps or something
            player = new Player(new Vector2(6 * 64, -2 * 64 - 1), rf);

            // Set camera position
            cam.MoveTo(player.GetPos().X, player.GetPos().Y - 32);

            // Create list of enemies
            enemies = new List<Enemy>();
        }


        // Update
        public void Update(Stage stage, Camera cam, InputManager input, float tm)
        {
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
        }


        // Draw
        public void Draw(Graphics g, Camera cam = null)
        {
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
