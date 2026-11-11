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

                // On ne rebondit que si la balle va vers la raquette :
                // évite les rebonds multiples et les accélérations répétées.
                if (leftPaddle && Velocity.X < 0)
                {
                    Velocity = new Vector2(Math.Abs(Velocity.X), Velocity.Y);
                    SpriteGeneric.Position = new Vector2(playerRect.Right, SpriteGeneric.Position.Y);
                    ApplyBoost();
                    break;
                }
                else if (!leftPaddle && Velocity.X > 0)
                {
                    Velocity = new Vector2(-Math.Abs(Velocity.X), Velocity.Y);
                    SpriteGeneric.Position = new Vector2(playerRect.Left - ballW, SpriteGeneric.Position.Y);
                    ApplyBoost();
                    break;
                }
            }
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