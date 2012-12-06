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

namespace VoodooBoyGame
{
    class TitleScreen : MenuScreen
    {
        IAsyncSaveDevice saveDevice;
        Texture2D background;

        public TitleScreen()
            : base()
        {
            Control pressAnyButtonEntry = new Control("PRESS ANY BUTTON");
            pressAnyButtonEntry.Position = new Vector2(495, 530);
            pressAnyButtonEntry.Selected += new EventHandler<PlayerIndexEventArgs>(PressAnyButtonSelected);
            AddControl(pressAnyButtonEntry);
        }

        public override void LoadContent()
        {
            background = Global.Content.Load<Texture2D>("MenuBackgrounds/title-screen-bg");
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw background!
            Global.SpriteBatch.Begin();
            Global.SpriteBatch.Draw(background, new Rectangle(0, 0, Global.Graphics.Viewport.Width, Global.Graphics.Viewport.Height), Color.White);
            Global.SpriteBatch.End();

            //Draws menu items
            base.Draw(gameTime);
        }

        protected override void OnCancel() { /* do nothing */ }

        void PressAnyButtonSelected(object sender, PlayerIndexEventArgs ev)
        { 
            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.Spanish);

            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            ScreenManager.Game.Components.Add(sharedSaveDevice);

            saveDevice = sharedSaveDevice;

            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

            sharedSaveDevice.PromptForDevice();
            sharedSaveDevice.DeviceSelected += (s, e) =>
            {
                Global.SaveDevice = (SaveDevice)s;

                if (this.IsActive)
                {
                    ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                    ExitScreen();
                }
            };

#if XBOX
            // add the GamerServicesComponent
            ScreenManager.Game.Components.Add(
                new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(ScreenManager.Game));
#endif
        }
    }
}
