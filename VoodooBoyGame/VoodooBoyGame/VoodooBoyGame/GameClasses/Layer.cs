using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace VoodooBoyGame
{
    public class Layer
    {
        private string name;
        private Vector2 scrollSpeed;
        private List<LayerTexture> textures;
        private LevelScreen parent;
        private Matrix transform;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Vector2 ScrollSpeed
        {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        public Matrix Transform
        {
            get { return transform; }
        }

        public Layer(string name, Vector2 scrollSpeed, LevelScreen levelScreen){
            Name = name;
            ScrollSpeed = scrollSpeed;
            parent = levelScreen;
            textures = new List<LayerTexture>();
        }

        public void Add(LayerTexture texture)
        {
            textures.Add(texture);
        }

        public void Draw()
        {
            transform = Matrix.Identity *
                        Matrix.CreateTranslation(new Vector3(-Global.Camera.Position.X * ScrollSpeed.X, -Global.Camera.Position.Y * ScrollSpeed.Y, 0)) *
                        Matrix.CreateScale(Global.Camera.Zoom) *
                        Matrix.CreateRotationZ(Global.Camera.Rotation) *
                        Matrix.CreateTranslation(new Vector3(Global.Camera.Size / 2, 0));

            Global.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Transform);

            foreach (LayerTexture texture in textures)
            {
                texture.Draw();
            }

            Global.SpriteBatch.End();
        }
    }
}
