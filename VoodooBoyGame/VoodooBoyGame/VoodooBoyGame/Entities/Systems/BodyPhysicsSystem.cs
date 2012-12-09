using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics;

namespace VoodooBoyGame
{
    class BodyPhysicsSystem : EntityProcessingSystem
    {
        private FarseerPhysics.Dynamics.World PhysicsWorld;
        private ComponentMapper<BodyComponent> dynamicBodyMapper;
        private ComponentMapper<TransformComponent> transformMapper;

        public BodyPhysicsSystem(FarseerPhysics.Dynamics.World physicsWorld)
            : base(typeof(BodyComponent), typeof(TransformComponent))
        {
            PhysicsWorld = physicsWorld;
        }

        public override void Initialize()
        {
            dynamicBodyMapper = new ComponentMapper<BodyComponent>(world);
            transformMapper = new ComponentMapper<TransformComponent>(world);
        }

        public override void Process(Entity e)
        {
            transformMapper.Get(e).Position = ConvertUnits.ToDisplayUnits(dynamicBodyMapper.Get(e).Position);
            transformMapper.Get(e).Rotation = dynamicBodyMapper.Get(e).Body.Rotation;
            
            Vector2 position = transformMapper.Get(e).Position;

            if (position.Y > 900)
            {
                e.Delete();
            }
        }

        public override void OnRemoved(Entity e)
        {
            PhysicsWorld.RemoveBody(dynamicBodyMapper.Get(e).Body);
        }
    }
}
