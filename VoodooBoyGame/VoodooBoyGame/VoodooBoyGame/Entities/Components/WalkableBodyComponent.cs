using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Artemis;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace VoodooBoyGame
{
    public class WalkableBodyComponent : Component
    {
        Body body;
        Body wheel;
        FixedAngleJoint fixedAngleJoint;
        RevoluteJoint motor;
        float centerOffset;

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

/*
    FarseerPhysics.Dynamics.World physics = (FarseerPhysics.Dynamics.World)args[0];

    Vector2 textureSize = new Vector2(e.GetComponent<TextureComponent>().Texture.Width, e.GetComponent<TextureComponent>().Texture.Height);

    BodyComponent playerUpperBody = new BodyComponent();                
    BodyComponent playerFeet = new BodyComponent();

    float upperBodyHeight = textureSize.Y - (textureSize.X / 2);

    playerUpperBody.Body = BodyFactory.CreateRectangle(physics, ConvertUnits.ToSimUnits(textureSize.X), ConvertUnits.ToSimUnits(textureSize.Y), 1.0f);
    playerUpperBody.Body.BodyType = BodyType.Dynamic;
    playerUpperBody.Body.Restitution = 0.0f;
    playerUpperBody.Body.Friction = 0.5f;
    playerUpperBody.Position = ConvertUnits.ToSimUnits(-Vector2.UnitY * (textureSize.X / 4));
    JointFactory.CreateFixedAngleJoint(physics, playerUpperBody.Body);

    playerFeet.Body = BodyFactory.CreateCircle(physics, ConvertUnits.ToSimUnits(textureSize.X / 2), 1.0f);
    playerFeet.Body.Position = playerUpperBody.Position + ConvertUnits.ToSimUnits(Vector2.UnitY * (upperBodyHeight / 2));
    playerFeet.Body.BodyType = BodyType.Dynamic;
    playerFeet.Restitution = 0.0f;
    playerFeet.Friction = float.MaxValue;

    RevoluteJoint motor = JointFactory.CreateRevoluteJoint(physics, playerUpperBody.Body, playerFeet.Body, Vector2.Zero);

    motor.MotorEnabled = true;
    motor.MaxMotorTorque = 10000f;
    motor.MotorSpeed = 500f;

    playerUpperBody.Body.IgnoreCollisionWith(playerFeet.Body);
    playerFeet.Body.IgnoreCollisionWith(playerUpperBody.Body);

    e.AddComponent(playerFeet);
    e.AddComponent(playerUpperBody); 

    Global.Camera.TrackingBody = playerUpperBody.Body;
*/