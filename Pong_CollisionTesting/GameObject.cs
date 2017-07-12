using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong_CollisionTesting
{
    class GameObject
    {
        Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;

        public Rectangle BoundBox { get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); } }

        public GameObject(Texture2D texture, Vector2 position) {
            this.texture = texture;
            this.position = position;
        }

        public GameObject(Texture2D texture, Vector2 position, Vector2 velocity) {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, position, Color.White);
        }

    }
}
