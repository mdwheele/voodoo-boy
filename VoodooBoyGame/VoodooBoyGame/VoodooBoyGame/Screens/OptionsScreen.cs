using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace VoodooBoyGame
{
    class OptionsScreen : MenuScreen
    {
        Texture2D background;
        Text toolbar;
        SliderControl soundSlider, musicSlider;

        public OptionsScreen()
            : base()
        {
            soundSlider = new SliderControl("SOUND");
            musicSlider = new SliderControl("MUSIC");

            soundSlider.Position = new Vector2(170, 400);
            musicSlider.Position = new Vector2(170, 440);

            AddControl(soundSlider);
            AddControl(musicSlider);
        }

        public override void LoadContent()
        {
            background = Global.Content.Load<Texture2D>("MenuBackgrounds/options-screen-bg");
            toolbar = new Text(Global.Content, "[A] SAVE [B] CANCEL", "Fonts/ToolBarText", new Vector2(171, 590), new Color(133, 133, 133));

            soundSlider.Value = ScreenManager.VoodooBoyGame.GameOptions.SoundVolume.ToString();
            musicSlider.Value = ScreenManager.VoodooBoyGame.GameOptions.MusicVolume.ToString();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw background!

            Global.SpriteBatch.Begin();
            Global.SpriteBatch.Draw(background, new Rectangle(0, 0, Global.Graphics.Viewport.Width, Global.Graphics.Viewport.Height), Color.White);
            toolbar.Draw(Global.SpriteBatch);
            Global.SpriteBatch.End();

            //Draws menu items
            base.Draw(gameTime);
        }

        protected override void OnSelectEntry(int entryIndex)
        {
            ScreenManager.VoodooBoyGame.GameOptions.SoundVolume = int.Parse(soundSlider.Value);
            ScreenManager.VoodooBoyGame.GameOptions.MusicVolume = int.Parse(musicSlider.Value);

            Global.SaveObject(Global.containerName, Global.gameOptionsFileName, ScreenManager.VoodooBoyGame.GameOptions);

            ExitScreen();
        }

        protected override void OnCancel() { ExitScreen(); }
    }
}
