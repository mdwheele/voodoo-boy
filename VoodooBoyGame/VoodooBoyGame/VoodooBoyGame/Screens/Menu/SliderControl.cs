using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoodooBoyGame
{
    public class SliderControl : Control
    {
        #region Fields

        Texture2D blank;
        private const int MinSliderValue = 0;
        private const int MaxSliderValue = 10;
        private const int LabelWidth = 100;
        private const int SliderBarHeight = 10;
        private const int SliderBarWidth = 200;
        private const int MarkerWidth = 10;
        private const int MarkerHeight = 20;
        private Vector2 MarkerPosition;

        #endregion

        #region Initialization

        public SliderControl(string text)
            : base(text, true)
        {
            Value = "0";
            MarkerPosition.X = Position.X + LabelWidth + ((float.Parse(Value) / MaxSliderValue) * SliderBarWidth) - MarkerWidth / 2;
            MarkerPosition.Y = Position.Y + SliderBarHeight - MarkerHeight / 2;
        }

        public override void LoadContent()
        {
            ContentManager content = MenuScreen.ScreenManager.Game.Content;
            blank = content.Load<Texture2D>("Utility/blank");
        }

        #endregion

        #region Update and Draw

        public override void Update(bool isSelected, GameTime gameTime)
        {
            //Update marker position.
            MarkerPosition.X = Position.X + LabelWidth + ((float.Parse(Value) / MaxSliderValue) * SliderBarWidth) - MarkerWidth / 2;
            MarkerPosition.Y = Position.Y + SliderBarHeight - MarkerHeight / 2;
        }

        public override void Draw(bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Black : (Selectable ? new Color(91, 91, 91) : new Color(180, 180, 180));

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = MenuScreen.ScreenManager;
            SpriteFont voodooFont = Global.Fonts["Voodoo"];
            SpriteFont arialFont = Global.Fonts["Arial"];

            Global.SpriteBatch.DrawString(voodooFont, Text, Position, color);

            //Draw slider bar.
            Global.SpriteBatch.Draw(blank, new Rectangle((int)(Position.X + LabelWidth), (int)Position.Y + 5, SliderBarWidth, SliderBarHeight), color);

            //Draw slider ticker.
            Global.SpriteBatch.Draw(blank, new Rectangle((int)(MarkerPosition.X), (int)MarkerPosition.Y, MarkerWidth, MarkerHeight), color);

            //Draw value at end of bar.
            Global.SpriteBatch.DrawString(arialFont, (int.Parse(Value) > 0) ? Value : "OFF", new Vector2(Position.X + LabelWidth + SliderBarWidth + 15, Position.Y - 2), color);
        }
        
        #endregion

        #region Events

        protected internal override void OnNextValue(PlayerIndex playerIndex)
        {
            Value = (MathHelper.Clamp(int.Parse(Value) + 1, MinSliderValue, MaxSliderValue)).ToString();
        }

        protected internal override void OnPrevValue(PlayerIndex playerIndex)
        {
            Value = (MathHelper.Clamp(int.Parse(Value) - 1, MinSliderValue, MaxSliderValue)).ToString();
        }

        #endregion
    }
}
