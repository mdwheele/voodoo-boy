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
    public class Text
    {
        protected SpriteFont mFont;
        public string mText;
        string mFontAssetName;
        public Vector2 Position;
        public Color mColor;
        public float Scale = 1.0f;

        public enum Alignment
        {
            Horizonatal,
            Vertical,
            Both
        }

        Button mButton;
        ContentManager mContent;

        public Text(ContentManager theContent, string theText, string theFontAssetName, Vector2 theStartingPosition, Color theColor)
        {
            mFont = theContent.Load<SpriteFont>(theFontAssetName);
            mFontAssetName = theFontAssetName;

            mText = theText;
            Position = theStartingPosition;
            mColor = theColor;

            mContent = theContent;
        }

        public virtual void Draw(SpriteBatch theBatch)
        {
            //Split the line of text to be drawn into the sections of text in between the buttons
            string[] aDisplayText = mText.Split(Button.ButtonText, StringSplitOptions.RemoveEmptyEntries);
            
            //Create the button if it hasn't been created yet
            if (mButton == null)
            {
                mButton = new Button(mContent, Button.Buttons.X, Vector2.Zero);
            }
            
            Vector2 aPosition = Position;
            
            //Check to see if the text to be drawn starts with a button image.
            if (mText.StartsWith("["))
            {
                SetButtonText(mText, "[");

                mButton.Position = aPosition;
                mButton.Position.Y -= mButton.Length().Y-3;
                mButton.Draw(theBatch);

                aPosition.X += mButton.Length().X;
            }

            //Cycle through all the sections and draw the text and buttons
            foreach (string aText in aDisplayText)
            {
                //Draw the text
                theBatch.DrawString(mFont, aText, aPosition, mColor);
                float aTextLength = mFont.MeasureString(aText).Length();

                //Set the text for the button
                SetButtonText(mText, aText);

                //Position the button
                mButton.Position = aPosition;
                mButton.Position.Y -= mButton.Length().Y;
                mButton.Position.X += aTextLength;
                
                //Draw the button image
                mButton.Draw(theBatch);

                //Position the next piece of text
                aPosition.X += aTextLength + mButton.Length().X;
            }
        }

        private void SetButtonText(string theFullText, string theCurrentText)
        {
            mButton.ChangeButtonType(Button.Buttons.None);

            int aStart = theFullText.IndexOf(theCurrentText);
            if (aStart == -1)
            {
                return;
            }

            int aLeftBracket = theFullText.IndexOf("[", aStart);
            if (aLeftBracket == -1)
            {
                return;
            }

            int aRightBracket = theFullText.IndexOf("]", aLeftBracket);
            if (aRightBracket == -1)
            {
                return;
            }

            string aButtonText = theFullText.Substring(aLeftBracket, aRightBracket - aLeftBracket + 1);
            mButton.ChangeButtonType(aButtonText);          
        }

        public void Center(Rectangle theDisplayArea, Alignment theAlignment)
        {
            Vector2 theTextSize = mFont.MeasureString(mText);

            switch (theAlignment)
            {
                case Alignment.Horizonatal:
                    {
                        Position.X = theDisplayArea.X + (theDisplayArea.Width / 2 - theTextSize.X / 2);
                        break;
                    }

                case Alignment.Vertical:
                    {
                        Position.Y = theDisplayArea.Y + (theDisplayArea.Height / 2 - theTextSize.Y / 2);                        
                        break;
                    }

                case Alignment.Both:
                    {
                        Position.X = theDisplayArea.X + (theDisplayArea.Width / 2 - theTextSize.X / 2);
                        Position.Y = theDisplayArea.Y + (theDisplayArea.Height / 2 - theTextSize.Y / 2); 
                        break;
                    }
            }
        }

    }
}
