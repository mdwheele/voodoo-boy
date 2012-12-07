using Microsoft.Xna.Framework;

namespace Demina
{
	public class Bone
	{
		public string Name { get; set; }
		public bool Hidden { get; set; }
		public int ParentIndex { get; set; }
		public int TextureIndex { get; set; }
		[Microsoft.Xna.Framework.Content.ContentSerializerIgnore]
		public int SelfIndex { get; set; }
		[Microsoft.Xna.Framework.Content.ContentSerializerIgnore]
		public int UpdateIndex { get; set; }
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
		public Vector2 Scale { get; set; }
		public bool TextureFlipHorizontal { get; set; }
		public bool TextureFlipVertical { get; set; }
	}
}
