using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;

namespace VoodooBoyGame
{
    class WalkableBodySystem : EntityProcessingSystem
    {
        private FarseerPhysics.Dynamics.World PhysicsWorld;
        private ComponentMapper<WalkableBodyComponent> walkableBodyMapper;
        private InputHelper input;

        public WalkableBodySystem(FarseerPhysics.Dynamics.World physicsWorld)
            : base(typeof(WalkableBodyComponent))
        {
            PhysicsWorld = physicsWorld;
            input = Global.Input;
        }

        public override void Initialize()
        {
            walkableBodyMapper = new ComponentMapper<WalkableBodyComponent>(world);
        }

        public override void Process(Entity e)
        {
            walkableBodyMapper.Get(e).Motor.MotorSpeed = input.CurrentGamepadState.ThumbSticks.Left.X * 20.0f;
            walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, 50f));

            if (input.IsPressed(Buttons.A))
            {
                walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, -1700f));
            }
        }

        public override void OnRemoved(Entity e)
        {
        }
    }
}
