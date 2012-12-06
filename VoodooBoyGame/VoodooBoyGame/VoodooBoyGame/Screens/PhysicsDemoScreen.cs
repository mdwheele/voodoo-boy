using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using System.IO;
using EasyStorage;
using Microsoft.Xna.Framework.Input;

namespace VoodooBoyGame
{
    class PhysicsDemoScreen : GameScreen
    {
        GameLevel level;
        Camera2D cam;

        public PhysicsDemoScreen()
            : base()
        {
            cam = new Camera2D(new Vector2(1280, 720));
            cam.Position = new Vector2(600, 375);
        }

        public override void LoadContent()
        {
            level = new GameLevel(ScreenManager.Game.Content);
        }

        public override void HandleInput(InputHelper input)
        {
            if(input.IsPressed(Buttons.Back) || input.IsPressed(Buttons.B)){
                ExitScreen();
            }

            float hori = input.CurrentGamepadState.ThumbSticks.Left.X;
            float vert = -input.CurrentGamepadState.ThumbSticks.Left.Y;
            cam.Move(new Vector2(hori*10, vert*10));

            if (input.IsHeldDown(Buttons.RightTrigger))
            {
                cam.RotateRight();
            }

            if (input.IsHeldDown(Buttons.LeftTrigger))
            {
                cam.RotateLeft();
            }

            if (input.IsHeldDown(Buttons.RightThumbstickUp))
            {
                cam.ZoomIn();
            }

            if (input.IsHeldDown(Buttons.RightThumbstickDown))
            {
                cam.ZoomOut();
            }

            if (input.IsHeldDown(Buttons.Y))
            {
                input.ShakePad(TimeSpan.FromMilliseconds(100), 0.0f, 1.0f);
            }

            level.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            cam.Update();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            level.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw background!
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cam.CameraMatrix);
                level.Draw(gameTime, SpriteBatch, LineBatch);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(ScreenManager.FontDictionary["DebugBuild"], "Physics Testbed\n----------------------------\n\nLSTICK - Move Camera\nLTRIGGER / RTRIGGER - Rotate Camera\nRSTICKUP/DOWN - Zoom +/-\nA - Small Burst of Balls\nX - Large Burst of Balls\nY - Rumble\nStart - Pause\nB - Back to Main Menu", new Vector2(10, 10), Color.Black);
            spriteBatch.End();
        }
    }
}
