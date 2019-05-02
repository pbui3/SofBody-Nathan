using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MathPhysSoftBody
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SoftBodyGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        protected const int WindowWidth = 400, WindowHeight = 400;
        Texture2D pVisual;

        SoftBody sb;
        public SoftBodyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();
            sb = new SoftBody(20, 1f, 100, 50, 1f, 10, 500, WindowWidth, WindowHeight);
            //Dampening - 0-1: oscillates, fast - 1: fastest  - >1: slower
            sb.CreateBall();
            //sb.CreateBox();
            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pVisual = Content.Load<Texture2D>("point");
            // TODO: use this.Content to load your game content here
        }


        protected override void Update(GameTime gameTime)
        {
           

            // TODO: Add your update logic here
            sb.Update(gameTime);

            
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            sb.Draw(spriteBatch,pVisual);
            // TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
