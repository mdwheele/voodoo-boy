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
    class WalkableBodyPhysicsSystem : EntityProcessingSystem
    {
        private FarseerPhysics.Dynamics.World PhysicsWorld;
        private ComponentMapper<WalkableBodyComponent> walkableBodyMapper;
        private ComponentMapper<TransformComponent> transformMapper;

        public WalkableBodyPhysicsSystem(FarseerPhysics.Dynamics.World physicsWorld)
            : base(typeof(WalkableBodyComponent), typeof(TransformComponent))
        {
            PhysicsWorld = physicsWorld;
        }

        public override void Initialize()
        {
            walkableBodyMapper = new ComponentMapper<WalkableBodyComponent>(world);
            transformMapper = new ComponentMapper<TransformComponent>(world);
        }

        public override void Process(Entity e)
        {
            transformMapper.Get(e).Position = ConvertUnits.ToDisplayUnits(walkableBodyMapper.Get(e).Body.Position)+transformMapper.Get(e).TransformOffset;

            walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, 25f));
            
            //determine if body is in the air
            Vector2 bottomOfCircle = walkableBodyMapper.Get(e).Wheel.Position + (Vector2.UnitY * walkableBodyMapper.Get(e).Wheel.FixtureList[0].Shape.Radius);

            walkableBodyMapper.Get(e).OnGround = false;
            PhysicsWorld.RayCast((fixture, point, normal, fraction) =>
            {
                if (!fixture.Body.ToString().Equals(""))
                {
                    walkableBodyMapper.Get(e).OnGround = true;
                }
                return 0;
            }, bottomOfCircle, bottomOfCircle + Vector2.UnitY * ConvertUnits.ToSimUnits(20));
            
            //Magnetism (reduces bouncing)
            if (walkableBodyMapper.Get(e).OnGround)
            {
                //walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, 25f));
            }
            else
            {
                //Magic Traction (control movements in air
                //walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(input.CurrentGamepadState.ThumbSticks.Left.X * 20.0f, 0));               
            }
        }

        public override void OnRemoved(Entity e)
        {
        }
    }
}
