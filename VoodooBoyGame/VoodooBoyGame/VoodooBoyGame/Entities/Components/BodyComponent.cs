using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Artemis;

namespace VoodooBoyGame
{
    public class BodyComponent : Component
    {
        private Body body;

        public Body Body
        {
            get { return body; }
            set { body = value; }
        }

        public Vector2 Position
        {
            get { return body.Position; }
            set { body.Position = value; }
        }

        public float Restitution
        {
            get { return body.Restitution; }
            set { body.Restitution = value; }
        }

        public float Friction
        {
            get { return body.Friction; }
            set { body.Friction = value; }
        }

        public float Mass
        {
            get { return body.Mass; }
            set { body.Mass = value; }
        }
        
        public BodyComponent() { }

        public BodyComponent(Vector2 position)
        {
            Position = position;
        }

        public BodyComponent(Vector2 position, float restitution, float friction)
        {
            Position = position;
            Restitution = restitution;
            Friction = friction;
        }

        public BodyComponent(Vector2 position, float restitution, float friction, float mass)
        {
            Position = position;
            Restitution = restitution;
            Friction = friction;
            Mass = mass;
        }
    }
}
