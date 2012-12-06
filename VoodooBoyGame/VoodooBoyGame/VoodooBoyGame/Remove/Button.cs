using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace VoodooBoyGame
{
    class Button : Text
    {
        public enum Buttons
        {
            X,
            A,
            Y,
            B,
            None
        }

        public Buttons ButtonType;

        const string mButtonAssetName = "Utility/xboxControllerSpriteFont";

        public static string[] ButtonText = new string[] { Button.AButton, Button.BButton, Button.XButton, Button.YButton };
        
        public Button(ContentManager theContent, Buttons theButton, Vector2 thePosition)
            : base(theContent, "", mButtonAssetName, thePosition, Color.White)
        {
            ChangeButtonType(theButton);
        }

        public void ChangeButtonType(Buttons theButton)
        {
            ButtonType = theButton;
            mText = "";

            switch (theButton)
            {
                case Buttons.A:
                    {
                        mText = "'";
                        break;
                    }

                case Buttons.B:
                    {
                        mText = "(";
                        break;
                    }

                case Buttons.X:
                    {
                        mText = ")";
                        break;
                    }

                case Buttons.Y:
                    {
                        mText = "*";
                        break;
                    }
            }
        }

        public void ChangeButtonType(string theButton)
        {
            switch (theButton)
            {
                case XButton:
                    {
                        ChangeButtonType(Buttons.X);
                        break;
                    }

                case YButton:
                    {
                        ChangeButtonType(Buttons.Y);
                        break;
                    }

                case AButton:
                    {
                        ChangeButtonType(Buttons.A);
                        break;
                    }

                case BButton:
                    {
                        ChangeButtonType(Buttons.B);
                        break;
                    }
            }
        }

        public const string XButton = "[X]";
        public const string YButton = "[Y]";
        public const string AButton = "[A]";
        public const string BButton = "[B]";
        
        public Vector2 Length()
        {
            Vector2 aLength = mFont.MeasureString(mText);
            aLength.Y = aLength.Y / 3;            
            return aLength;
        }
    }
}
