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
        private ComponentMapper<AnimationComponent> animationMapper;

        public AnimationUpdateSystem()
            : base(typeof(AnimationComponent))
        {
        }

        public override void Initialize()
        {
            animationMapper = new ComponentMapper<AnimationComponent>(world);
        }

        public override void Process(Entity e)
        {
            animationMapper.Get(e).AnimationPlayer.Update((float)world.Delta/1000.0f);
        }
    }
}
