using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using Demina;
using Microsoft.Xna.Framework.Input;

namespace VoodooBoyGame
{
    class Player : IEntityTemplate
    {
        public Entity BuildEntity(Entity e, params object[] args)
        {
            if (args[0].GetType() == typeof(FarseerPhysics.Dynamics.World))
            {
                FarseerPhysics.Dynamics.World physics = (FarseerPhysics.Dynamics.World)args[0];

                e.Group = "PLAYER";
                e.AddComponent(new WalkableBodyComponent(physics, new Vector2(400, 300), ConvertUnits.ToSimUnits(64), ConvertUnits.ToSimUnits(118), 2.0f));

                TransformComponent transform = new TransformComponent();
                transform.TransformOffset = new Vector2(0, 40);
                e.AddComponent(transform);

                AnimationPlayerComponent animation = new AnimationPlayerComponent();
                animation.AddAnimation("resting", Global.Content.Load<Animation>("Characters/Victor/resting"));
                animation.AddAnimation("running", Global.Content.Load<Animation>("Characters/Victor/running"));
                animation.AnimationPlayer.StartAnimation("resting");
                e.AddComponent(animation);

                PlayerComponent player = new PlayerComponent();

                e.AddComponent(player);

                return e;
            }
            else
            {
                return null;
            }
        }
    }
}
