using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Demina
{
	public class AnimationReader : ContentTypeReader<Animation>
	{
		protected override Animation Read(ContentReader input, Animation existingInstance)
		{
			Animation output = new Animation();

			output.Version = input.ReadString();
			output.FrameRate = input.ReadInt32();
			output.LoopFrame = input.ReadInt32();

			int textureEntries = input.ReadInt32();
			output.Textures = new List<TextureEntry>(textureEntries);

			for (int i = 0; i < textureEntries; i++)
			{
				TextureEntry textureEntry = new TextureEntry();
				textureEntry.UseDictionary = input.ReadBoolean();

				if (textureEntry.UseDictionary)
				{
					TextureDictionary textureDictionary = input.ReadExternalReference<TextureDictionary>();
					string name = input.ReadString();

					textureEntry.Texture = textureDictionary.Texture;
					textureEntry.TextureBounds = textureDictionary.TextureCollection[name];
				}
				else
				{
					textureEntry.Texture = input.ReadExternalReference<Texture2D>();
					textureEntry.TextureBounds = new TextureBounds(
						new Rectangle(0, 0, textureEntry.Texture.Width, textureEntry.Texture.Height),
						new Vector2(textureEntry.Texture.Width / 2, textureEntry.Texture.Height / 2));
				}

				output.Textures.Add(textureEntry);
			}

			output.Keyframes = input.ReadObject<List<Keyframe>>();

			float frameRate = 1.0f / output.FrameRate;
			output.LoopTime = output.LoopFrame * frameRate;
			output.Loop = output.LoopFrame != -1;

			foreach (Keyframe keyframe in output.Keyframes)
			{
				for (int i = 0; i < keyframe.Bones.Count; i++)
				{
					keyframe.Bones[i].SelfIndex = i;
				}

				keyframe.FrameTime = frameRate * keyframe.FrameNumber;
				keyframe.SortBones();
			}

			return output;
		}
	}
}
