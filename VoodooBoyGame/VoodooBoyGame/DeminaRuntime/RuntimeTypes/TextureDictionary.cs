using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Demina
{
	public class TextureDictionary
	{
		public string Version { get; set; }

		public Texture2D Texture { get; set; }

		public Dictionary<string, TextureBounds> TextureCollection { get; set; }
	}
}
