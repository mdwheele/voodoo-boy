using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace VoodooBoyGame
{
    class PlayerControlSystem : TagSystem
    {
        private ComponentMapper<WalkingComponent> walkingComponent;
        private InputHelper input;

        public PlayerControlSystem()
            : base("PLAYER")
        {
            input = Global.Input;
        }

        public override void Initialize()
        {
            walkingComponent = new ComponentMapper<WalkingComponent>(world);
        }

        public override void Process(Entity e)
        {
            if (e.HasComponent<WalkingComponent>())
            {
                WalkingComponent body = walkingComponent.Get(e);

                if (body.OnGround && body.CurrState != WalkingState.Jumping)
                {
                    //Walking Left, On Ground
                    if (input.IsHeldDown(Buttons.LeftThumbstickLeft))
                    {
                        body.MoveLeft();
                    }

                    //Walking Right, On Ground
                    if (input.IsHeldDown(Buttons.LeftThumbstickRight))
                    {
                        body.MoveRight();
                    }
                    
                    if (!input.IsHeldDown(Buttons.LeftThumbstickRight) && !input.IsHeldDown(Buttons.LeftThumbstickLeft))
                    {
                        body.StopMoving();
                    }

                    //Jump
                    if (input.IsPressed(Buttons.A))
                    {
                        body.Jump();
                    }
                }
                else
                {

                }
            }
        }
    }
}
