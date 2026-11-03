using Class;
using Class.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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


            _font = Content.Load<SpriteFont>("Font/Font");
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
                    //ResetSimulation();
                }
                else
                {
                    Exit();
                }
            }
        }
        private void UpdatePlaying(float deltaTime)
        {

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

        }
    }
}
