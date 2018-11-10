using System;

using monogame_experiment.Desktop.Core;
using Microsoft.Xna.Framework;

namespace monogame_experiment.Desktop.Field
{
    // Game object manager
    public class ObjectManager
    {
        // Player
        private Player player;

        // Constructor
        public ObjectManager(Camera cam, AssetPack assets, GameField rf = null)
        {

            // Create player
            // TODO: Get position from the maps or something
            player = new Player(new Vector2(6 * 64, -2 * 64 - 1), rf);

            // Set camera position
            cam.MoveTo(player.GetPos().X, player.GetPos().Y - 32);
        }


        // Update
        public void Update(Stage stage, Camera cam, InputManager input, float tm)
        {
            // Update player
            player.Update(tm, input);
            // Set camera following
            player.SetCameraFollowing(cam, tm);

            // Player collisions
            stage.GetObjectCollision(player, tm, false);
            // Player tongue collisions
            stage.GetObjectCollision(player.GetTongue(), tm);
        }


        // Draw
        public void Draw(Graphics g, Camera cam = null)
        {
            player.Draw(g);
        }
    }
}
