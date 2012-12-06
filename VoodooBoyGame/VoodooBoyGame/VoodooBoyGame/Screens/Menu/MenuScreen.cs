using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoodooBoyGame
{
    public class MenuScreen : GameScreen
    {
        #region Fields

        List<Control> menuEntries = new List<Control>();
        int selectedEntry = 0;

        #endregion

        #region Properties

        internal IList<Control> MenuEntries
        {
            get { return menuEntries; }
        }

        
        #endregion

        #region Initialization

        public MenuScreen()
        {
        }

        public override void LoadContent()
        {
            foreach (Control control in menuEntries)
            {
                control.LoadContent();
            }
        }

        public override void UnloadContent()
        {
            foreach (Control control in menuEntries)
            {
                control.UnloadContent();
            }
        }

        #endregion

        #region Events

        protected virtual void OnSelectEntry(int entryIndex)
        {
            if (menuEntries.Count > 0)
                menuEntries[entryIndex].OnSelectControl(PlayerIndex.One);
        }

        protected virtual void OnNextValue(int entryIndex)
        {
            if (menuEntries.Count > 0)
                menuEntries[entryIndex].OnNextValue(PlayerIndex.One);
        }

        protected virtual void OnPrevValue(int entryIndex)
        {
            if (menuEntries.Count > 0)
                menuEntries[entryIndex].OnPrevValue(PlayerIndex.One);
        }

        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel();
        }

        #endregion

        #region Update and Draw

        public override void HandleInput(InputHelper input)
        {
            if (input.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                if (MenuEntries.Count > 0)
                    if (!menuEntries[selectedEntry].Selectable)
                    {
                        selectedEntry = GetNextSelectableIndexBefore(selectedEntry);
                    }
            }

            if (input.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                if (MenuEntries.Count > 0)
                    if (!menuEntries[selectedEntry].Selectable)
                    {
                        selectedEntry = GetNextSelectableIndexAfter(selectedEntry);
                    }
            }


            if (input.IsMenuPrev())
            {
                OnPrevValue(selectedEntry);
            }

            if (input.IsMenuNext())
            {
                OnNextValue(selectedEntry);
            }

            if (input.IsMenuSelect())
            {
                OnSelectEntry(selectedEntry);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntries[i].Update(isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Global.SpriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                Control menuEntry = menuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(isSelected, gameTime);
            }

            Global.SpriteBatch.End();
        }

        #endregion

        #region Public Methods
        public void AddControl(Control control)
        {
            control.MenuScreen = this;
            menuEntries.Add(control);
        }

        public void RemoveControl(Control control)
        {
            menuEntries.Remove(control);
        }
        #endregion

        #region Helper Methods
        private int GetNextSelectableIndexAfter(int currIndex)
        {
            int i = currIndex;
            int nextControlIndex = (int)MathHelper.Clamp(i + 1, 0, menuEntries.Count - 1);

            if (menuEntries[nextControlIndex].Selectable)
            {
                return nextControlIndex;
            }
            else
            {
                return GetNextSelectableIndexAfter(nextControlIndex);
            }
        }

        private int GetNextSelectableIndexBefore(int currIndex)
        {
            int i = currIndex;
            int prevControlIndex = (int)MathHelper.Clamp(i - 1, 0, menuEntries.Count - 1);

            if (menuEntries[prevControlIndex].Selectable)
            {
                return prevControlIndex;
            }
            else
            {
                return GetNextSelectableIndexBefore(prevControlIndex);
            }
        } 
        #endregion
    }
}
