using System;

using Microsoft.Xna.Framework;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // Stage
    public class Stage
    {
        // Tile size
        const int TILE_SIZE = 96;

        // Tilemap
        private Tilemap map;


        // Map
        public Stage(Tilemap map)
        {
            this.map = map;
        }


        // Update
        public void Update(float tm)
        {
            // ...
        }


        // Player collision
        public void GetPlayerCollision(Player pl)
        {
            // ...
        }


        // Draw
        public void Draw(Graphics g, Camera cam)
        {
            g.BeginDrawing();

            // Get top left corner & viewport
            Vector2 corner = cam.GetTopLeftCorner();
            Vector2 view = cam.GetViewport();

            // Compute starting positions
            int sx = (int)(corner.X / TILE_SIZE) - 1;
            int sy = (int)(corner.Y / TILE_SIZE) - 1;

            // Compute dimensions
            int w = (int)(view.X / TILE_SIZE) + 2;
            int h = (int)(view.Y / TILE_SIZE) + 2;

            // Draw tiles
            int tile = 0;
            for (int y = sy; y < sy + h; ++ y)
            {
                for (int x = sx; x < sx + w; ++ x)
                {
                    // Get tile
                    tile = map.GetTile(0, x, y);
                    if (tile == -1)
                        continue;

                    // Draw a black tile if solid
                    if(tile == 1)
                    {
                        g.SetColor(0, 0, 0);
                        g.FillRect(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                        g.SetColor();
                    }
                }
            }

            g.EndDrawing();
        }
    }
}
