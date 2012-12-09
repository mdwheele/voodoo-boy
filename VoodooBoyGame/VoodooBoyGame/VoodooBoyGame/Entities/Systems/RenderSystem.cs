using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoodooBoyGame
{
    class RenderSystem : EntityProcessingSystem
    {
        private ComponentMapper<TextureComponent> renderTextureMapper;
        private ComponentMapper<TransformComponent> transformMapper;
        private ComponentMapper<ExpiresComponent> expiresMapper;

        public RenderSystem()
            : base(typeof(TransformComponent), typeof(TextureComponent))
        {
        }

        public override void Initialize()
        {
            renderTextureMapper = new ComponentMapper<TextureComponent>(world);
            transformMapper = new ComponentMapper<TransformComponent>(world);
        }

        public override void Process(Entity e)
        {
            Texture2D texture = renderTextureMapper.Get(e).Texture;
            Vector2 position = transformMapper.Get(e).Position;

            Global.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Global.Camera.CameraMatrix);
            Global.SpriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height), null, Color.White, transformMapper.Get(e).Rotation, renderTextureMapper.Get(e).Origin, SpriteEffects.None, 0);
            Global.SpriteBatch.End();
        }
    }
}
