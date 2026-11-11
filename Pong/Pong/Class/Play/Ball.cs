using Class.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Pong.Class.Play
{
    public class Ball
    {
        // Échelle utilisée pour la hitbox de la balle.
        // IMPORTANT : doit correspondre à la taille réellement dessinée par SpriteGeneric.Draw.
        public float BallScale;

        // Multiplicateur de vitesse à chaque rebond.
        private const float SpeedBoost = 1.05f;

        // Vitesse maximale de la balle (px/s).
        private const float MaxSpeed = 2000f;

        // Distance maximale parcourue par la balle en un seul pas de simulation (px).
        // Doit rester inférieure à la largeur d'une raquette pour éviter le tunneling.
        private const float MaxStep = 4f;

        // Part de la vitesse de la raquette transmise à la balle (0 = aucun effet, 1 = totalité).
        private const float PaddleInfluence = 0.5f;

        // Pente maximale de la balle après un rebond (|Vy| / |Vx|). 2 ≈ 63° max.
        private const float MaxSlope = 2f;

        // Mémorisation de la position et de la vitesse verticale de chaque raquette.
        private readonly Dictionary<Player, float> _lastPaddleY = new Dictionary<Player, float>();
        private readonly Dictionary<Player, float> _paddleVelY = new Dictionary<Player, float>();

        public SpriteGeneric SpriteGeneric { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 OriginalVelocity { get; set; }

        public Ball(SpriteGeneric spriteGeneric, Vector2 velocity, float scale)
        {
            SpriteGeneric = spriteGeneric;
            OriginalVelocity = velocity;
            Velocity = velocity;
            BallScale = scale;
        }

        public void Update(float deltaTime, Game1 game, List<Player> players)
        {
            int screenH = Game1.VirtualHeight;
            int screenW = Game1.VirtualWidth;

            float ballW = SpriteGeneric.Width * BallScale;
            float ballH = SpriteGeneric.Height * BallScale;

            // Mesure la vitesse verticale de chaque raquette sur cette frame.
            UpdatePaddleSpeeds(players, deltaTime);

            // Découpe le déplacement en petits pas pour ne jamais "sauter" une raquette.
            int steps = Math.Max(1, (int)Math.Ceiling(Velocity.Length() * deltaTime / MaxStep));
            float subDt = deltaTime / steps;

            for (int i = 0; i < steps; i++)
            {
                SpriteGeneric.Position += Velocity * subDt;

                HandleWalls(screenH, ballH);
                HandlePlayers(players, screenW, ballW, ballH);

                // Point marqué : on arrête la simulation de cette frame.
                if (SpriteGeneric.Position.X < 0)
                {
                    game.scoreP2++;
                    ResetBall(screenW, screenH);
                    game.ResetGame();
                    return;
                }
                else if (SpriteGeneric.Position.X > screenW)
                {
                    game.scoreP1++;
                    ResetBall(screenW, screenH);
                    game.ResetGame();
                    return;
                }
            }
        }

        // Calcule la vitesse verticale (px/s) de chaque raquette sur cette frame.
        private void UpdatePaddleSpeeds(List<Player> players, float deltaTime)
        {
            if (deltaTime <= 0f)
                return;

            foreach (Player player in players)
            {
                float y = player.PixelGeneric.Position.Y;

                if (_lastPaddleY.TryGetValue(player, out float lastY))
                    _paddleVelY[player] = (y - lastY) / deltaTime;
                else
                    _paddleVelY[player] = 0f;

                _lastPaddleY[player] = y;
            }
        }

        // Rebonds haut / bas (direction forcée pour éviter de rester coincée).
        private void HandleWalls(int screenH, float ballH)
        {
            if (SpriteGeneric.Position.Y < 0)
            {
                SpriteGeneric.Position = new Vector2(SpriteGeneric.Position.X, 0);
                Velocity = new Vector2(Velocity.X, Math.Abs(Velocity.Y));
                ApplyBoost();
            }
            else if (SpriteGeneric.Position.Y > screenH - ballH)
            {
                SpriteGeneric.Position = new Vector2(SpriteGeneric.Position.X, screenH - ballH);
                Velocity = new Vector2(Velocity.X, -Math.Abs(Velocity.Y));
                ApplyBoost();
            }
        }

        // Rebonds sur les raquettes.
        private void HandlePlayers(List<Player> players, int screenW, float ballW, float ballH)
        {
            Rectangle ballRect = new Rectangle(
                (int)SpriteGeneric.Position.X,
                (int)SpriteGeneric.Position.Y,
                (int)ballW,
                (int)ballH
            );

            foreach (Player player in players)
            {
                Rectangle playerRect = new Rectangle(
                    (int)player.PixelGeneric.Position.X,
                    (int)player.PixelGeneric.Position.Y,
                    player.PixelGeneric.Width,
                    player.PixelGeneric.Height
                );

                if (!ballRect.Intersects(playerRect))
                    continue;

                bool leftPaddle = player.PixelGeneric.Position.X < screenW / 2f;

                _paddleVelY.TryGetValue(player, out float paddleVy);

                // On ne rebondit que si la balle va vers la raquette :
                // évite les rebonds multiples et les accélérations répétées.
                if (leftPaddle && Velocity.X < 0)
                {
                    Velocity = Deflect(+1f, paddleVy);
                    SpriteGeneric.Position = new Vector2(playerRect.Right, SpriteGeneric.Position.Y);
                    ApplyBoost();
                    break;
                }
                else if (!leftPaddle && Velocity.X > 0)
                {
                    Velocity = Deflect(-1f, paddleVy);
                    SpriteGeneric.Position = new Vector2(playerRect.Left - ballW, SpriteGeneric.Position.Y);
                    ApplyBoost();
                    break;
                }
            }
        }

        // Renvoie la balle vers dirX (+1 ou -1) en la déviant selon le mouvement de la raquette.
        private Vector2 Deflect(float dirX, float paddleVelY)
        {
            float speed = Velocity.Length();

            float vx = Math.Abs(Velocity.X);
            float vy = Velocity.Y + paddleVelY * PaddleInfluence;

            // Empêche un angle trop vertical (la balle resterait presque coincée en haut/bas).
            float maxVy = vx * MaxSlope;
            vy = MathHelper.Clamp(vy, -maxVy, maxVy);

            // On garde la même vitesse globale : seul l'angle change.
            Vector2 result = new Vector2(dirX * vx, vy);
            return Vector2.Normalize(result) * speed;
        }

        // Accélère la balle en respectant la vitesse maximale.
        private void ApplyBoost()
        {
            Vector2 v = Velocity * SpeedBoost;
            if (v.Length() > MaxSpeed)
                v = Vector2.Normalize(v) * MaxSpeed;
            Velocity = v;
        }

        // Replace simplement la balle au centre.
        // La nouvelle vitesse est définie par Game1.ResetGame().
        private void ResetBall(int screenW, int screenH)
        {
            SpriteGeneric.Position = new Vector2(screenW / 2f, screenH / 2f);
            Velocity = OriginalVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteGeneric.Draw(spriteBatch, BallScale);
        }
    }
}