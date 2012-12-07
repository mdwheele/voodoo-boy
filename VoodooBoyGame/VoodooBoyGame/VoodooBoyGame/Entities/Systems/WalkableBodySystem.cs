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
        private ComponentMapper<TransformComponent> transformMapper;
        private InputHelper input;

        public WalkableBodySystem(FarseerPhysics.Dynamics.World physicsWorld)
            : base(typeof(WalkableBodyComponent), typeof(TransformComponent))
        {
            PhysicsWorld = physicsWorld;
            input = Global.Input;
        }

        public override void Initialize()
        {
            walkableBodyMapper = new ComponentMapper<WalkableBodyComponent>(world);
            transformMapper = new ComponentMapper<TransformComponent>(world);
        }

        public override void Process(Entity e)
        {
            transformMapper.Get(e).Position = ConvertUnits.ToDisplayUnits(walkableBodyMapper.Get(e).Body.Position)+transformMapper.Get(e).TransformOffset;
            
            walkableBodyMapper.Get(e).Motor.MotorSpeed = input.CurrentGamepadState.ThumbSticks.Left.X * 20.0f;
            walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, 25f));

            if (input.IsPressed(Buttons.A))
            {
                walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, -700f));
            }

            if (transformMapper.Get(e).Position.Y > 900)
            {
                PhysicsWorld.RemoveBody(walkableBodyMapper.Get(e).Body);
                PhysicsWorld.RemoveBody(walkableBodyMapper.Get(e).Wheel);
                e.Delete();
            }
        }

        public override void OnRemoved(Entity e)
        {
        }
    }
}
