using System.Collections.Generic;

namespace Demina
{
	public class Keyframe
	{
		public int FrameNumber { get; set; }
		public List<Bone> Bones { get; set; }
		public string Trigger { get; set; }
		public bool FlipVertically { get; set; }
		public bool FlipHorizontally { get; set; }

		[Microsoft.Xna.Framework.Content.ContentSerializerIgnore]
		public float FrameTime { get; set; }
		[Microsoft.Xna.Framework.Content.ContentSerializerIgnore]
		public List<Bone> UpdateOrderBones { get; set; }

		public Keyframe()
		{
			UpdateOrderBones = new List<Bone>();
		}

		public void SortBones()
		{
			UpdateOrderBones.Clear();

			foreach (Bone bone in Bones)
			{
				BoneSortAdd(bone);
			}
		}

		protected void BoneSortAdd(Bone b)
		{
			if (UpdateOrderBones.Contains(b))
				return;

			if (b.ParentIndex != -1)
				BoneSortAdd(Bones[b.ParentIndex]);

			UpdateOrderBones.Add(b);
			b.UpdateIndex = UpdateOrderBones.Count - 1;
		}
	}
}
