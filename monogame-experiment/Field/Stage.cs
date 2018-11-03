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

        // Bitmaps
        private Bitmap bmpTileset;

        // Y translation
        private int transY;


        // Draw tilemap
        private void DrawTilemap(Graphics g, Camera cam, int tx = 0, int ty = 0, int outline = 0, bool black=false)
        {
            g.BeginDrawing();

            // Get top left corner & viewport
            Vector2 topLeft = cam.GetTopLeftCorner();
            Vector2 bottomRight = cam.GetBottomRightCorner();
            Vector2 view = cam.GetViewport();

            // Compute starting positions
            int sx = (int)(topLeft.X / TILE_SIZE) - 1;
            int sy = (int)( (topLeft.Y-transY) / TILE_SIZE) - 1;

            int ex = (int)(bottomRight.X / TILE_SIZE) + 1;
            int ey = (int)( (bottomRight.Y- transY) / TILE_SIZE) + 1;

            // Draw tiles
            int tile = 0;
            int srcx, srcy;

            for (int y = sy; y <= ey; ++y)
            {
                for (int x = sx; x <= ex; ++x)
                {
                    // Get tile
                    tile = map.GetTile(0, x, y);
                    if (tile == -1)
                        continue;

                    // If rendering black outlines, not every tile
                    // must be re-rendered
                    if(black)
                    {
                        if (map.GetTile(0, x + tx, y + ty) > 0)
                            continue;
                    }

                    // Draw a black tile if solid
                    if (tile-- != 0)
                    {
                        srcx = tile % 16;
                        srcy = tile / 16;

                        g.DrawScaledBitmapRegion(bmpTileset, srcx * 64, srcy * 64, 64, 64,
                                                 tx*outline + x * TILE_SIZE, 
                                                 ty*outline + y * TILE_SIZE + transY, 
                                                 TILE_SIZE, TILE_SIZE);
                    }
                }
            }

            g.EndDrawing();
        }


        // Map
        public Stage(AssetPack assets, int index)
        {
            // Get current map
            map = assets.GetTilemap(index.ToString());
            bmpTileset = assets.GetBitmap("tileset");

            // Compute y translation
            transY = -map.GetHeight() * TILE_SIZE;
        }


        // Update
        public void Update(float tm)
        {
            // ...
        }


        // Player collision
        public void GetObjectCollision(CollisionObject pl, float tm, bool floorPlus = false)
        {
            const int CHECK = 5;
            const float WIDTH_PLUS = 8.0f;

            float widthPlus = floorPlus ? WIDTH_PLUS : 0.0f;

            Vector2 p = pl.GetPos();

            // Compute starting positions
            int sx = (int)(p.X / TILE_SIZE) - CHECK/2;
            int sy = (int)( (p.Y-transY) / TILE_SIZE) - CHECK/2;

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
                        
                    // If collision tile
                    if(tile != 0)
                    {
                        // Check if nearby tiles are empty
                        if (map.GetTile(0, x, y - 1) == 0)
                        {
                            pl.GetFloorCollision(x * TILE_SIZE - widthPlus,
                                                 y * TILE_SIZE + transY,
                                                 TILE_SIZE + widthPlus * 2, tm);
                        }

                        if (map.GetTile(0, x, y + 1) == 0)
                        {
                            pl.GetCeilingCollision(x * TILE_SIZE, 
                                                   (y + 1) * TILE_SIZE + transY,
                                                   TILE_SIZE, tm);
                        }

                        if (map.GetTile(0, x - 1, y) <= 0)
                        {
                            pl.GetWallCollision(x * TILE_SIZE, y * TILE_SIZE + transY,
                                                TILE_SIZE, 1, tm);
                        }

                        if (map.GetTile(0, x + 1, y) <= 0)
                        {
                            pl.GetWallCollision((x + 1) * TILE_SIZE, y * TILE_SIZE + transY,
                                                TILE_SIZE, -1, tm);
                        }
                    }
                }
            }

        }


        // Draw
        public void Draw(Graphics g, Camera cam)
        {
            const int OUTLINE = 2;

            // Draw black outlines
            g.SetColor(0, 0, 0);
            for (int y = -1; y <= 1; ++ y)
            {
                for (int x = -1; x <= 1; ++ x)
                {
                    if (x == y && x == 0) continue;

                    DrawTilemap(g, cam, x, y, OUTLINE, true);
                }
            }


            // Draw with colors
            g.SetColor();
            DrawTilemap(g, cam);
        }
    }
}
