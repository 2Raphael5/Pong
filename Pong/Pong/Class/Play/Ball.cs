using Class.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pong.Class.Play
{
    public class Ball
    {
        public SpriteGeneric SpriteGeneric { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 OriginalVelocity { get; set; }
        public Ball(SpriteGeneric spriteGeneric, Vector2 velocity)
        {
            SpriteGeneric = spriteGeneric;
            OriginalVelocity = velocity;
            Velocity = velocity;
        }

        public void Update(float deltaTime, Game1 game, List<Player> _players)
        {
            SpriteGeneric.Position += Velocity * deltaTime;

            int screenH = Game1.VirtualHeight;
            int screenW = Game1.VirtualWidth;

            float ballW = SpriteGeneric.Width / 10f;
            float ballH = SpriteGeneric.Height / 10f;

            // Rebonds haut / bas (en forçant la direction pour éviter de rester coincée)
            if (SpriteGeneric.Position.Y < 0)
            {
                SpriteGeneric.Position = new Vector2(SpriteGeneric.Position.X, 0);
                Velocity = new Vector2(Velocity.X, Math.Abs(Velocity.Y));
                Velocity *= 1.05f;
            }
            else if (SpriteGeneric.Position.Y > screenH - ballH)
            {
                SpriteGeneric.Position = new Vector2(SpriteGeneric.Position.X, screenH - ballH);
                Velocity = new Vector2(Velocity.X, -Math.Abs(Velocity.Y));
                Velocity *= 1.05f;
            }

            Rectangle ballRect = new Rectangle((int)SpriteGeneric.Position.X, (int)SpriteGeneric.Position.Y, (int)ballW, (int)ballH);
            foreach (Player player in _players)
            {
                Rectangle playerRect = new Rectangle((int)player.PixelGeneric.Position.X, (int)player.PixelGeneric.Position.Y,
                                                     player.PixelGeneric.Width, player.PixelGeneric.Height);

                if (ballRect.Intersects(playerRect))
                {
                    // On renvoie toujours vers le centre du terrain
                    bool leftSide = SpriteGeneric.Position.X < screenW / 2;
                    Velocity = new Vector2(leftSide ? Math.Abs(Velocity.X) : -Math.Abs(Velocity.X), Velocity.Y);
                    Velocity *= 1.05f;
                    break;
                }
            }

            if (SpriteGeneric.Position.X < 0)
            {
                game.scoreP2++;
                ResetBall(screenW, screenH);
                game.ResetGame();
            }
            else if (SpriteGeneric.Position.X > screenW)
            {
                game.scoreP1++;
                ResetBall(screenW, screenH);
                game.ResetGame();
            }
        }

        private void ResetBall(int screenW, int screenH)
        {
            Random rnd = new Random();
            Velocity = new Vector2(rnd.Next(-500, 500), rnd.Next(-500, 500));
            do
            {
                Velocity = new Vector2(rnd.Next(-500, 500), rnd.Next(-500, 500));
            }
            while ((Velocity.X > -250 && Velocity.X < 250) || (Velocity.Y > -250 && Velocity.Y < 250));
            SpriteGeneric.Position = new Vector2(screenW / 2, screenH / 2);
            Velocity = OriginalVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteGeneric.Draw(spriteBatch);
        }
    }
}
