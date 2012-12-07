using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Demina
{
	public class KeyframeTriggerEventArgs : EventArgs
	{
		public string TriggerString { get; set; }

		public KeyframeTriggerEventArgs()
		{
			TriggerString = "";
		}
	}

	public delegate void KeyframeTriggerEventHandler(object sender, KeyframeTriggerEventArgs e);

	public class AnimationPlayer
	{
		public string CurrentAnimation { get { return currentAnimation; } }
		public int CurrentKeyframe { get { return animations[currentAnimation].Keyframes[currentKeyframeIndex].FrameNumber; } }
		public bool Transitioning { get { return transitioning; } }

        public float PlaySpeedMultiplier { get; set; }

		public BoneTransformation[] BoneTransformations { get; protected set; }

		public event KeyframeTriggerEventHandler KeyframeTriggerEvent;
		KeyframeTriggerEventArgs triggerEventArgs = new KeyframeTriggerEventArgs();

        public AnimationPlayer()
        {
            PlaySpeedMultiplier = 1.0f;
        }

		public void AddAnimation(string name, Animation animation)
		{
			animations[name] = animation;

			if (BoneTransformations == null || animation.Keyframes[0].Bones.Count > BoneTransformations.Length)
			{
				BoneTransformations = new BoneTransformation[animation.Keyframes[0].Bones.Count];
				transitionStates = new BoneTransitionState[animation.Keyframes[0].Bones.Count];
			}
		}

		public void StartAnimation(string animation)
		{
			StartAnimation(animation, false);
		}

		public void StartAnimation(string animation, bool allowRestart)
		{
			transitioning = false;

			if (currentAnimation != animation || allowRestart)
			{
				currentAnimation = animation;
				currentAnimationTime = 0;
				currentKeyframeIndex = 0;

				foreach (Bone b in animations[currentAnimation].Keyframes[0].Bones)
				{
					transitionStates[b.UpdateIndex].Position = b.Position;
					transitionStates[b.UpdateIndex].Rotation = b.Rotation;
				}
			}

			Update(0);
		}

		public void ForceAnimationSwitch(string animation)
		{
			currentAnimation = animation;
		}

		public void TransitionToAnimation(string animation, float time)
		{
			if (transitioning)
			{
				Animation.UpdateBoneTransitions(transitionStates, animations[currentAnimation], animations[transitionAnimation], transitionTime / transitionTotalTime);
			}

			transitioning = true;
			transitionTime = 0;
			transitionTotalTime = time;
			transitionAnimation = animation;
		}

		public int GetBoneTransformIndex(string boneName)
		{
			Animation animation = animations[currentAnimation];

			for (int boneIndex = 0; boneIndex < animation.Keyframes[0].Bones.Count; boneIndex++)
			{
				Bone bone = animation.Keyframes[currentKeyframeIndex].Bones[boneIndex];
				if (bone.Name == boneName)
					return boneIndex;
			}

			return -1;
		}

		public bool Update(float deltaSeconds)
		{
			if (string.IsNullOrEmpty(currentAnimation))
				return false;

			bool returnValue = false;
			int startKeyframeIndex = currentKeyframeIndex;

            deltaSeconds *= PlaySpeedMultiplier;

			if (transitioning)
			{
				transitionTime += deltaSeconds;

				if (transitionTime > transitionTotalTime)
				{
					transitioning = false;

					currentAnimation = transitionAnimation;
					currentAnimationTime = transitionTime - transitionTotalTime;
					currentKeyframeIndex = 0;

					Animation animation = animations[currentAnimation];
					animation.GetBoneTransformations(BoneTransformations, transitionStates, currentKeyframeIndex, currentAnimationTime - animation.Keyframes[currentKeyframeIndex].FrameTime);
				}
				else
				{
					Animation.GetBoneTransformationsTransition(BoneTransformations, transitionStates, animations[currentAnimation], animations[transitionAnimation], transitionTime / transitionTotalTime);
				}
			}
			else
			{
				bool reachedEnd = false;

				currentAnimationTime += deltaSeconds;

				Animation animation = animations[currentAnimation];

				if (currentKeyframeIndex == animation.Keyframes.Count - 1)
				{
					if (animation.Loop)
					{
						if (currentAnimationTime > animation.LoopTime)
						{
							currentAnimationTime -= animation.LoopTime;
							currentKeyframeIndex = 0;
						}
					}
					else
					{
						currentAnimationTime = animation.Keyframes[currentKeyframeIndex].FrameTime;
						reachedEnd = true;
					}
				}
				else
				{
					if (currentAnimationTime > animation.Keyframes[currentKeyframeIndex + 1].FrameTime)
						currentKeyframeIndex++;
				}

				animation.GetBoneTransformations(BoneTransformations, transitionStates, currentKeyframeIndex, currentAnimationTime - animation.Keyframes[currentKeyframeIndex].FrameTime);

				returnValue = reachedEnd;
			}

			if (currentKeyframeIndex != startKeyframeIndex && KeyframeTriggerEvent != null &&
				!string.IsNullOrEmpty(animations[currentAnimation].Keyframes[currentKeyframeIndex].Trigger))
			{
				triggerEventArgs.TriggerString = animations[currentAnimation].Keyframes[currentKeyframeIndex].Trigger;
				KeyframeTriggerEvent(this, triggerEventArgs);
			}

			return returnValue;
		}

		public bool Update(GameTime gameTime)
		{
			return Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			Draw(spriteBatch, position, false, false, 0, Color.White, new Vector2(1, 1), Matrix.Identity);
		}

        /// <summary>
        /// Draw the given animation using the passed in spritebatch. Use this option if you have an existing spritebatch you would like to pass in.
		/// Make sure that you have backface culling disabled if you want to use bone texture flipping.
        /// </summary>
        /// <param name="spriteBatch">Existing active spritebatch. Begin/end will not be called.</param>
        /// <param name="tintColor">Color to tint the animation.</param>
        /// <param name="position">Position of the animation to draw.</param>
        public void Draw(ref SpriteBatch spriteBatch, Color tintColor, Vector2 position)
        {
            Animation animation = animations[currentAnimation];

            for (int boneIndex = 0; boneIndex < animation.Keyframes[0].Bones.Count; boneIndex++)
            {
                Bone bone = animation.Keyframes[currentKeyframeIndex].Bones[boneIndex];
                if (bone.Hidden)
                    continue;

                SpriteEffects spriteEffects = SpriteEffects.None;
				Vector2 scale = BoneTransformations[boneIndex].Scale;

				// originally we were flipping using SpriteEffects, but that doesn't mirror about the texture origin
				// so now we're using negative scaling. This requires backface culling disabled.
				if (bone.TextureFlipHorizontal)
				{
					scale.X *= -1;
					//spriteEffects = SpriteEffects.FlipHorizontally;
				}
				if (bone.TextureFlipVertical)
				{
					scale.Y *= -1;
					//spriteEffects |= SpriteEffects.FlipVertically;
				}

                spriteBatch.Draw(animation.Textures[bone.TextureIndex].Texture, position + BoneTransformations[boneIndex].Position, animation.Textures[bone.TextureIndex].TextureBounds.Location, tintColor,
                    BoneTransformations[boneIndex].Rotation, animation.Textures[bone.TextureIndex].TextureBounds.Origin,
                    scale, spriteEffects, 0);
            }
        }

		public void Draw(SpriteBatch spriteBatch, Vector2 position, bool flipHorizontal, bool flipVertical, float rotation, Color tintColor, Vector2 scale, Matrix cameraTransform)
		{
			if (string.IsNullOrEmpty(currentAnimation))
				return;

			Animation animation = animations[currentAnimation];

			flipHorizontal |= animation.Keyframes[currentKeyframeIndex].FlipHorizontally;
			flipVertical |= animation.Keyframes[currentKeyframeIndex].FlipVertically;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null,
                Matrix.CreateScale(scale.X * (flipHorizontal ? -1 : 1), scale.Y * (flipVertical ? -1 : 1), 1) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(position.X, position.Y, 0) *
                cameraTransform);

            Draw(ref spriteBatch, tintColor, new Vector2(0,0));

			spriteBatch.End();
		}

		string currentAnimation;
		float currentAnimationTime;
		int currentKeyframeIndex;

		bool transitioning = false;
		string transitionAnimation;
		float transitionTime;
		float transitionTotalTime;

		BoneTransitionState[] transitionStates;

		Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
	}
}
