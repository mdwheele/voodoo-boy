using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Demina
{
	public class Animation
	{
		public string Version { get; set; }

		public int FrameRate { get; set; }

		public bool Loop { get; set; }
		public int LoopFrame { get; set; }
		public float LoopTime { get; set; }

		public List<TextureEntry> Textures { get; set; }
		public List<Keyframe> Keyframes { get; set; }

		public void GetBoneTransformations(BoneTransformation[] transforms, BoneTransitionState[] transitions, int keyframeIndex, float time)
		{
			Keyframe currentKeyframe = Keyframes[keyframeIndex];
			Keyframe nextKeyframe;
			float t;

			if (keyframeIndex == Keyframes.Count - 1)
			{
				nextKeyframe = Keyframes[0];

				if (Loop)
					t = time / (LoopTime - currentKeyframe.FrameTime);
				else
					t = 0;
			}
			else
			{
				nextKeyframe = Keyframes[keyframeIndex + 1];
				t = time / (nextKeyframe.FrameTime - currentKeyframe.FrameTime);
			}

			for (int boneIndex = 0; boneIndex < Keyframes[0].UpdateOrderBones.Count; boneIndex++)
			{
				Vector2 position = Vector2.Lerp(currentKeyframe.UpdateOrderBones[boneIndex].Position, nextKeyframe.UpdateOrderBones[boneIndex].Position, t);
				Vector2 scale = Vector2.Lerp(currentKeyframe.UpdateOrderBones[boneIndex].Scale, nextKeyframe.UpdateOrderBones[boneIndex].Scale, t);
				float rotation = MathHelper.Lerp(currentKeyframe.UpdateOrderBones[boneIndex].Rotation, nextKeyframe.UpdateOrderBones[boneIndex].Rotation, t);

				transitions[boneIndex].Position = position;
				transitions[boneIndex].Rotation = rotation;

				Matrix parentTransform = currentKeyframe.UpdateOrderBones[boneIndex].ParentIndex == -1 ? Matrix.Identity : transforms[currentKeyframe.UpdateOrderBones[boneIndex].ParentIndex].Transform;

				int drawIndex = currentKeyframe.UpdateOrderBones[boneIndex].SelfIndex;

				transforms[drawIndex].Transform = Matrix.CreateScale(scale.X, scale.Y, 1) *
					Matrix.CreateRotationZ(rotation) *
					Matrix.CreateTranslation(position.X, position.Y, 0) *
					parentTransform;

				Vector3 position3, scale3;
				Vector2 direction;
				Quaternion rotationQ;

				transforms[drawIndex].Transform.Decompose(out scale3, out rotationQ, out position3);
				direction = Vector2.Transform(Vector2.UnitX, rotationQ);

				transforms[drawIndex].Position = new Vector2(position3.X, position3.Y);
				transforms[drawIndex].Scale = new Vector2(scale3.X, scale3.Y);
				transforms[drawIndex].Rotation = (float)Math.Atan2(direction.Y, direction.X);
			}
		}

		public static void GetBoneTransformationsTransition(BoneTransformation[] transforms, BoneTransitionState[] transitionState, Animation currentAnimation, Animation stopAnimation, float transitionPosition)
		{
			for (int boneIndex = 0; boneIndex < currentAnimation.Keyframes[0].UpdateOrderBones.Count; boneIndex++)
			{
				Bone currentBone = currentAnimation.Keyframes[0].UpdateOrderBones[boneIndex];
				Bone transitionBone = null;

				foreach (Bone b in stopAnimation.Keyframes[0].UpdateOrderBones)
				{
					if (currentBone.Name == b.Name)
					{
						transitionBone = b;
						break;
					}
				}

				if (transitionBone == null)
					continue;

				Vector2 position = Vector2.Lerp(transitionState[boneIndex].Position, transitionBone.Position, transitionPosition);
				Vector2 scale = new Vector2(1, 1);
				float rotation = MathHelper.Lerp(transitionState[boneIndex].Rotation, transitionBone.Rotation, transitionPosition);

				Matrix parentTransform = currentBone.ParentIndex == -1 ? Matrix.Identity : transforms[currentBone.ParentIndex].Transform;

				int drawIndex = currentBone.SelfIndex;

				transforms[drawIndex].Transform = Matrix.CreateScale(scale.X, scale.Y, 1) *
					Matrix.CreateRotationZ(rotation) *
					Matrix.CreateTranslation(position.X, position.Y, 0) *
					parentTransform;

				Vector3 position3, scale3;
				Vector2 direction;
				Quaternion rotationQ;

				transforms[drawIndex].Transform.Decompose(out scale3, out rotationQ, out position3);
				direction = Vector2.Transform(Vector2.UnitX, rotationQ);

				transforms[drawIndex].Position = new Vector2(position3.X, position3.Y);
				transforms[drawIndex].Rotation = (float)Math.Atan2(direction.Y, direction.X);
				transforms[drawIndex].Scale = new Vector2(scale3.X, scale3.Y);
			}
		}

		public static void UpdateBoneTransitions(BoneTransitionState[] transitionState, Animation currentAnimation, Animation stopAnimation, float transitionPosition)
		{
			for (int boneIndex = 0; boneIndex < currentAnimation.Keyframes[0].UpdateOrderBones.Count; boneIndex++)
			{
				Bone currentBone = currentAnimation.Keyframes[0].UpdateOrderBones[boneIndex];
				Bone transitionBone = null;

				foreach (Bone b in stopAnimation.Keyframes[0].UpdateOrderBones)
				{
					if (currentBone.Name == b.Name)
					{
						transitionBone = b;
						break;
					}
				}

				if (transitionBone == null)
					continue;

				transitionState[boneIndex].Position = Vector2.Lerp(transitionState[boneIndex].Position, transitionBone.Position, transitionPosition);
				transitionState[boneIndex].Rotation = MathHelper.Lerp(transitionState[boneIndex].Rotation, transitionBone.Rotation, transitionPosition);
			}
		}
	}
}
