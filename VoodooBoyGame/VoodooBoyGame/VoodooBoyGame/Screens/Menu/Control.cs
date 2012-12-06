using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoodooBoyGame
{
    public class Control
    {
        #region Fields

        string text;
        string value;
        Vector2 position;
        bool selectable;
        MenuScreen menuScreen;

        #endregion

        #region Properties

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool Selectable
        {
            get { return selectable; }
            set { selectable = value; }
        }
        
        internal MenuScreen MenuScreen
        {
            get { return menuScreen; }
            set { menuScreen = value; }
        }
        
        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectControl(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        protected internal virtual void OnNextValue(PlayerIndex playerIndex) { }
        protected internal virtual void OnPrevValue(PlayerIndex playerIndex) { }

        #endregion

        #region Initialization

        public Control(string text)
        {
            Value = Text = text;
            Selectable = true;
        }

        public Control(string text, bool selectable)
        {
            Value = Text = text;
            Selectable = selectable;
        }

        public Control(string text, string value, bool selectable)
        {
            Text = text;
            Value = value;
            Selectable = selectable;
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        #endregion

        #region Update and Draw

        public virtual void Update(bool isSelected, GameTime gameTime)
        {
        }

        public virtual void Draw(bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Black: (Selectable ? new Color(91, 91, 91) : new Color(180, 180, 180));
            
            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = MenuScreen.ScreenManager;

            Global.SpriteBatch.DrawString(Global.Fonts["Voodoo"], text, position, color);
        }

        #endregion
    }
}
