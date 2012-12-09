using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace VoodooBoyGame
{
    class PlayerControlSystem : EntityProcessingSystem
    {
        private ComponentMapper<WalkableBodyComponent> walkableBodyMapper;
        private ComponentMapper<AnimationPlayerComponent> animationMapper;
        private InputHelper input;

        public PlayerControlSystem()
            : base(typeof(WalkableBodyComponent), typeof(AnimationPlayerComponent), typeof(PlayerComponent))
        {
            input = Global.Input;
        }

        public override void Initialize()
        {
            walkableBodyMapper = new ComponentMapper<WalkableBodyComponent>(world);
            animationMapper = new ComponentMapper<AnimationPlayerComponent>(world);
        }

        public override void Process(Entity e)
        {
            WalkableBodyComponent body = walkableBodyMapper.Get(e);
            AnimationPlayerComponent animation = animationMapper.Get(e);

            body.Motor.MotorSpeed = 0.0f;

            if (Math.Abs(body.Wheel.LinearVelocity.X) < 0.5f)
                animation.StartAnimation("resting");

            if (input.IsHeldDown(Buttons.LeftThumbstickLeft) || input.IsHeldDown(Keys.Left))
            {
                body.Motor.MotorSpeed = -15f;

                animation.Flipped = true;

                if (animation.AnimationPlayer.CurrentAnimation != "running")
                    animation.StartAnimation("running");
            }

            if (input.IsHeldDown(Buttons.LeftThumbstickRight) || input.IsHeldDown(Keys.Right))
            {
                body.Motor.MotorSpeed = 15f;

                animation.Flipped = false;

                if (animation.AnimationPlayer.CurrentAnimation != "running")
                    animation.StartAnimation("running");
            }

            if ((input.IsPressed(Buttons.A) || input.IsPressed(Keys.Space)) && body.OnGround)
            {
                body.Wheel.ApplyLinearImpulse(ConvertUnits.ToSimUnits(new Vector2(0, -1300f)));
            }
        }
    }
}
