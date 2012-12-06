using Artemis;
using Microsoft.Xna.Framework;
using System;

namespace VoodooBoyGame
{
    public class TransformComponent : Component
    {
        private Vector2 position;
        private float rotation;
        private float scale;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public TransformComponent() { }
       
        public TransformComponent(Vector2 position)
        {
            Position = position;
        }

        public TransformComponent(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public TransformComponent(Vector2 position, float rotation, float scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public float RotationAsRadians
        {
            get { return (float)Math.PI * rotation / 180.0f; }
        }

        public float DistanceTo(TransformComponent t)
        {
            return Artemis.Utils.Distance(t.X, t.Y, X, Y);
        }
    }
}
