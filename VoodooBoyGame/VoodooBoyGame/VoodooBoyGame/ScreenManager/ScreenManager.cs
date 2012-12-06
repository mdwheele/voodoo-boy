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
using System.Reflection;

namespace VoodooBoyGame
{
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        InputHelper input = new InputHelper();

        Dictionary<string, SpriteFont> fontDictionary;

        bool isInitialized;
        private PlayerIndex playerIndex;

        VoodooBoyGame game;

#if(DEBUG)
        //FPS Variables
        int totalFrames = 0;
        float elapsedTime = 0.0f;
        int fps = 0;
#endif

        #endregion

        #region Properties

        public Dictionary<string, SpriteFont> FontDictionary
        {
            get { return fontDictionary; }
        }

        public InputHelper InputHelper
        {
            get { return input; }
        }

        public VoodooBoyGame VoodooBoyGame
        {
            get { return game; }
            set { game = value; }
        }

        #endregion

        #region Initialization
        
        public ScreenManager(VoodooBoyGame game) : base(game)
        {
            VoodooBoyGame = game;
        }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            Global.Graphics = GraphicsDevice;
            Global.Camera = new Camera2D(new Vector2(VoodooBoyGame.Graphics.PreferredBackBufferWidth, VoodooBoyGame.Graphics.PreferredBackBufferHeight));
            Global.SpriteBatch = new SpriteBatch(GraphicsDevice);
            Global.LineBatch = new LineBatch(GraphicsDevice);
            Global.Content = Game.Content;
            Global.Input = InputHelper;

            // load font dictionary
            Global.Fonts = new Dictionary<string, SpriteFont>();
            Global.Fonts.Add("DebugBuild", Global.Content.Load<SpriteFont>("Fonts/DebugBuild"));
            Global.Fonts.Add("Voodoo", Global.Content.Load<SpriteFont>("Fonts/Voodoo"));
            Global.Fonts.Add("Arial", Global.Content.Load<SpriteFont>("Fonts/Arial"));
            
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {
            input.Update(gameTime);
            
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while (screensToUpdate.Count > 0)
            {
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);
                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            totalFrames++;

            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;
                
                screen.Draw(gameTime);
            }

            Global.SpriteBatch.Begin();
            Global.SpriteBatch.DrawString(Global.Fonts["DebugBuild"], String.Format("{0} FPS - Alpha Build", fps), new Vector2(10, GraphicsDevice.Viewport.Height - 25), Color.Gray);
            Global.SpriteBatch.End();
        } 

        #endregion

        #region Public Methods

        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        #endregion
    }
}
