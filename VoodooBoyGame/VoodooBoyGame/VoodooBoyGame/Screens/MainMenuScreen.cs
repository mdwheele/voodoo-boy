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
    class MainMenuScreen : MenuScreen
    {
        Texture2D background;
        Text toolbar;

        public MainMenuScreen()
            : base()
        {
            Control newGameEntry = new Control("START NEW GAME");
            Control continueGameEntry = new Control("CONTINUE", false);
            Control optionsEntry = new Control("OPTIONS");
            Control quitGameEntry = new Control("QUIT GAME");

            newGameEntry.Position = new Vector2(170, 400);
            continueGameEntry.Position = new Vector2(170, 440);
            optionsEntry.Position = new Vector2(170, 480);
            quitGameEntry.Position = new Vector2(170, 520);

            newGameEntry.Selected += new EventHandler<PlayerIndexEventArgs>(newGameEntrySelected);
            continueGameEntry.Selected += new EventHandler<PlayerIndexEventArgs>(continueGameEntrySelected);
            optionsEntry.Selected += new EventHandler<PlayerIndexEventArgs>(optionsEntrySelected);
            quitGameEntry.Selected += new EventHandler<PlayerIndexEventArgs>(quitGameEntrySelected);

            AddControl(newGameEntry);
            AddControl(continueGameEntry);
            AddControl(optionsEntry);
            AddControl(quitGameEntry);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            background = content.Load<Texture2D>("MenuBackgrounds/main-menu-screen-bg");
            toolbar = new Text(content, "[A] SELECT", "Fonts/ToolBarText", new Vector2(171, 590), new Color(133, 133, 133));

            //Load GameOptions
            GameOptions options = Global.LoadObject<GameOptions>(Global.containerName, Global.gameOptionsFileName);
            ScreenManager.VoodooBoyGame.GameOptions = (options != null) ? options : new GameOptions();
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

        protected override void OnCancel() { /* do nothing */ }
        
        void newGameEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LevelScreen("debug-level.gleed"), ControllingPlayer);
        }

        void continueGameEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.InputState.ShakePad(TimeSpan.FromMilliseconds(500), 0.0f, 1.0f);
        }

        void optionsEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsScreen(), ControllingPlayer);
        }

        void quitGameEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
