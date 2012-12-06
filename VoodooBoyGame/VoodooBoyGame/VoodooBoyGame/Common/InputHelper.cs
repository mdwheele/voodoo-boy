using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace VoodooBoyGame
{
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public class InputHelper
    {
        private GamePadState lastGamepadState;
        private GamePadState currentGamepadState;

#if (!XBOX)
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;
        private MouseState lastMouseState;
        private MouseState currentMouseState;
#endif
        private PlayerIndex playerIndex = PlayerIndex.One;
        private bool refreshData = false;
        private bool rumbleEnabled = true;
        TimeSpan rumbleStartTime;
        TimeSpan currentTime;
        double rumbleRatio;
        float leftMotor;
        float rightMotor;

        public InputHelper()
        {
            lastGamepadState = currentGamepadState = new GamePadState();

#if (!XBOX)
            lastKeyboardState = currentKeyboardState = new KeyboardState();
            lastMouseState = currentMouseState = new MouseState();
#endif
        }

        public void Update(GameTime gameTime)
        {
            if (!refreshData)
                refreshData = true;

            lastGamepadState = currentGamepadState;
            currentGamepadState = GamePad.GetState(playerIndex);

#if (!XBOX)
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
#endif

            if (rumbleEnabled)
            {
                currentTime -= gameTime.ElapsedGameTime;
                rumbleRatio = currentTime.Ticks / (double)rumbleStartTime.Ticks;

                if (currentTime.Milliseconds <= 0)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                }
                else
                {
                    GamePad.SetVibration(PlayerIndex.One, (float)(leftMotor * rumbleRatio), (float)(rightMotor * rumbleRatio));
                }
            }
        }

        public GamePadState LastGamepadState
        {
            get { return lastGamepadState; }
        }

        public GamePadState CurrentGamepadState
        {
            get { return currentGamepadState; }
        }

#if (!XBOX)
        public KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        public KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }

        public MouseState LastMouseState
        {
            get { return lastMouseState; }
        }

        public MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }
#endif

        #region Handle Input Events

        public Vector2 LeftStickPosition
        {
            get { return new Vector2(currentGamepadState.ThumbSticks.Left.X, -currentGamepadState.ThumbSticks.Left.Y); }
        }

        public Vector2 RightStickPosition
        {
            get { return new Vector2(currentGamepadState.ThumbSticks.Right.X, -currentGamepadState.ThumbSticks.Right.Y); }
        }

        public Vector2 LeftStickVelocity
        {
            get { Vector2 temp = currentGamepadState.ThumbSticks.Left - lastGamepadState.ThumbSticks.Left; return new Vector2(temp.X, -temp.Y); }
        }

        public Vector2 RightStickVelocity
        {
            get { Vector2 temp = currentGamepadState.ThumbSticks.Right - lastGamepadState.ThumbSticks.Right; return new Vector2(temp.X, -temp.Y); }
        }

        public float LeftTriggerPosition
        {
            get { return currentGamepadState.Triggers.Left; }
        }

        public float RightTriggerPosition
        {
            get { return currentGamepadState.Triggers.Right; }
        }

        public float LeftTriggerVelocity
        {
            get { return currentGamepadState.Triggers.Left - lastGamepadState.Triggers.Left; }
        }

        public float RightTriggerVelocity
        {
            get { return currentGamepadState.Triggers.Right - lastGamepadState.Triggers.Right; }
        }
#if (!XBOX)
        public Vector2 MousePosition
        {
            get { return new Vector2(currentMouseState.X, currentMouseState.Y); }
        }

        public Vector2 MouseVelocity
        {
            get { return (new Vector2(currentMouseState.X, currentMouseState.Y) - new Vector2(lastMouseState.X, lastMouseState.Y)); }
        }

        public float MouseScrollWheelPosition
        {
            get { return currentMouseState.ScrollWheelValue; }
        }

        public float MouseScrollWheelVelocity
        {
            get { return currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue; }
        }
#endif
        public bool IsPressed(Buttons button)
        {
            return (lastGamepadState.IsButtonUp(button) && currentGamepadState.IsButtonDown(button));
        }

        public bool IsHeldDown(Buttons button)
        {
            return (lastGamepadState.IsButtonDown(button) && currentGamepadState.IsButtonDown(button));
        }

        public bool IsReleased(Buttons button)
        {
            return (lastGamepadState.IsButtonDown(button) && currentGamepadState.IsButtonUp(button));
        }
#if (!XBOX)
        public bool IsPressed(Keys key)
        {
            return (lastKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key));
        }

        public bool IsHeldDown(Keys key)
        {
            return (lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key));
        }

        public bool IsReleased(Keys key)
        {
            return (lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key));
        }

        public bool IsPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (lastMouseState.MiddleButton == ButtonState.Released && currentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (lastMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (lastMouseState.XButton1 == ButtonState.Released && currentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (lastMouseState.XButton2 == ButtonState.Released && currentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        public bool IsHeldDown(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (lastMouseState.MiddleButton == ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (lastMouseState.XButton1 == ButtonState.Pressed && currentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (lastMouseState.XButton2 == ButtonState.Pressed && currentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        public bool IsReleased(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (lastMouseState.MiddleButton == ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (lastMouseState.XButton1 == ButtonState.Pressed && currentMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (lastMouseState.XButton2 == ButtonState.Pressed && currentMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }
#endif
        #endregion

        #region Macro Helpers

        public bool IsMenuSelect()
        {
            return IsPressed(Keys.Space) ||
                   IsPressed(Keys.Enter) ||
                   IsPressed(Buttons.A) ||
                   IsPressed(Buttons.Start);
        }

        public bool IsMenuCancel()
        {
            return IsPressed(Keys.Escape) ||
                   IsPressed(Buttons.B) ||
                   IsPressed(Buttons.Back);
        }

        public bool IsMenuUp()
        {
            return IsPressed(Keys.Up) ||
                   IsPressed(Buttons.DPadUp) ||
                   IsPressed(Buttons.LeftThumbstickUp);
        }

        public bool IsMenuDown()
        {
            return IsPressed(Keys.Down) ||
                   IsPressed(Buttons.DPadDown) ||
                   IsPressed(Buttons.LeftThumbstickDown);
        }

        public bool IsMenuPrev()
        {
            return IsPressed(Keys.Left) ||
                   IsPressed(Buttons.DPadLeft) ||
                   IsPressed(Buttons.LeftThumbstickLeft);
        }

        public bool IsMenuNext()
        {
            return IsPressed(Keys.Right) ||
                   IsPressed(Buttons.DPadRight) ||
                   IsPressed(Buttons.LeftThumbstickRight);
        }

        public bool IsPauseGame()
        {
            return IsPressed(Keys.Escape) ||
                   IsPressed(Buttons.Back) ||
                   IsPressed(Buttons.Start);
        }

        #endregion

        #region Rumble Feedback Helpers

        public void ShakePad(TimeSpan time, float leftMotor, float rightMotor)
        {
            this.leftMotor = leftMotor;
            this.rightMotor = rightMotor;
            currentTime = rumbleStartTime = time;
        }

        #endregion
    }
}
