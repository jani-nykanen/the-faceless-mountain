using System;

using Microsoft.Xna.Framework;

using monogame_experiment.Desktop.Core;
using monogame_experiment.Desktop.Field.Enemies;


namespace monogame_experiment.Desktop.Field
{
    // Stage
    public class Stage
    {
        // Collision tile row
        const int COLLISION_ROW = 6;

        // Tile size
        public const int TILE_SIZE = 64;
        // Cloud sizes
        const int CLOUD_WIDTH = 768;
        const int CLOUD_HEIGHT = 192;

        // Tilemap
        private Tilemap map;

        // Bitmaps
        private Bitmap bmpTileset;
        private Bitmap bmpWater;
        private Bitmap bmpCloud;
        private Bitmap bmpSky;

        // Y translation
        private int transY = 0;
        // Water position
        private float waterPos = 0.0f;
        // Water wave
        private float waterWave = 0.0f;

        // Cloud positions
        private float[] cloudPos;


        // Draw tilemap
        private void DrawTilemap(int layer, Bitmap bmp, Graphics g, Camera cam,
                                 int tx = 0, int ty = 0, int outline = 0, bool black = false,
                                 int xpad = 0, int ypad = 0)
        {
            g.BeginDrawing();

            // Get top left corner & viewport
            Vector2 topLeft = cam.GetTopLeftCorner();
            Vector2 bottomRight = cam.GetBottomRightCorner();
            Vector2 view = cam.GetViewport();

            // Compute starting positions
            int sx = (int)(topLeft.X / TILE_SIZE) - 1;
            int sy = (int)((topLeft.Y - transY) / TILE_SIZE) - 1;

            int ex = (int)(bottomRight.X / TILE_SIZE) + 1;
            int ey = (int)((bottomRight.Y - transY) / TILE_SIZE) + 1;

            // Draw tiles
            int tile = 0;
            int srcx, srcy;

            // TODO: Get info from the bitmap!
            float scaleFactor = 64 / TILE_SIZE;

            for (int y = sy; y <= ey; ++y)
            {
                for (int x = sx; x <= ex; ++x)
                {
                    // Get tile
                    tile = map.GetTile(layer, x, y);
                    if (tile == -1)
                        continue;

                    // If rendering black outlines, not every tile
                    // must be re-rendered
                    if (layer == 0 && black && ((tile - 1) / 16) != COLLISION_ROW)
                    {
                        if (!IsFree(x + tx, y + ty))
                            continue;
                    }

                    // Draw a black tile if solid
                    if (tile-- != 0)
                    {
                        srcx = tile % 16;
                        srcy = tile / 16;

                        g.DrawScaledBitmapRegion(bmp,
                                                 srcx * (64 + xpad * 2), srcy * (64 + ypad * 2),
                                                 64, 64,
                                                 tx * outline + x * TILE_SIZE,
                                                 ty * outline + y * TILE_SIZE + transY,
                                                 TILE_SIZE + scaleFactor,
                                                 TILE_SIZE + scaleFactor);
                    }
                }
            }

            g.EndDrawing();
        }


        // Draw water
        private void DrawWater(Graphics g, Camera cam, bool background = false)
        {
            const float WAVE_AMPL = 8.0f;
            const int MOVE_X = 32;
            const float MOVE_WAVE = (float)Math.PI / 2.0f;

            float ypos = background ? -TILE_SIZE : -TILE_SIZE / 1.5f;

            g.BeginDrawing();

            // Get top left corner & viewport
            Vector2 topLeft = cam.GetTopLeftCorner();
            Vector2 bottomRight = cam.GetBottomRightCorner();
            Vector2 view = cam.GetViewport();

            // Compute starting & ending positions
            int sx = (int)(topLeft.X / TILE_SIZE) - 1;
            int ex = (int)(bottomRight.X / TILE_SIZE) + 2;
            int ey = (int)((bottomRight.Y - transY) / TILE_SIZE) + 1;

            if (ey < 0) return;

            // Compute wave "height"
            float wavePlus = background ? 0.0f : MOVE_WAVE;
            int wave = (int)((float)Math.Sin(waterWave + wavePlus) * WAVE_AMPL);

            // Draw water
            int xpos = (int)waterPos + (background ? 0 : MOVE_X);
            for (int i = sx; i <= ex; ++i)
            {
                g.DrawScaledBitmapRegion(bmpWater, 0, background ? 80 : 0, 64, 64,
                                         i * TILE_SIZE - xpos,
                                         (int)ypos + wave,
                                         TILE_SIZE, TILE_SIZE);
            }

            g.EndDrawing();
        }


        // Draw a cloud layer
        private void DrawCloudLayer(Graphics g, float pos, int y)
        {
            int w = (int)g.GetViewport().X;
            int h = (int)g.GetViewport().Y;

            for (int i = 0; i <= w / CLOUD_WIDTH + 1; ++i)
            {
                g.DrawScaledBitmap(bmpCloud, i * CLOUD_WIDTH + (int)pos,
                                   h - CLOUD_HEIGHT + y,
                                   CLOUD_WIDTH, CLOUD_HEIGHT);
            }
        }


        // Draw clouds
        private void DrawClouds(Graphics g)
        {
            const int BOTTOM = 128;
            const int YOFF = 64;

            g.SetColor(0.30f, 0.50f, 0.90f);
            DrawCloudLayer(g, -cloudPos[0], -BOTTOM);

            g.SetColor(0.45f, 0.625f, 0.95f);
            DrawCloudLayer(g, -cloudPos[1], -BOTTOM + YOFF);

            g.SetColor(0.70f, 0.80f, 1.0f);
            DrawCloudLayer(g, -cloudPos[2], -BOTTOM + 2 * YOFF);

            g.SetColor();
        }


        // Is the tile free
        private bool IsFree(int x, int y, int l = 0)
        {
            int v = map.GetTile(l, x, y);
            return (v - 1) / 16 == COLLISION_ROW || v <= 0;
        }


        // Map
        public Stage(AssetPack assets, int index)
        {
            // Get current map
            map = assets.GetTilemap(index.ToString());
            bmpTileset = assets.GetBitmap("tileset");
            bmpWater = assets.GetBitmap("water");
            bmpCloud = assets.GetBitmap("cloud");
            bmpSky = assets.GetBitmap("sky");

            // Compute y translation
            transY = -map.GetHeight() * TILE_SIZE;

            // Create cloud positions
            cloudPos = new float[3];
            int xoff = CLOUD_WIDTH / 3;
            for (int i = 0; i < 3; ++i)
            {
                cloudPos[i] += i * xoff;
            }
        }


        // Update
        public void Update(float tm)
        {
            const float WATER_SPEED = 1.0f;
            const float WATER_WAVE = 0.025f;
            const float BASE_CLOUD_SPEED = 2.0f;

            // Update water
            waterPos += WATER_SPEED * tm;
            waterPos %= 64;
            waterWave += WATER_WAVE * tm;

            // Update clouds
            for (int i = 0; i < 3; ++i)
            {
                cloudPos[i] += BASE_CLOUD_SPEED * (i + 1) * tm;
                cloudPos[i] %= CLOUD_WIDTH;
            }
        }


        // Player collision
        public void GetObjectCollision(CollisionObject pl, float tm, bool floorPlus = false)
        {
            const int CHECK = 5;
            const float WIDTH_PLUS = 8.0f;

            const float HURT_WIDTH = 32.0f;
            const float HURT_HEIGHT = 32.0f;

            float widthPlus = floorPlus ? WIDTH_PLUS : 0.0f;

            Vector2 p = pl.GetPos();

            // Compute starting positions
            int sx = (int)(p.X / TILE_SIZE) - CHECK / 2;
            if (sx < 0) sx = 0;

            int sy = (int)((p.Y - transY) / TILE_SIZE) - CHECK / 2;
            if (sy < 0) sy = 0;

            int ex = sx + CHECK;
            if (ex >= map.GetWidth()) ex = map.GetWidth() - 1;

            int ey = sy + CHECK;
            if (ey >= map.GetHeight()) ey = map.GetHeight() - 1;

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
                    if (tile != 0 && (tile - 1) / 16 != COLLISION_ROW)
                    {
                        // Check if nearby tiles are empty
                        if (IsFree(x, y - 1))
                        {
                            pl.GetFloorCollision(x * TILE_SIZE - widthPlus,
                                                 y * TILE_SIZE + transY,
                                                 TILE_SIZE + widthPlus * 2, tm);
                        }

                        if (IsFree(x, y + 1))
                        {
                            pl.GetCeilingCollision(x * TILE_SIZE,
                                                   (y + 1) * TILE_SIZE + transY,
                                                   TILE_SIZE, tm);
                        }

                        if (IsFree(x - 1, y))
                        {
                            pl.GetWallCollision(x * TILE_SIZE, y * TILE_SIZE + transY,
                                                TILE_SIZE, 1, tm);
                        }

                        if (IsFree(x + 1, y))
                        {
                            pl.GetWallCollision((x + 1) * TILE_SIZE, y * TILE_SIZE + transY,
                                                TILE_SIZE, -1, tm);
                        }
                    }
                    else if ((tile - 1) / 16 == COLLISION_ROW)
                    {
                        float hx = x * TILE_SIZE + (TILE_SIZE - HURT_WIDTH) / 2;
                        float hy = y * TILE_SIZE + (TILE_SIZE - HURT_HEIGHT) / 2;


                        pl.GetHurtCollision(hx, hy + transY, HURT_WIDTH, HURT_HEIGHT);
                    }
                }
            }

        }


        // Draw background
        public void DrawBackground(Graphics g)
        {
            const int SKY_WIDTH = 320;
            const int SKY_HEIGHT = 640;

            g.SetColor();
            g.Identity();
            g.IdentityWorld();
            g.BeginDrawing();

            // Draw sky
            int max = (int)(g.GetViewport().X / SKY_WIDTH);
            for (int i = 0; i < max; ++i)
            {
                g.DrawScaledBitmapRegion(bmpSky, i == max - 1 ? 240 : 0, 0, 240, 480,
                                         i * SKY_WIDTH, 0, SKY_WIDTH, SKY_HEIGHT);
            }


            // Draw clouds
            DrawClouds(g);

            g.EndDrawing();
        }


        // Draw
        public void Draw(Graphics g, Camera cam)
        {
            const int OUTLINE = 2;
            const int PADDING = 1;

            // Draw background water
            DrawWater(g, cam, true);

            // Draw decorations
            DrawTilemap(1, bmpTileset, g, cam, 0, 0, 0, false, PADDING, PADDING);

            // Draw black outlines
            g.SetColor(0, 0, 0);
            for (int y = -1; y <= 1; ++y)
            {
                for (int x = -1; x <= 1; ++x)
                {
                    if (x == y && x == 0) continue;

                    DrawTilemap(0, bmpTileset, g, cam, x, y, OUTLINE, true, PADDING, PADDING);
                }
            }


            // Draw with colors
            g.SetColor();
            DrawTilemap(0, bmpTileset, g, cam, 0, 0, 0, false, PADDING, PADDING);

        }


        // Post-draw
        public void PostDraw(Graphics g, Camera cam)
        {
            // Draw the front layer of water
            DrawWater(g, cam);
        }


        // Parse objects
        public void ParseObjects(ObjectManager objMan)
        {
            const int ENEMY_INDEX = 9 * 16;
            const int LAYER = 2;

            Enemy e = null;
            Vector2 target;

            // Go through tiles and find enemies
            int tile = 0;
            for (int y = 0; y < map.GetHeight(); ++y)
            {
                for (int x = 0; x < map.GetWidth(); ++x)
                {
                    tile = map.GetTile(LAYER, x, y) - 1;
                    if (tile < ENEMY_INDEX || tile >= ENEMY_INDEX + 16)
                    {
                        continue;
                    }
                    tile -= ENEMY_INDEX;

                    // Add enemy
                    e = null;
                    target = new Vector2(x * TILE_SIZE + TILE_SIZE / 2,
                                                 (y + 1) * TILE_SIZE + transY);
                    switch (tile)
                    {
                        // Horizontal fly
                        case 0:

                            e = new HorizontalFly(target.X, target.Y);
                            break;

                        // Vertical fly
                        case 1:

                            e = new VerticalFly(target.X, target.Y);
                            break;

                        // Falling fly, no flip
                        case 2:
                            e = new FallingFly(target.X, target.Y);
                            break;

                        // Falling fly, flip vertical
                        case 3:
                            e = new FallingFly(target.X, target.Y, Graphics.Flip.Vertical);
                            break;

                        // Static fly
                        case 4:
                            e = new StaticFly(target.X, target.Y);
                            break;

                        // Horizontal follower
                        case 5:
                            e = new HorizontalFollower(target.X, target.Y);
                            break;

                        // Vertical follower
                        case 6:
                            e = new VerticalFollower(target.X, target.Y);
                            break;

                        // Vertical thwomp
                        case 7:
                            e = new VerticalThwomp(target.X, target.Y);
                            break;

                        // Vertical thwomp, flip
                        case 8:
                            e = new VerticalThwomp(target.X, target.Y, Graphics.Flip.Vertical);
                            break;

                        // Horizontal thwomp
                        case 9:
                            e = new HorizontalThwomp(target.X, target.Y);
                            break;

                        // Horizontal thwomp, flip
                        case 10:
                            e = new HorizontalThwomp(target.X, target.Y, Graphics.Flip.Horizontal);
                            break;

                        default:
                            break;
                    }

                    // If not null, add
                    if (e != null)
                    {
                        objMan.AddEnemy(e);
                    }
                }
            }
        }
    }
}
