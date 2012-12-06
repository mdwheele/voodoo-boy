#region File Description
//-----------------------------------------------------------------------------
// Tile.cs
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
    public enum CollisionType
    {
        Passable = 0,
        Impassable = 1,
        Platform = 2,
        Ladder = 3,
        Breakable = 4
    }

    public class CollisionTile
    {
        public CollisionType Collision;
        public Body body;

        public Vector2 Position
        {
            get { return body.Position; }
        }

        public CollisionTile(CollisionType collision, float width, float height, Vector2 position, float rotation, World world)
        {
            Collision = collision;

            body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), 64f, ConvertUnits.ToSimUnits(Position));
            body.BodyType = BodyType.Static;
            body.Rotation = rotation;

            body.Restitution = 0.3f;
            body.Friction = 0f;
        }
    }
}