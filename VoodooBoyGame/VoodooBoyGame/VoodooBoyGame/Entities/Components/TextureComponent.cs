using Microsoft.Xna.Framework;
using Artemis;
using Microsoft.Xna.Framework.Graphics;

namespace VoodooBoyGame
{
    class TextureComponent : Component
    {
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(texture.Width / 2, texture.Height / 2); }
        }

        public TextureComponent() { }

        public TextureComponent(Texture2D texture)
        {
            Texture = texture;
        }

        public TextureComponent(string fileName)
        {
            Texture = Global.Content.Load<Texture2D>(fileName);
        }
    }
}
