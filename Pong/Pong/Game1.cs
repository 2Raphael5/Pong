using Class;
using Class.DebugTools;
using Class.Generic;
using Class.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pong.Class.Play;
using Simulation_etre_vivant.Class.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

// Définit les différents écrans du jeu.
public enum GameState
{
    Playing, // Partie en cours.
    Pause,   // Partie en pause.
    Menu     // Menu principal.
}

namespace Pong
{
    public class Game1 : Game
    {
        private AudioManager _audio;
        private Song _gameplayMusic;

        float player1Speed = 800f; //La vitesse du joueur 1
        float player2Speed = 800f; //La vitesse du joueur 2
        int player1Width = 10; //La largeur du joueur 1
        int player2Width = 10; //La largeur du joueur 2
        int player1Height = 200; //La hauteur du joueur 1
        int player2Height = 200; //La hauteur du joueur 2
        Color backgrounColor = Color.Black; //Couleur de font
        Color player1Color = Color.Red; //Couleur du joueur à gauche
        Color player2Color = Color.Blue; //Couleur du jouer à droite
        string ballImg = "football"; //Image du ballon
        string menuFont = "Font"; //Font du menu
        string scoreFont = "pixelart"; //Font du score
        string music = "arcade";

        #region Autre Variables

        Vector2 direction;

        // Gestion des entrées du joueur (clavier et manette).
        private InputManager _input;

        // Gestion du menu principal.
        private MainMenu _mainMenu;

        // Gestion de l'affichage des scores.
        private Hud _hud;

        // Gestion des paramètres graphiques du jeu.
        private GraphicsDeviceManager _graphics;

        // Permet de dessiner les éléments à l'écran.
        private SpriteBatch _spriteBatch;

        // Polices utilisées pour afficher les textes.
        SpriteFont _font;
        SpriteFont _scoreFont;

        // Liste contenant les deux joueurs.
        List<Player> _players;

        // Balle utilisée pendant la partie.
        Ball _ball;

        // Scores des deux joueurs.
        public int scoreP1;
        public int scoreP2;

        // Dimensions de référence du jeu.
        // Elles permettent de conserver les proportions du jeu
        // même lorsque la fenêtre est redimensionnée.
        public const int VirtualWidth = 1920;
        public const int VirtualHeight = 1080;

        // Empêche plusieurs redimensionnements simultanés de la fenêtre.
        private bool _resizing;

        // Générateur de nombres aléatoires.
        Random rnd;

        #endregion

        // Constructeur : initialise les paramètres généraux du jeu.
        public Game1()
        {
            rnd = new Random();

            // Initialise le gestionnaire graphique.
            _graphics = new GraphicsDeviceManager(this);

            // Définit le dossier contenant les ressources du jeu.
            Content.RootDirectory = "Content";

            // Autorise le redimensionnement de la fenêtre.
            Window.AllowUserResizing = true;

            // Définit la résolution de départ.
            _graphics.PreferredBackBufferWidth = VirtualWidth;
            _graphics.PreferredBackBufferHeight = VirtualHeight;

            // Détecte les changements de taille de la fenêtre.
            Window.ClientSizeChanged += (s, e) =>
            {
                // Évite de relancer un redimensionnement déjà en cours.
                if (_resizing) return;

                _resizing = true;

                // Adapte la résolution à la nouvelle taille de la fenêtre.
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

                // Applique les nouvelles dimensions.
                _graphics.ApplyChanges();

                _resizing = false;
            };
        }

        // Initialise les éléments nécessaires au fonctionnement du jeu.
        protected override void Initialize()
        {
            // Charge les polices utilisées pour les différents textes.
            _font = Content.Load<SpriteFont>($"Font/{menuFont}");
            _scoreFont = Content.Load<SpriteFont>($"Font/{scoreFont}");

            // Affiche le menu principal au lancement.
            GameStateManager.ChangeState(GameState.Menu);

            // Initialise la gestion des commandes du joueur.
            _input = new InputManager();

            // Crée le menu principal.
            _mainMenu = new MainMenu();

            // Crée les deux joueurs et définit leur position de départ.
            _players = new List<Player>
            {
                // Joueur 1 : placé à gauche de l'écran.
                new Player(
                    new PixelGeneric(
                        new Vector2(0, (VirtualHeight - player1Height) / 2),
                        player1Color
                    ),
                    player1Width,
                    player1Height,
                    Vector2.Zero,
                    player1Speed
                ),

                // Joueur 2 : placé à droite de l'écran.
                new Player(
                    new PixelGeneric(
                        new Vector2(VirtualWidth - player2Width, (VirtualHeight - player2Height) / 2),
                        player2Color
                    ),
                    player2Width,
                    player2Height,
                    Vector2.Zero,
                    player2Speed
                ),
            };

            // Charge l'image utilisée pour représenter la balle.
            Texture2D ballTexture = Content.Load<Texture2D>($"Img/{ballImg}");

            // Crée la balle avec son image et sa direction de déplacement.
            _ball = new Ball(new SpriteGeneric(ballTexture), direction);

            // Positionne la balle au centre de la résolution virtuelle.
            _ball.SpriteGeneric.Position = new Vector2(
                VirtualWidth / 2,
                VirtualHeight / 2
            );

            // Initialise l'interface des scores avec la police pixel art.
            _hud = new Hud(_scoreFont);

            // Initialise le système permettant de dessiner des formes simples.
            PrimitiveRenderer.Initialize(GraphicsDevice);

            base.Initialize();
        }

        // Charge les ressources graphiques nécessaires à l'affichage.
        protected override void LoadContent()
        {
            // Initialise le SpriteBatch, utilisé pour dessiner les images et les textes.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameplayMusic = Content.Load<Song>($"Music/{music}");

            _audio = new AudioManager();
            _audio.SetMusicVolume(10f);
            _audio.PlayMusic(_gameplayMusic, true);
        }

        // Méthode exécutée régulièrement pour mettre à jour le jeu.
        protected override void Update(GameTime gameTime)
        {
            // Actualise les entrées EN PREMIER pour éviter un frame de retard.
            _input.Update(gameTime);

            // Ferme le jeu si le joueur appuie sur Échap
            // ou sur le bouton Retour de la manette.
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            // Temps écoulé depuis la dernière mise à jour (indépendant des FPS).
            // Plafonné pour éviter un énorme saut après un freeze ou un redimensionnement.
            float deltaTime = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 30f);

            // Appelle la mise à jour correspondant à l'écran actuel.
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

        // Gère les interactions avec le menu principal.
        private void UpdateMainMenu()
        {
            // Ferme l'application si le joueur utilise la commande Exit.
            if (_input.IsJustPressed(InputAction.Exit))
            {
                Application.Exit();
            }

            // Met à jour le menu en fonction des commandes du joueur.
            _mainMenu.Update(_input);

            // Vérifie si le joueur a confirmé son choix.
            if (_mainMenu.IsConfirmed(_input))
            {
                // Si le premier élément est sélectionné, démarre une partie.
                if (_mainMenu.SelectedIndex == 0)
                {
                    ResetGame();
                }
                else
                {
                    // Sinon, quitte le jeu.
                    Exit();
                }
            }
        }

        // Gère les déplacements des joueurs et de la balle pendant la partie.
        private void UpdatePlaying(float deltaTime)
        {
            // Met à jour chaque joueur de la liste.
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].Update(deltaTime, _input, i, this);
            }

            // Met à jour la balle et gère ses interactions avec les joueurs.
            _ball.Update(deltaTime, this, _players);
        }

        // Gère les mises à jour lorsque la partie est en pause.
        private void UpdatePause()
        {

        }

        // Dessine les éléments visibles à l'écran.
        protected override void Draw(GameTime gameTime)
        {
            // Récupère la largeur et la hauteur réelles de la fenêtre.
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Remplit toute la fenêtre avec un fond noir (bandes noires si les proportions diffèrent).
            GraphicsDevice.Viewport = new Viewport(0, 0, w, h);
            GraphicsDevice.Clear(backgrounColor);

            // Échelle qui adapte le jeu à la fenêtre sans déformation.
            float scale = Math.Min(
                w / (float)VirtualWidth,
                h / (float)VirtualHeight
            );

            // Taille de la zone de jeu après mise à l'échelle.
            int vw = (int)(VirtualWidth * scale);
            int vh = (int)(VirtualHeight * scale);

            // Centre la zone de jeu dans la fenêtre.
            GraphicsDevice.Viewport = new Viewport(
                (w - vw) / 2,
                (h - vh) / 2,
                vw,
                vh
            );

            // Prépare le dessin en appliquant l'échelle calculée.
            _spriteBatch.Begin(
                transformMatrix: Matrix.CreateScale(scale, scale, 1f)
            );

            // Dessine les éléments correspondant à l'écran actuel.
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

        // Affiche le menu principal au centre de l'écran.
        private void DrawMenu()
        {
            // Récupère les dimensions du texte pour pouvoir le centrer.
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

        // Dessine les éléments présents pendant une partie.
        private void DrawPlaying()
        {
            _players.ForEach(e => e.Draw(_spriteBatch));
            _ball.Draw(_spriteBatch);
        }

        // Affiche l'interface de jeu et les scores des joueurs.
        private void DrawHUD()
        {
            _hud.DrawMenue(_spriteBatch, scoreP1, scoreP2, this);
        }

        // Lance une manche : passe en mode jeu et donne une direction aléatoire à la balle.
        public void ResetGame()
        {
            GameStateManager.ChangeState(GameState.Playing);

            // Génère une direction aléatoire, en évitant les trajectoires
            // presque purement horizontales ou verticales.
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

            _ball.OriginalVelocity = direction;
            _ball.Velocity = direction;
        }
    }
}