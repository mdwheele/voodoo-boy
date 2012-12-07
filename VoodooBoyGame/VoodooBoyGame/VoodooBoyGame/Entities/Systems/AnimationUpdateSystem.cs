using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoodooBoyGame
{
    class AnimationUpdateSystem : EntityProcessingSystem
    {
        private ComponentMapper<AnimationPlayerComponent> animationMapper;
        private ComponentMapper<TransformComponent> transformMapper;

        public AnimationUpdateSystem()
            : base(typeof(AnimationPlayerComponent), typeof(TransformComponent))
        {
        }

        public override void Initialize()
        {
            animationMapper = new ComponentMapper<AnimationPlayerComponent>(world);
        }

        public override void Process(Entity e)
        {
            animationMapper.Get(e).AnimationPlayer.Update((float)world.Delta/1000.0f);
        }
    }
}
