using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace VoodooBoyGame
{
    class GibTemplate : IEntityTemplate
    {
        public Entity BuildEntity(Entity e, params object[] args)
        {
            if (args[0].GetType() == typeof(FarseerPhysics.Dynamics.World))
            {
                e.AddComponent(new TransformComponent());
                e.AddComponent(new BodyComponent());
                e.AddComponent(new TextureComponent("Textures/gib"));
                e.GetComponent<BodyComponent>().Body = BodyFactory.CreateCircle((FarseerPhysics.Dynamics.World)args[0], ConvertUnits.ToSimUnits(4), 2.0f);
                e.GetComponent<BodyComponent>().Body.BodyType = BodyType.Dynamic;
                return e;
            }
            else
            {
                return null;
            }
        }
    }
}
