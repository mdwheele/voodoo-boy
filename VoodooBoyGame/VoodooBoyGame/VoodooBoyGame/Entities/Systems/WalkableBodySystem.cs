using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;

namespace VoodooBoyGame
{
    class WalkableBodySystem : EntityProcessingSystem
    {
        private FarseerPhysics.Dynamics.World PhysicsWorld;
        private ComponentMapper<WalkingComponent> walkableBodyMapper;
        private ComponentMapper<TransformComponent> transformMapper;
        private ComponentMapper<AnimationComponent> animationMapper;

        private float groundRaycastThreshold;

        public WalkableBodySystem(FarseerPhysics.Dynamics.World physicsWorld)
            : base(typeof(WalkingComponent))
        {
            PhysicsWorld = physicsWorld;
        }

        public override void Initialize()
        {
            walkableBodyMapper = new ComponentMapper<WalkingComponent>(world);
            transformMapper = new ComponentMapper<TransformComponent>(world);
            animationMapper = new ComponentMapper<AnimationComponent>(world);

            groundRaycastThreshold = ConvertUnits.ToSimUnits(30);
        }

        public override void Process(Entity e)
        {
            if(e.HasComponent<TransformComponent>())
                transformMapper.Get(e).Position = ConvertUnits.ToDisplayUnits(walkableBodyMapper.Get(e).Body.Position)+transformMapper.Get(e).TransformOffset;

            //Apply downward force to stabilize the body.
            walkableBodyMapper.Get(e).Wheel.ApplyForce(new Vector2(0, 25f));
            
            /**
             * DETERMINE WHETHER BODY IS IN AIR OR NOT
             * 
             * To determine whether or not the body is in the air, we cast three rays at the 
             * left-bounds, center, and right-bounds of the body underneath the circle.  If either of them 
             * hit, we're on the ground.  If they all three return null, then we must be in the air.
             * 
             * @TODO: This is going to say we're on the ground when you are on top of another dynamic body. 
             * Through playtesting, we should see whether or not this is a big deal.
             */
            Vector2 bottomOfCircle = walkableBodyMapper.Get(e).Wheel.Position + (Vector2.UnitY * walkableBodyMapper.Get(e).Wheel.FixtureList[0].Shape.Radius);

            walkableBodyMapper.Get(e).OnGround = false;

            //Left-bound Raycast
            PhysicsWorld.RayCast((fixture, point, normal, fraction) =>
            {
                if (!fixture.Body.ToString().Equals(""))
                {
                    walkableBodyMapper.Get(e).OnGround = true;
                }
                return 0;
            }, bottomOfCircle - Vector2.UnitX * walkableBodyMapper.Get(e).Wheel.FixtureList[0].Shape.Radius, bottomOfCircle + Vector2.UnitY * groundRaycastThreshold);

            //Center Raycast
            PhysicsWorld.RayCast((fixture, point, normal, fraction) =>
            {
                if (!fixture.Body.ToString().Equals(""))
                {
                    walkableBodyMapper.Get(e).OnGround = true;
                }
                return 0;
            }, bottomOfCircle, bottomOfCircle + Vector2.UnitY * groundRaycastThreshold);

            //Right-bound Raycast
            PhysicsWorld.RayCast((fixture, point, normal, fraction) =>
            {
                if (!fixture.Body.ToString().Equals(""))
                {
                    walkableBodyMapper.Get(e).OnGround = true;
                }
                return 0;
            }, bottomOfCircle + Vector2.UnitX * walkableBodyMapper.Get(e).Wheel.FixtureList[0].Shape.Radius, bottomOfCircle + Vector2.UnitY * groundRaycastThreshold);   
         
            /**
             * FALLING STATE
             * 
             * Set falling state if LinearVelocity.Y > 0 and we're in the jumping state.
             */
            if (!walkableBodyMapper.Get(e).OnGround && walkableBodyMapper.Get(e).Body.LinearVelocity.Y > 0)
            {
                walkableBodyMapper.Get(e).PrevState = walkableBodyMapper.Get(e).CurrState;
                walkableBodyMapper.Get(e).CurrState = WalkingState.Falling;
            }

            /**
             * RESET MULTIPLE JUMP COUNTER IF ON GROUND
             */
            if (walkableBodyMapper.Get(e).OnGround)
            {
                walkableBodyMapper.Get(e).MultipleJumpCounter = 0;
            }

            /** 
             * JUMPING AIR DRAG
             */
            if (!walkableBodyMapper.Get(e).OnGround)
            {
                walkableBodyMapper.Get(e).Body.ApplyForce(-Vector2.UnitX * walkableBodyMapper.Get(e).Mass * walkableBodyMapper.Get(e).Body.LinearVelocity.X * 0.25f);
            }

            /**
             * ANIMATION
             * 
             * If the entity has an AnimationComponent, then start the appropriate animation
             * keyed on WalkingState.
             */
            if (e.HasComponent<AnimationComponent>())
            {
                AnimationComponent animation = animationMapper.Get(e);

                switch (walkableBodyMapper.Get(e).CurrState)
                {
                    case WalkingState.Resting:
                        animation.StartAnimation("resting");
                        break;

                    case WalkingState.WalkingLeft:
                        animation.Flipped = true;

                        if (walkableBodyMapper.Get(e).PrevState != WalkingState.WalkingLeft)
                            animation.StartAnimation("running");

                        break;

                    case WalkingState.WalkingRight:
                        animation.Flipped = false;

                        if (walkableBodyMapper.Get(e).PrevState != WalkingState.WalkingRight)
                            animation.StartAnimation("running");

                        break;

                    case WalkingState.Jumping:
                        if (walkableBodyMapper.Get(e).PrevState != WalkingState.Jumping)
                            animation.StartAnimation("jumping");
                        
                        break;

                    case WalkingState.Falling:
                        if(walkableBodyMapper.Get(e).PrevState != WalkingState.Falling)
                            animation.Transition("falling", 0.2f);
                    
                        break;
                }
            }
        }

        public override void OnRemoved(Entity e)
        {
        }
    }
}
