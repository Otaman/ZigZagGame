using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ZigZagGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        
        private Model _startBlock;
        private Model _block;
        private Model _ball;
        private Model _octahedron;
        private Model _fraction;

        private SpriteFont _counterFont;
        private SpriteFont _titleFont;
        public static int Counter;
        private int _maxCount;

        private BlockHelper _blockHelper;
        private BallHelper _ballHelper;
        private OctahedronHelper _octahedronHelper;
        private FractionHelper _fractionHelper;

        private bool _gameOver;

        public Game1()
        {
            var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            if (width > height)
            {
                var w = width;
                width = height;
                height = w;
            }

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.SupportedOrientations = DisplayOrientation.Portrait;
            _graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

//            int ticks = 166667;
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _startBlock = Content.Load<Model>("Models/start_block");
            _block = Content.Load<Model>("Models/block");
            _ball = Content.Load<Model>("Models/ball4");
            _octahedron = Content.Load<Model>("Models/octahedron4");
            _fraction = Content.Load<Model>("Models/explode1");
            _counterFont = Content.Load<SpriteFont>("TopRightCornerCounter");
            _titleFont = Content.Load<SpriteFont>("TitleFont");

            Matrix world = Matrix.CreateScale(0.01f);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 16f, 20f), Vector3.Zero, Vector3.Up);
            Matrix projection = /*Matrix.CreatePerspectiveFieldOfView(
              MathHelper.PiOver4,
              GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);*/
                Matrix.CreateOrthographic(12, 24, 0.1f, 100);

            _fractionHelper = new FractionHelper(_fraction, world, view, projection);
            _fractionHelper.InitBeginState();

            _octahedronHelper = new OctahedronHelper(_octahedron, world, view, projection);
            _octahedronHelper.InitBeginState();

            _blockHelper = new BlockHelper(world, view, projection, _octahedronHelper);
            _blockHelper.InitBeginState(_block, _startBlock);

            _ballHelper = new BallHelper(world, view, projection);
            _ballHelper.InitBeginState(_ball, new Vector3(0, 2.333f, 0));
            
            ReadBestResult();
        }

        private void ReadBestResult()
        {
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream fs = savegameStorage.OpenFile("Score.txt", FileMode.OpenOrCreate))
            {
                byte[] buffer = new byte[4];

                fs.Read(buffer, 0, 4);
                _maxCount = BitConverter.ToInt32(buffer, 0);
            }
        }

        protected override void UnloadContent()
        {
        }

        private bool _tapActive;
        private bool _firstTap = true;
        private bool _counterWrited;

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            
            if (_gameOver)
            {
                VelocityHelper.ClearVelocity();

                if (Counter > _maxCount)
                {
                    _maxCount = Counter;
                    SaveCounter();    
                }
            }

            if (!MHelper.BallOnBlock(_ballHelper.GetBallPosition(), _blockHelper.GetBlocksOnZeroZ()))
            {
                _ballHelper.Fall();
            }

            if (TouchPanel.GetState().Count > 0)
            {
                if (!_tapActive)
                {
                    if (_firstTap)
                    {
                        VelocityHelper.InitVelocity();
                        _firstTap = false;
                    }
                    else
                    {
                        if (_gameOver)
                        {
                            _gameOver = false;
                            _firstTap = false;
                            Counter = 0;
                            _counterWrited = false;
                            _octahedronHelper.InitBeginState();
                            _fractionHelper.InitBeginState();
                            _blockHelper.InitBeginState(_block, _startBlock);
                            _ballHelper.InitBeginState(_ball, new Vector3(0, 2.333f, 0));
                            VelocityHelper.InitVelocity();
                        }
                        else
                        {
                            _ballHelper.ShangeVelocity();    
                        }
                        
                    }
                    _tapActive = true;
                }
            }
            else
            {
                _tapActive = false;
            }

            
            MHelper.ExplodeOctahedrons(_ballHelper.GetBallPosition(), _octahedronHelper, _fractionHelper);

            if (_ballHelper.GetBallPosition().Y < -6)
            {
                _gameOver = true;
            }
            
            base.Update(gameTime);
        }

        private void SaveCounter()
        {
            if (_counterWrited)
            {
                return;
            }
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream fs = savegameStorage.OpenFile("Score.txt", FileMode.Create))
            {
                byte[] bytes = BitConverter.GetBytes(Counter);
                fs.Write(bytes, 0, bytes.Length);
            }
            _counterWrited = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _blockHelper.UpdateElements();
            _octahedronHelper.UpdateElements();
            _fractionHelper.UpdateElements();

            _blockHelper.DrawElements();
            _octahedronHelper.DrawElements();
            _fractionHelper.DrawElements();

            _ballHelper.DrawElement();


            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            _spriteBatch.Begin();
            string counterRepresentation = Counter.ToString();
            _spriteBatch.DrawString(
                _counterFont, 
                counterRepresentation, 
                new Vector2(460 - 18*counterRepresentation.Length, 5), 
                new Color(new Vector3(0.1f)));

            if (_firstTap)
            {
                DrawTitle();
            }
            if (_gameOver)
            {
                DrawGameOver();
            }

            _spriteBatch.End();

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds > 0)
            {
                Debug.WriteLine(1f / seconds);
            }

            base.Draw(gameTime);
        }

        private void DrawGameOver()
        {
            Texture2D rect = new Texture2D(_graphics.GraphicsDevice, 420, 320);

            Color panelColor = Color.Lerp(new Color(Block.CurrentColor), Color.White, 0.8f);

            Color[] data = new Color[420 * 320];
            for (int i = 0; i < data.Length; ++i) 
                data[i] = panelColor;
            rect.SetData(data);

            Vector2 coor = new Vector2(30, 120);
            _spriteBatch.Draw(rect, coor, Color.White);

            _spriteBatch.DrawString(
                _titleFont,
                "Game Over",
                new Vector2(120, 130),
                new Color(new Vector3(0.1f)));

            _spriteBatch.DrawString(
                _titleFont,
                "Score:",
                new Vector2(80, 220),
                Color.Gray);
            _spriteBatch.DrawString(
                _titleFont,
                Counter.ToString(),
                new Vector2(260, 220),
                Color.Gray);
            _spriteBatch.DrawString(
                _titleFont,
                "Best:",
                new Vector2(80, 280),
                Color.Gray);
            _spriteBatch.DrawString(
                _titleFont,
                _maxCount.ToString(),
                new Vector2(260, 280),
                Color.Gray);

            _spriteBatch.DrawString(
                _counterFont,
                "Tap to play again",
                new Vector2(50, 370),
                new Color(Block.CurrentColor));
        }

        private void DrawTitle()
        {
            _spriteBatch.DrawString(
                    _titleFont,
                    "ZigZag Master",
                    new Vector2(80, 100),
                    new Color(new Vector3(0.1f)));

            _spriteBatch.DrawString(
                _counterFont,
                "Tap to play",
                new Vector2(120, 160),
                Color.Gray);  
        }
    }
}
