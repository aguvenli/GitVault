using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToW.Model;
using ToW.Model.Providers;
using System.Linq;
using Model.Arenas;
using Model.Entities;
using Model.Entities.Helpers;
using ToW.Win8.Network;

namespace ToW.Win8
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Arena Arena = new Arena();

        bool isServer = false;
        TCPServer serv;
        Client cli;

        public bool c { get; set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Client cli;
            //cli = new Client("198.168.2.72", 9000);
            //cli = new Client("127.0.0.1", 9000);
            //cli.OnMessageReceived += cli_OnMessageReceived;
            //c = false;
            //cli.Connect(this);

            //do
            //{
            //    //nofink
            //} while (!c);

            Arena.Initialize();
            
            CreateSampleUnits();

            base.Initialize();

        }

        void cli_OnMessageReceived(Client sender, Windows.Storage.Streams.IBuffer args)
        {
            throw new System.NotImplementedException();
        }

        private void CreateSampleUnits()
        {

            int RowCount = 10;
            int ColumnCount = 1;
           

            Unit u;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    u = (Unit) Arena.p1.BuildingTemplates[4].Product.Clone();
                    u.Owner = Arena.p1;
                    u.Location = new Location()
                        {
                            Grid = new Vector2(x - Constants.GRID_HORIZONTAL_OFFSET, y - RowCount / 2)
                        };
                   
                    Arena.AllItems.Add(u);
                }
            }
            /*
            Arena.ParseNetworkString("          \n          \n    H     \n     MM   \n    AA    \n          \n");
            */
            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < 9; y++)
                {

                    u = (Unit)Arena.p2.BuildingTemplates[4].Product.Clone();
                    u.Owner = Arena.p2;
                    u.Location = new Location()
                        {
                            Grid = new Vector2(Constants.GRID_HORIZONTAL_OFFSET - x, y - RowCount / 2)
                        };

                    Arena.AllItems.Add(u);
                }
            }

            Arena.AllItems= Arena.AllItems.OrderBy(a => a.GetHashCode()).ToList();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);           

            // TODO: use this.Content to load your game content here
            GameTimeProvider.Current = new GameTimeProvider();
            GraphicsProvider.Current = new GraphicsProvider();
            GraphicsProvider.Current.Game = this; 
            GraphicsProvider.Current.SpriteBatch = _spriteBatch;
            GraphicsProvider.Current.Initialize();
            MouseProvider.Current = new MouseProvider();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTimeProvider.Current.GameTime = gameTime;
            // TODO: Add your update logic here
            Keys[] KeysBuffer= Keyboard.GetState().GetPressedKeys();
            if (KeysBuffer.Contains(Keys.Escape))
            {
                this.Exit();

            }
            MouseProvider.Current.MouseState = Mouse.GetState();
            Arena.Update(Mouse.GetState());

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Orange);
            
            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            Arena.Draw();
            GraphicsProvider.Current.Draw(GraphicsProvider.Current.Flat, new Rectangle(900, 0, 20000, 20000));
            GraphicsProvider.Current.Draw(GraphicsProvider.Current.Flat, new Rectangle(0, 600, 20000, 20000));
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
