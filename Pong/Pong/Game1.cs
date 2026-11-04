using Class;
using Class.DebugTools;
using Class.Generic;
using Class.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pong.Class.Play;
using Simulation_etre_vivant.Class.Input;
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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        SpriteFont _font;

        List<Player> _players;
        Ball _ball;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GameStateManager.ChangeState(GameState.Menu);
            _input = new InputManager();
            _mainMenu = new MainMenu();
            _players = new List<Player>
            {
                new Player(new PixelGeneric(new Vector2(0,(Window.ClientBounds.Height-50) / 2), Color.White), 10, 50, new Vector2(0,0)),
                new Player(new PixelGeneric(new Vector2(Window.ClientBounds.Width-10,(Window.ClientBounds.Height-50) / 2), Color.White), 10, 50, new Vector2(0,0)),
            };
            //_ball = new Ball();
            _font = Content.Load<SpriteFont>("Font/Font");
            PrimitiveRenderer.Initialize(GraphicsDevice);
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
        }
        private void UpdatePause()
        {

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;
                case GameState.Playing:
                    DrawPlaying(_spriteBatch);
                    break;
            }
            base.Draw(gameTime);

            _spriteBatch.End();
        }

        private void DrawMenu()
        {
            Vector2 textSize = _font.MeasureString("Play");
            _mainMenu.Draw(_spriteBatch, _font, new Vector2((_graphics.PreferredBackBufferWidth - textSize.X) / 2f, (_graphics.PreferredBackBufferHeight - textSize.Y) / 2f));
        }
        private void DrawPlaying(SpriteBatch _spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            _players.ForEach(e => e.Draw(_spriteBatch));
            //_ball.Draw(_spriteBatch);
        }
        
        
        private void ResetGame()
        {
            GameStateManager.ChangeState(GameState.Playing);
        }
    }
}
