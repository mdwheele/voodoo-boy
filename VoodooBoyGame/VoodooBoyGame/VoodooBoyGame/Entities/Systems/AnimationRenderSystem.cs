﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoodooBoyGame
{
    class AnimationRenderSystem : EntityProcessingSystem
    {
        private ComponentMapper<AnimationComponent> animationMapper;
        private ComponentMapper<TransformComponent> transformMapper;

        public AnimationRenderSystem()
            : base(typeof(AnimationComponent), typeof(TransformComponent))
        {
        }

        public override void Initialize()
        {
            animationMapper = new ComponentMapper<AnimationComponent>(world);
            transformMapper = new ComponentMapper<TransformComponent>(world);
        }

        public override void Process(Entity e)
        {
            animationMapper.Get(e).AnimationPlayer.Draw(Global.SpriteBatch, transformMapper.Get(e).Position, animationMapper.Get(e).Flipped, false, 0.0f, Color.White, new Vector2(1.0f, 1.0f), Global.Camera.CameraMatrix);
        }
    }
}
