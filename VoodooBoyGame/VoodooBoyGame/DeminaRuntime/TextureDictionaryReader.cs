using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Demina
{
	public class TextureDictionaryReader : ContentTypeReader<TextureDictionary>
	{
		protected override TextureDictionary Read(ContentReader input, TextureDictionary existingInstance)
		{
			TextureDictionary output = new TextureDictionary();

			output.Version = input.ReadString();
			output.Texture = input.ReadExternalReference<Texture2D>();
			output.TextureCollection = input.ReadObject<Dictionary<string, TextureBounds>>();

			return output;
		}
	}
}
