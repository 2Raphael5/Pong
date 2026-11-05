using Class;
using Class.DebugTools;
using Class.Generic;
using Class.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pong.Class.Play;
using Simulation_etre_vivant.Class.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
public enum GameState
{
    Playing,
    Pause,
    Menu
}
namespace Pong
{
    public class Game1 : Game
    {
        private InputManager _input;
        private MainMenu _mainMenu;
        private Hud _hud;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        SpriteFont _font;
        SpriteFont _pixelart;

        List<Player> _players;
        Ball _ball;

        public int scoreP1;
        public int scoreP2;

        public const int VirtualWidth = 1920;
        public const int VirtualHeight = 1080;
        private bool _resizing;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;
            _graphics.PreferredBackBufferWidth = VirtualWidth;
            _graphics.PreferredBackBufferHeight = VirtualHeight;

            Window.ClientSizeChanged += (s, e) =>
            {
                if (_resizing) return;
                _resizing = true;
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                _graphics.ApplyChanges();
                _resizing = false;
            };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GameStateManager.ChangeState(GameState.Menu);
            _input = new InputManager();
            _mainMenu = new MainMenu(); _players = new List<Player>
            {
                new Player(new PixelGeneric(new Vector2(0, (VirtualHeight - 50) / 2), Color.White), 10, 50, Vector2.Zero),
                new Player(new PixelGeneric(new Vector2(VirtualWidth - 10, (VirtualHeight - 50) / 2), Color.White), 10, 50, Vector2.Zero),
            };
            Texture2D ballTexture = Content.Load<Texture2D>("Img/football");
            Random rnd = new Random();
            Vector2 direction = new Vector2(rnd.Next(-500, 500), rnd.Next(-500, 500)); 
            do
            {
                direction = new Vector2(rnd.Next(-500, 500), rnd.Next(-500, 500));
            }
            while ((direction.X > -250 && direction.X < 250) || (direction.Y > -250 && direction.Y < 250));
            direction = new Vector2(rnd.Next(-500, 500), rnd.Next(-500, 500));
            _ball = new Ball(new SpriteGeneric(ballTexture), direction);
            _ball.SpriteGeneric.Position = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
            _font = Content.Load<SpriteFont>("Font/Font");
            _pixelart = Content.Load<SpriteFont>("Font/pixelart");
            _hud = new Hud(_pixelart);
            PrimitiveRenderer.Initialize(GraphicsDevice);
            _ball.SpriteGeneric.Position = new Vector2(VirtualWidth / 2, VirtualHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    UpdateMainMenu();
                    break;
                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;
                case GameState.Pause:
                    UpdatePause();
                    break;
            }
            _input.Update(gameTime);
            base.Update(gameTime);
        }
        private void UpdateMainMenu()
        {
            if (_input.IsJustPressed(InputAction.Exit))
            {
                Application.Exit();
            }
            _mainMenu.Update(_input);
            if (_mainMenu.IsConfirmed(_input))
            {
                if (_mainMenu.SelectedIndex == 0)
                {
                    ResetGame();
                }
                else
                {
                    Exit();
                }
            }
        }
        private void UpdatePlaying(float deltaTime)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].Update(deltaTime, _input, i, this);
            }
            _ball.Update(deltaTime, this, _players);
        }
        private void UpdatePause()
        {

        }
        protected override void Draw(GameTime gameTime)
        {
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Fond noir sur toute la fenêtre (bandes noires)
            GraphicsDevice.Viewport = new Viewport(0, 0, w, h);
            GraphicsDevice.Clear(Color.Black);

            // Échelle uniforme + zone de jeu centrée
            float scale = Math.Min(w / (float)VirtualWidth, h / (float)VirtualHeight);
            int vw = (int)(VirtualWidth * scale);
            int vh = (int)(VirtualHeight * scale);
            GraphicsDevice.Viewport = new Viewport((w - vw) / 2, (h - vh) / 2, vw, vh);

            _spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale, scale, 1f));

            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Playing:
                    DrawPlaying();
                    DrawHUD();
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMenu()
        {
            Vector2 textSize = _font.MeasureString("Play");
            _mainMenu.Draw(_spriteBatch, _font,
                new Vector2((VirtualWidth - textSize.X) / 2f, (VirtualHeight - textSize.Y) / 2f));
        }

        private void DrawPlaying()
        {
            _players.ForEach(e => e.Draw(_spriteBatch));
            _ball.Draw(_spriteBatch);
        }


        private void DrawHUD()
        {
            _hud.DrawMenue(_spriteBatch, scoreP1, scoreP2, this);
        }
        private void ResetGame()
        {
            GameStateManager.ChangeState(GameState.Playing);
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            int w = Window.ClientBounds.Width;
            int h = Window.ClientBounds.Height;

            _graphics.PreferredBackBufferWidth = w;
            _graphics.PreferredBackBufferHeight = h;
            _graphics.ApplyChanges();

            _players[0].PixelGeneric.Position = new Vector2(0, (h - _players[0].PixelGeneric.Height) / 2);

            _players[1].PixelGeneric.Position = new Vector2(w - _players[1].PixelGeneric.Width, (h - _players[1].PixelGeneric.Height) / 2);
        }
    }
}
