#region File Description
//-----------------------------------------------------------------------------
// MyTexture.cs
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace VoodooBoyGame
{
    public class LayerTexture
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 origin;
        private float rotation;
        private Vector2 scale;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
        }

        private float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public LayerTexture(Texture2D tex, Vector2 pos, float rot, Vector2 sca)
        {
            Texture = tex;
            Rotation = rot;
            Scale = sca;
            Position = pos;

            this.origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
        }

        public void Draw()
        {
            Global.SpriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
