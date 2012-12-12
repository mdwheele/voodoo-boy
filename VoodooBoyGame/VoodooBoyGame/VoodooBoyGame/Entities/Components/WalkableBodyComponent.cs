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
    public enum WalkingState
    {
        Resting,
        WalkingLeft,
        WalkingRight,
        Jumping,
        Falling
    }

    public class WalkingComponent : Component
    {
        Body body;
        Body wheel;
        FixedAngleJoint fixedAngleJoint;
        RevoluteJoint motor;
        float centerOffset;
        bool onGround;

        WalkingState prevWalkingState;
        WalkingState currWalkingState;

        public Body Body
        {
            get { return body; }
        }

        public Body Wheel
        {
            get { return wheel; }
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

        public WalkingState PrevState
        {
            get { return prevWalkingState; }
            set { prevWalkingState = value; }
        }

        public WalkingState CurrState
        {
            get { return currWalkingState; }
            set { currWalkingState = value; }
        }

        public WalkingComponent(FarseerPhysics.Dynamics.World physics, Vector2 position, float width, float height, float density)
        {
            //Create a body with almost the size of the entire object but with the bottom cut off.
            float upperBodyHeight = height - (width / 2);

            body = BodyFactory.CreateRectangle(physics, width, upperBodyHeight, density / 2);
            body.BodyType = BodyType.Dynamic;
            body.Restitution = -0.3f;
            body.Friction = 0.5f;
            body.Mass = density * upperBodyHeight * width / 2;

            //shift body up a bit to keep the new object's center correct.
            body.Position = ConvertUnits.ToSimUnits(position - (Vector2.UnitY * (height / 4)));

            //remember offset for setting transform position
            centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y);

            //makes sure the upper body always faces up
            fixedAngleJoint = JointFactory.CreateFixedAngleJoint(physics, body);

            //create wheel as wide as whole object
            wheel = BodyFactory.CreateCircle(physics, (float)width / 2, density / 2);
            wheel.BodyType = BodyType.Dynamic;
            wheel.Restitution = 0.3f;
            wheel.Friction = float.MaxValue;
            wheel.Mass = density * (float)(Math.PI * Math.Pow(width / 2, 2));

            //position wheel at bottom of body, centered
            wheel.Position = body.Position + Vector2.UnitY * upperBodyHeight / 2;

            motor = JointFactory.CreateRevoluteJoint(physics, body, wheel, Vector2.Zero);

            motor.MotorEnabled = true;
            motor.MaxMotorTorque = 1000f;
            motor.MotorSpeed = 0.0f;

            wheel.IgnoreCollisionWith(body);
            body.IgnoreCollisionWith(wheel);

            prevWalkingState = currWalkingState = WalkingState.Resting;
        }

        public void StopMoving()
        {
            if (OnGround && CurrState != WalkingState.Jumping)
            {
                motor.MotorSpeed = 0.0f;
                prevWalkingState = currWalkingState;
                currWalkingState = WalkingState.Resting;
            }
        }

        public void MoveLeft()
        {
            if (OnGround && CurrState != WalkingState.Jumping)
            {
                motor.MotorSpeed = -15.0f;
                prevWalkingState = currWalkingState;
                currWalkingState = WalkingState.WalkingLeft;
            }
        }

        public void MoveRight()
        {
            if (OnGround && CurrState != WalkingState.Jumping)
            {
                motor.MotorSpeed = 15.0f;
                prevWalkingState = currWalkingState;
                currWalkingState = WalkingState.WalkingRight;
            }
        }

        public void Jump()
        {
            if (OnGround && PrevState != WalkingState.Falling)
            {
                motor.MotorSpeed = 0.0f;
                body.ApplyLinearImpulse(new Vector2(0, -150f));

                prevWalkingState = currWalkingState;
                currWalkingState = WalkingState.Jumping;
            }
        }

        public void AddThrust()
        {

        }

        public void AddDrag()
        {
        }
    }
}
