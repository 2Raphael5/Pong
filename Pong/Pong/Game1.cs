
using Class;
using Class.DebugTools;
using Class.Generic;
using Class.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pong.Class.Play;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

// Définit les différents écrans du jeu.
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
        private AudioManager _audio;
        private Song _gameplayMusic;

        float player1Speed = 2000f;
        float player2Speed = 2000f;

        int player1Width = 20;
        int player2Width = 10;

        int player1Height = 300;
        int player2Height = 300;

        float ballSize = 0.1f;

        Color backgroundColor = Color.Black;

        Color player1Color = Color.Blue;
        Color player2Color = Color.Green;

        // Ressources du jeu.
        string ballImg = "bille";
        string music = "arcade";

        string menuFont = "Font";
        string scoreFont = "pixelart";

        #region Autres Variables

        Vector2 direction;

        private InputManager _input;
        private MainMenu _mainMenu;
        private Hud _hud;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont _font;
        SpriteFont _scoreFont;

        List<Player> _players;
        Ball _ball;

        public int scoreP1;
        public int scoreP2;

        // Résolution virtuelle du jeu.
        public const int VirtualWidth = 1920;
        public const int VirtualHeight = 1080;

        Random rnd;

        #endregion

        // Constructeur.
        public Game1()
        {
            rnd = new Random();

            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            // Active le plein écran.
            _graphics.IsFullScreen = true;

            // Utilise la résolution native de l'écran.
            _graphics.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            _graphics.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // Synchronisation verticale.
            _graphics.SynchronizeWithVerticalRetrace = true;

            // Applique les paramètres graphiques.
            _graphics.ApplyChanges();

            // Désactive le redimensionnement manuel.
            Window.AllowUserResizing = false;
        }

        // Initialisation du jeu.
        protected override void Initialize()
        {
            // Chargement des polices.
            _font = Content.Load<SpriteFont>(menuFont);
            _scoreFont = Content.Load<SpriteFont>(scoreFont);

            // Affiche le menu principal.
            GameStateManager.ChangeState(GameState.Menu);

            // Gestion des entrées.
            _input = new InputManager();

            // Création du menu.
            _mainMenu = new MainMenu();

            // Création des joueurs.
            _players = new List<Player>
            {
                new Player(
                    new PixelGeneric(
                        new Vector2(
                            0,
                            (VirtualHeight - player1Height) / 2
                        ),
                        player1Color
                    ),
                    player1Width,
                    player1Height,
                    Vector2.Zero,
                    player1Speed
                ),

                new Player(
                    new PixelGeneric(
                        new Vector2(
                            VirtualWidth - player2Width,
                            (VirtualHeight - player2Height) / 2
                        ),
                        player2Color
                    ),
                    player2Width,
                    player2Height,
                    Vector2.Zero,
                    player2Speed
                )
            };

            // Chargement de la texture de la balle.
            Texture2D ballTexture =
                Content.Load<Texture2D>(ballImg);

            // Création de la balle.
            _ball = new Ball(
                new SpriteGeneric(ballTexture),
                direction,
                ballSize
            );

            // Position initiale de la balle.
            _ball.SpriteGeneric.Position = new Vector2(
                VirtualWidth / 2,
                VirtualHeight / 2
            );

            // Initialisation du HUD.
            _hud = new Hud(_scoreFont);

            // Initialisation du rendu des formes.
            PrimitiveRenderer.Initialize(GraphicsDevice);

            base.Initialize();
        }

        // Chargement des ressources.
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameplayMusic = Content.Load<Song>(music);

            _audio = new AudioManager();

            _audio.SetMusicVolume(10f);
            _audio.PlayMusic(_gameplayMusic, true);
        }

        // Mise à jour du jeu.
        protected override void Update(GameTime gameTime)
        {
            // Actualisation des entrées.
            _input.Update(gameTime);

            // Fermeture avec Échap ou le bouton retour.
            if (
                Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            )
            {
                Exit();
            }

            // Calcul du temps écoulé.
            float deltaTime = Math.Min(
                (float)gameTime.ElapsedGameTime.TotalSeconds,
                1f / 30f
            );

            // Mise à jour selon l'écran actuel.
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

            base.Update(gameTime);
        }

        // Mise à jour du menu principal.
        private void UpdateMainMenu()
        {
            if (_input.IsJustPressed(InputAction.Exit))
            {
                Exit();
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

        // Mise à jour de la partie.
        private void UpdatePlaying(float deltaTime)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].Update(
                    deltaTime,
                    _input,
                    i,
                    this
                );
            }

            _ball.Update(
                deltaTime,
                this,
                _players
            );
        }

        // Mise à jour de la pause.
        private void UpdatePause()
        {
        }

        // Affichage du jeu.
        protected override void Draw(GameTime gameTime)
        {
            // Récupération de la résolution réelle.
            int screenWidth =
                GraphicsDevice.PresentationParameters.BackBufferWidth;

            int screenHeight =
                GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Nettoyage de l'écran.
            GraphicsDevice.Viewport = new Viewport(
                0,
                0,
                screenWidth,
                screenHeight
            );

            GraphicsDevice.Clear(backgroundColor);

            // Calcul de l'échelle pour conserver les proportions.
            float scale = Math.Min(
                screenWidth / (float)VirtualWidth,
                screenHeight / (float)VirtualHeight
            );

            // Dimensions de la zone de jeu.
            int virtualWidth = (int)(VirtualWidth * scale);
            int virtualHeight = (int)(VirtualHeight * scale);

            // Centrage de la zone de jeu.
            int offsetX = (screenWidth - virtualWidth) / 2;
            int offsetY = (screenHeight - virtualHeight) / 2;

            // Définition du Viewport.
            GraphicsDevice.Viewport = new Viewport(
                offsetX,
                offsetY,
                virtualWidth,
                virtualHeight
            );

            // Transformation graphique.
            Matrix scaleMatrix = Matrix.CreateScale(
                scale,
                scale,
                1f
            );

            _spriteBatch.Begin(
                transformMatrix: scaleMatrix
            );

            // Affichage selon l'état du jeu.
            switch (GameStateManager.CurrentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;

                case GameState.Playing:
                    DrawPlaying();
                    DrawHUD();
                    break;

                case GameState.Pause:
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Affichage du menu.
        private void DrawMenu()
        {
            Vector2 textSize = _font.MeasureString("Play");

            _mainMenu.Draw(
                _spriteBatch,
                _font,
                new Vector2(
                    (VirtualWidth - textSize.X) / 2f,
                    (VirtualHeight - textSize.Y) / 2f
                )
            );
        }

        // Affichage des joueurs et de la balle.
        private void DrawPlaying()
        {
            _players.ForEach(player =>
                player.Draw(_spriteBatch)
            );

            _ball.Draw(_spriteBatch);
        }

        // Affichage des scores.
        private void DrawHUD()
        {
            _hud.DrawMenue(
                _spriteBatch,
                scoreP1,
                scoreP2,
                this
            );
        }

        // Réinitialisation et lancement d'une partie.
        public void ResetGame()
        {
            GameStateManager.ChangeState(GameState.Playing);

            // Génération d'une direction aléatoire.
            do
            {
                direction = new Vector2(
                    rnd.Next(-500, 500),
                    rnd.Next(-500, 500)
                );
            }
            while (
                (direction.X > -250 && direction.X < 250) ||
                (direction.Y > -250 && direction.Y < 250)
            );

            // Application de la direction à la balle.
            _ball.OriginalVelocity = direction;
            _ball.Velocity = direction;
        }
    }
}
