using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using monogame_experiment.Desktop.Core;
using monogame_experiment.Desktop.Field;


namespace monogame_experiment.Desktop
{
    
    // A runner class, starts application and pass
    // some useful data to other classes
    public class Runner : Game
    {      
        // Base application
        private Application appBase;

        // Graphics device manager
        private GraphicsDeviceManager gman;

        
        // Constructor
        public Runner()
        {
            Content.RootDirectory = "Content";

            // Create graphics device manager
            gman = new GraphicsDeviceManager(this);

			// Create configuration
			Configuration conf = Configuration.ReadXML("config.xml");
            
            // Create application base for custom app behavior
            appBase = new Application(conf, gman, this);

			// Add scenes
			appBase.AddScene(new GameField(), true);
            appBase.AddScene(new Ending(), false);
			appBase.AddScene(new Global(), false, true);
        }


        // Initialize
        protected override void Initialize()
        {         
            // Initialize base here
            base.Initialize();         
        }


        // Load content
        protected override void LoadContent()
        {
            // Initialize application graphics
            appBase.InitGraphics(GraphicsDevice);

            // Load game assets
            appBase.LoadAssets();

			// Initialize scenes
            appBase.InitScenes();
        }
        

        // Unload content
        protected override void UnloadContent()
        {
            // Destroy application
            appBase.Destroy();
        }
        

        // Update
        protected override void Update(GameTime gameTime)
        {
            // Update application base
            appBase.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);

            base.Update(gameTime);
        }

        
        // Draw
        protected override void Draw(GameTime gameTime)
        {         
            // Draw application base
            appBase.Draw();

            base.Draw(gameTime);
        }
    }
}
