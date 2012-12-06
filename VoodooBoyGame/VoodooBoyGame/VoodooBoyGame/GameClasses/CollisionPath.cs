using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;

namespace VoodooBoyGame
{
    class CollisionPath
    {
        public Body body;

        public Vector2 Position
        {
            get { return body.Position; }
        }

        public CollisionPath(List<Vector2> localPoints, Vector2 position, World world)
        {
            body = new Body(world);
            body.Position = ConvertUnits.ToSimUnits(position);

            Vertices terrain = new Vertices();

            foreach (Vector2 point in localPoints)
            {
                terrain.Add(ConvertUnits.ToSimUnits(point));
            }

            for (int i = 0; i < terrain.Count - 1; ++i)
            {
                FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], body);
            }

            body.Restitution = 0f;
            body.Friction = float.MaxValue;
        }
    }
}
