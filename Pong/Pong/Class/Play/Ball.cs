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
        public float Width { get; set; }
        public float Height { get; set; }

        public Ball(SpriteGeneric spriteGeneric, float width, float height)
        {
            SpriteGeneric = spriteGeneric;
            Width = width;
            Height = height;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
