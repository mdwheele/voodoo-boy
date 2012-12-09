using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Artemis;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.DebugViews;
using FarseerPhysics;

namespace VoodooBoyGame
{
    public class WalkableBodyComponent : Component
    {
        Body body;
        Body wheel;
        FixedAngleJoint fixedAngleJoint;
        RevoluteJoint motor;
        float centerOffset;
        bool onGround;

        public Body Body
        {
            get { return body; }
        }

        public Body Wheel
        {
            get { return wheel; }
        }

        public RevoluteJoint Motor
        {
            get { return motor; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(ConvertUnits.ToDisplayUnits(body.Position.X), centerOffset); }
        }

        public bool OnGround
        {
            get { return onGround; }
            set { onGround = value; }
        }

        public WalkableBodyComponent(FarseerPhysics.Dynamics.World physics, Vector2 position, float width, float height, float mass)
        {
            //Create a body with almost the size of the entire object but with the bottom cut off.
            float upperBodyHeight = height - (width / 2);

            body = BodyFactory.CreateRectangle(physics, width, upperBodyHeight, mass / 2);
            body.BodyType = BodyType.Dynamic;
            body.Restitution = -0.3f;
            body.Friction = 0.5f;

            //shift body up a bit to keep the new object's center correct.
            body.Position = ConvertUnits.ToSimUnits(position - (Vector2.UnitY * (height / 4)));

            //remember offset for setting transform position
            centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y);

            //makes sure the upper body always faces up
            fixedAngleJoint = JointFactory.CreateFixedAngleJoint(physics, body);

            //create wheel as wide as whole object
            wheel = BodyFactory.CreateCircle(physics, (float)width / 2, mass / 2);
            wheel.BodyType = BodyType.Dynamic;
            wheel.Restitution = -0.3f;
            wheel.Friction = float.MaxValue;

            //position wheel at bottom of body, centered
            wheel.Position = body.Position + Vector2.UnitY * upperBodyHeight / 2;

            motor = JointFactory.CreateRevoluteJoint(physics, body, wheel, Vector2.Zero);

            motor.MotorEnabled = true;
            motor.MaxMotorTorque = 1000f;
            motor.MotorSpeed = 0.0f;

            wheel.IgnoreCollisionWith(body);
            body.IgnoreCollisionWith(wheel);                        
        }
    }
}
