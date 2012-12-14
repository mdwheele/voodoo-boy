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

    public enum MultipleJumpAllowance
    {
        Unlimited = -1,
        None,
        Double,
        Triple,
        Quadruple
    }

    public class WalkingComponent : Component
    {
        Body body;
        Body wheel;
        FixedAngleJoint fixedAngleJoint;
        RevoluteJoint motor;
        float centerOffset;
        bool onGround;

        float speed;
        float maxMotorSpeed;

        bool canJump;
        MultipleJumpAllowance multiJumpAllowance;
        int multiJumpCounter;

        WalkingState prevWalkingState;
        WalkingState currWalkingState;

        public Vector2 Position
        {
            get { return (body.Position + wheel.Position) / 2; }
        }

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

        public float Mass
        {
            get { return wheel.Mass + body.Mass; }
        }

        public float Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                maxMotorSpeed = 0.5f / wheel.FixtureList[0].Shape.Radius * speed;
            }
        }

        public bool CanJump
        {
            get { return canJump; }
            set { canJump = value; }
        }

        public MultipleJumpAllowance MultipleJumpAllowance
        {
            get { return multiJumpAllowance; }
            set { multiJumpAllowance = value; }
        }

        public int MultipleJumpCounter
        {
            get { return multiJumpCounter; }
            set { multiJumpCounter = value; }
        }

        public WalkingComponent(FarseerPhysics.Dynamics.World physics, Vector2 position, float width, float height, float density)
        {
            //Create a body with almost the size of the entire object but with the bottom cut off.
            float upperBodyHeight = height - (width / 2);

            body = BodyFactory.CreateRectangle(physics, width, upperBodyHeight, density / 2);
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 0.25f;
            body.Friction = 0.45f;
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
            wheel.Restitution = 0.25f;
            wheel.Friction = 0.9f;
            wheel.Mass = density * (float)(Math.PI * Math.Pow(width / 2, 2));

            //position wheel at bottom of body, centered
            wheel.Position = body.Position + Vector2.UnitY * upperBodyHeight / 2;

            motor = JointFactory.CreateRevoluteJoint(physics, body, wheel, Vector2.Zero);

            Speed = 15;

            motor.MotorEnabled = true;
            motor.MaxMotorTorque = float.MaxValue;
            motor.MotorSpeed = 0.0f;

            wheel.IgnoreCollisionWith(body);
            body.IgnoreCollisionWith(wheel);

            //set state
            prevWalkingState = currWalkingState = WalkingState.Resting;

            /**
             * CAN JUMP?
             */
            CanJump = true;

            /** 
             * MULTIPLE JUMP ALLOWANCE
             */
            MultipleJumpAllowance = MultipleJumpAllowance.None;
            MultipleJumpCounter = 0;
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
                motor.MotorSpeed = -maxMotorSpeed;
                prevWalkingState = currWalkingState;
                currWalkingState = WalkingState.WalkingLeft;
            }
            else if (!OnGround && CurrState == WalkingState.Falling)
            {
                Vector2 force = -Vector2.UnitX * body.Mass * Math.Abs(body.LinearVelocity.X);
                body.ApplyForce(force);
            }
        }

        public void MoveRight()
        {
            if (OnGround && CurrState != WalkingState.Jumping)
            {
                motor.MotorSpeed = maxMotorSpeed;
                prevWalkingState = currWalkingState;
                currWalkingState = WalkingState.WalkingRight;
            }
            else if (!OnGround && CurrState == WalkingState.Falling)
            {
                Vector2 force = Vector2.UnitX * body.Mass * Math.Abs(body.LinearVelocity.X);
                body.ApplyForce(force);
            }
        }

        public void Jump()
        {
            if (CanJump)
            {
                // If we're on the ground, jump
                if (OnGround && PrevState != WalkingState.Falling)
                {
                    motor.MotorSpeed = 0.0f;
                    wheel.ApplyLinearImpulse(new Vector2(0, -(Mass * 9.8f) + 0f));

                    prevWalkingState = currWalkingState;
                    currWalkingState = WalkingState.Jumping;
                }
                // If we're still in the air and we haven't started falling, test to see if we can jump again (up to max allowance).
                else if (!OnGround && CurrState == WalkingState.Jumping)
                {
                    if (MultipleJumpCounter++ < (int)MultipleJumpAllowance || MultipleJumpAllowance == MultipleJumpAllowance.Unlimited)
                    {
                        wheel.ApplyLinearImpulse(new Vector2(0, -(Mass * 9.8f) * 0.25f + 0f));
                    }
                }
            }
        }
    }
}
