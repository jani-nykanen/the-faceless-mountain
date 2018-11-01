using System;

using Microsoft.Xna.Framework;

using monogame_experiment.Desktop.Core;


namespace monogame_experiment.Desktop.Field
{
    // Stage
    public class Stage
    {
        // Tile size
        const int TILE_SIZE = 64;

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
        public void GetPlayerCollision(Player pl, float tm)
        {
            const int CHECK = 5;
            const float WIDTH_PLUS = 8.0f;

            Vector2 p = pl.GetPos();

            // Compute starting positions
            int sx = (int)(p.X / TILE_SIZE) - CHECK/2;
            int sy = (int)(p.Y / TILE_SIZE) - CHECK/2;

            int ex = sx + CHECK;
            int ey = sy + CHECK;

            // Check solid tiles in this area
            int tile = 0;
            for (int y = sy; y <= ey; ++y)
            {
                for (int x = sx; x <= ex; ++x)
                {
                    // Get tile
                    tile = map.GetTile(0, x, y);
                    if (tile == -1)
                        continue;

                    // TODO: Check for nearby tiles, not
                    // all the collisions are necessary
                    if(tile == 1)
                    {
                        if(map.GetTile(0, x, y-1) != 1)
                            pl.GetFloorCollision(x * TILE_SIZE - WIDTH_PLUS, 
                                                 y * TILE_SIZE, 
                                                 TILE_SIZE + WIDTH_PLUS*2, tm);

                        if (map.GetTile(0, x, y + 1) != 1)
                            pl.GetCeilingCollision(x * TILE_SIZE, (y + 1) * TILE_SIZE, TILE_SIZE, tm);

                        if (map.GetTile(0, x-1, y) != 1)
                            pl.GetWallCollision(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, 1, tm);

                        if (map.GetTile(0, x+1, y) != 1)
                            pl.GetWallCollision( (x+1) * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, -1, tm);
                    }
                }
            }

        }


        // Draw
        public void Draw(Graphics g, Camera cam)
        {
            g.BeginDrawing();

            // Get top left corner & viewport
            Vector2 topLeft = cam.GetTopLeftCorner();
            Vector2 bottomRight = cam.GetBottomRightCorner();
            Vector2 view = cam.GetViewport();

            // Compute starting positions
            int sx = (int)(topLeft.X / TILE_SIZE) - 1;
            int sy = (int)(topLeft.Y / TILE_SIZE) - 1;

            int ex = (int)(bottomRight.X / TILE_SIZE) + 1;
            int ey = (int)(bottomRight.Y / TILE_SIZE) + 1;


            // Draw tiles
            int tile = 0;
            for (int y = sy; y <= ey; ++ y)
            {
                for (int x = sx; x <= ex; ++ x)
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
