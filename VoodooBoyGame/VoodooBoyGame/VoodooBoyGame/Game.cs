using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VoodooBoyGame
{
    public class VoodooBoyGame : Microsoft.Xna.Framework.Game
    {
        internal GraphicsDeviceManager graphics;
        internal ScreenManager screenManager;
        internal GameOptions gameOptions;

        public GameOptions GameOptions
        {
            get { return gameOptions; }
            set { gameOptions = value; }
        }

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
        }

        public VoodooBoyGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
                        
            Window.Title = "Voodoo Boy Alpha";

            screenManager = new ScreenManager(this);
            gameOptions = new GameOptions();

            Components.Add(screenManager);

            screenManager.AddScreen(new TitleScreen(), null);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            base.Draw(gameTime);
        }
    }

#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (VoodooBoyGame game = new VoodooBoyGame())
            {
                game.Run();
            }
        }
    }
#endif
}