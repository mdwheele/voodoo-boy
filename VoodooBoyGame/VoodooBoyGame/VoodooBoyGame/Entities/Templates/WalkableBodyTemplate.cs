﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using Demina;

namespace VoodooBoyGame
{
    class WalkableBodyTemplate : IEntityTemplate
    {
        public Entity BuildEntity(Entity e, params object[] args)
        {
            if (args[0].GetType() == typeof(FarseerPhysics.Dynamics.World))
            {
                FarseerPhysics.Dynamics.World physics = (FarseerPhysics.Dynamics.World)args[0];

                e.Group = "PLAYER";
                e.AddComponent(new WalkableBodyComponent(physics, new Vector2(200, 200), ConvertUnits.ToSimUnits(32), ConvertUnits.ToSimUnits(96), 2.0f));

                TransformComponent transform = new TransformComponent();
                transform.TransformOffset = new Vector2(0, 30);
                e.AddComponent(transform);

                AnimationPlayerComponent animation = new AnimationPlayerComponent();
                animation.AddAnimation("walking", Global.Content.Load<Animation>("Characters/Victor/walking"));
                animation.AnimationPlayer.StartAnimation("walking");
                e.AddComponent(animation);

                return e;
            }
            else
            {
                return null;
            }
        }
    }
}