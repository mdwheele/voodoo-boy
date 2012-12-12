using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Gleed2D;
using Gleed2D.InGame;
using FarseerPhysics.Collision.Shapes;
using Artemis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Common;
using FarseerPhysics;
using FarseerPhysics.DebugViews;
using System.Text;

namespace VoodooBoyGame
{
    public class LevelScreen : GameScreen
    {
        #region Fields

        private EntityWorld entityWorld;
        private World physicsWorld;
        private Dictionary<string, Layer> layers;
        private string levelName;

        private RenderSystem renderSystem;
        private BodyPhysicsSystem physicsSystem;
        private WalkableBodySystem walkableBodySystem;
        private AnimationUpdateSystem animationUpdateSystem;
        private AnimationRenderSystem animationRenderSystem;
        private PlayerControlSystem playerControlSystem;

        private DebugViewXNA debug;
        #endregion

        #region Properties

        public EntityWorld EntityWorld
        {
            get { return entityWorld; }
        }

        public World PhysicsWorld
        {
            get { return physicsWorld; }
        }

        public Layer Layer(string layerName)
        {
            if (layers.ContainsKey(layerName))
            {
                return layers[layerName];
            }
            else
            {
                return null;
            }
        }

        public string LevelName
        {
            get { return levelName; }
            set { levelName = value; }
        }

        #endregion

        public LevelScreen(string level)
            : base()
        {
            LevelName = level;
            Global.Camera.Position = new Vector2(800, 444);
            Global.Camera.Zoom = 1.0f;
            Global.Camera.MinPosition = new Vector2(783, 400);
            Global.Camera.MaxPosition = new Vector2(2400, 400);
        }

        public override void LoadContent()
        {
            #region Physics World Initialization
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            if (physicsWorld == null)
            {
                physicsWorld = new World(new Vector2(0f, 10f));
            }
            else
            {
                physicsWorld.Clear();
            }
            #endregion

            #region Entity World Initialization
            
            entityWorld = new EntityWorld();

            SystemManager systemManager = entityWorld.SystemManager;
            EntityWorld.SetEntityTemplate("Victor", new Player());

            renderSystem = systemManager.SetSystem(new RenderSystem(), ExecutionType.Draw);
            physicsSystem = systemManager.SetSystem(new BodyPhysicsSystem(PhysicsWorld), ExecutionType.Update);
            walkableBodySystem = systemManager.SetSystem(new WalkableBodySystem(PhysicsWorld), ExecutionType.Update);
            animationUpdateSystem = systemManager.SetSystem(new AnimationUpdateSystem(), ExecutionType.Update);
            animationRenderSystem = systemManager.SetSystem(new AnimationRenderSystem(), ExecutionType.Draw);
            playerControlSystem = systemManager.SetSystem(new PlayerControlSystem(), ExecutionType.Update);

            systemManager.InitializeAll();

            #endregion

            #region Load Level From Gleed2D File

            Gleed2D.InGame.Level level;            

            using (Stream stream = TitleContainer.OpenStream(String.Format("Content/Levels/{0}/level.gleed", LevelName))) //#1 change here you level file path
            {
                XElement xml = XElement.Load(stream);
                level = LevelLoader.Load(xml);
            }

            #region Load Texture Layers

            layers = new Dictionary<string, Layer>();
            foreach (Gleed2D.InGame.Layer layer in level.Layers)
            {
                if (!layer.Properties.Name.Equals("COLLISION") && !layer.Properties.Name.Equals("ENTITY"))
                {
                    layers.Add(layer.Properties.Name, new Layer(layer.Properties.Name, layer.Properties.ScrollSpeed, this));

                    foreach (LayerItem item in layer.Items)
                    {
                        if (item.Properties is TextureItemProperties)
                        {
                            TextureItemProperties textureProperties = item.Properties as TextureItemProperties;
                            string filename = String.Format("Levels/{0}/{1}", LevelName, System.IO.Path.GetFileNameWithoutExtension(textureProperties.TexturePathRelativeToContentRoot)); //3# change here your tile textures' file path
                            Console.WriteLine(textureProperties.TexturePathRelativeToContentRoot);
                            Texture2D texture = Global.Content.Load<Texture2D>(filename);

                            layers[layer.Properties.Name].Add(new LayerTexture(texture, textureProperties.Position, textureProperties.Rotation, textureProperties.Scale));
                        }
                    }
                }
            }

            #endregion

            #region Load Collision Layer

            foreach (Gleed2D.InGame.Layer layer in level.Layers)
            {
                if (layer.Properties.Name.Equals("COLLISION"))
                {
                    foreach (LayerItem item in layer.Items)
                    {
                        if (item.Properties is PathItemProperties)
                        {
                            PathItemProperties pathProperties = item.Properties as PathItemProperties;
                            Body body = new Body(PhysicsWorld);
                            body.Position = ConvertUnits.ToSimUnits(pathProperties.Position);

                            Vertices terrain = new Vertices();

                            foreach (Vector2 point in pathProperties.LocalPoints)
                            {
                                terrain.Add(ConvertUnits.ToSimUnits(point));
                            }

                            for (int i = 0; i < terrain.Count - 1; ++i)
                            {
                                FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], body);
                            }

                            body.Restitution = 0.3f;
                            body.Friction = float.MaxValue;
                        }

                        if (item.Properties is RectangleItemProperties)
                        {
                            RectangleItemProperties pathProperties = item.Properties as RectangleItemProperties;
                           
                            Body body = BodyFactory.CreateRectangle(PhysicsWorld, ConvertUnits.ToSimUnits(pathProperties.Width), ConvertUnits.ToSimUnits(pathProperties.Height), 64f, ConvertUnits.ToSimUnits(pathProperties.Position));
                            body.BodyType = BodyType.Static;
                            body.Rotation = pathProperties.Rotation;
                            body.Restitution = 0.3f;
                            body.Friction = 0f;
                        }
                    }
                }
            }

            #endregion
            
            #endregion

            debug = new DebugViewXNA(PhysicsWorld);
            debug.AppendFlags(DebugViewFlags.Shape);
            debug.DefaultShapeColor = Color.Red;
            debug.SleepingShapeColor = Color.LightGray;
            debug.LoadContent(Global.Graphics, Global.Content);

            Global.Camera.Rotation = 0.0f;

            Entity e = EntityWorld.CreateEntity("Victor", PhysicsWorld);
            Global.Camera.TrackingBody = e.GetComponent<WalkingComponent>().Body;
            e.Refresh();
        }

        public override void HandleInput(InputHelper input)
        {
            if (input.IsPressed(Buttons.Back) || input.IsPressed(Buttons.B) || input.IsPressed(Keys.Escape))
            {
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {            
            PhysicsWorld.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            
            EntityWorld.LoopStart();
            EntityWorld.Delta = gameTime.ElapsedGameTime.Milliseconds;
            EntityWorld.SystemManager.UpdateSynchronous(ExecutionType.Update);

            Global.Camera.Update();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            Global.Graphics.Clear(Color.CornflowerBlue);

            layers["BGLAYER1"].Draw();
            layers["BGLAYER2"].Draw();
            layers["BGLAYER3"].Draw();
            layers["LAYER1"].Draw();

            EntityWorld.SystemManager.UpdateSynchronous(ExecutionType.Draw);

            layers["LAYER2"].Draw();
            layers["LAYER3"].Draw();

            Entity victor = EntityWorld.TagManager.GetEntity("PLAYER");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("WalkingBody State Test");
            sb.AppendLine("----------------------\n");
            sb.AppendLine(String.Format("On Ground? {0}", victor.GetComponent<WalkingComponent>().OnGround));
            sb.AppendLine(String.Format("WalkingState: {0}", victor.GetComponent<WalkingComponent>().CurrState));
            sb.AppendLine(String.Format("Camera Position:\n{0}", Global.Camera.Position));

            Global.SpriteBatch.Begin();
            Global.SpriteBatch.DrawString(Global.Fonts["DebugBuild"], sb, new Vector2(10, 10), Color.White);
            Global.SpriteBatch.End();

            // calculate the projection and view adjustments for the debug view
            // Assume you have a _camera object with Zoom and Position properties
            Matrix projection, view;
          
            projection = Matrix.CreateOrthographic(Global.Graphics.Viewport.Width / 64f, -Global.Graphics.Viewport.Height / 64f, 0, 1);
            view = Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(-Global.Camera.Position), 0)) *
                   Matrix.CreateScale(Global.Camera.Zoom) *
                   Matrix.CreateRotationZ(Global.Camera.Rotation);
            
            //debug.RenderDebugData(ref projection, ref view);
        }
    }
}
