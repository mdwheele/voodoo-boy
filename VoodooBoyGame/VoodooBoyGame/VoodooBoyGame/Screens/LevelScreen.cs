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

namespace VoodooBoyGame
{
    public class LevelScreen : GameScreen
    {
        #region Fields

        private EntityWorld entityWorld;
        private World physicsWorld;
        private Dictionary<string, Layer> layers;
        private string fileName;

        private RenderSystem renderSystem;
        private PhysicsSystem physicsSystem;
        private WalkableBodySystem walkableBodySystem;
        private AnimationUpdateSystem animationUpdateSystem;
        private AnimationRenderSystem animationRenderSystem;

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

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        #endregion

        public LevelScreen(string levelFileName)
            : base()
        {
            FileName = levelFileName;
            Global.Camera.Position = new Vector2(664, 444);
            Global.Camera.Zoom = 1.0f;
            Global.Camera.MinPosition = new Vector2(266, 128);
            Global.Camera.MaxPosition = new Vector2(2620, 740);
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
            EntityWorld.SetEntityTemplate("Gib", new GibTemplate());
            EntityWorld.SetEntityTemplate("BodyTest", new WalkableBodyTemplate());

            renderSystem = systemManager.SetSystem(new RenderSystem(), ExecutionType.Draw);
            physicsSystem = systemManager.SetSystem(new PhysicsSystem(PhysicsWorld), ExecutionType.Update);
            walkableBodySystem = systemManager.SetSystem(new WalkableBodySystem(PhysicsWorld), ExecutionType.Update);
            animationUpdateSystem = systemManager.SetSystem(new AnimationUpdateSystem(), ExecutionType.Update);
            animationRenderSystem = systemManager.SetSystem(new AnimationRenderSystem(), ExecutionType.Draw);

            systemManager.InitializeAll();

            #endregion

            #region Load Level From Gleed2D File

            Gleed2D.InGame.Level level;            

            using (Stream stream = TitleContainer.OpenStream(String.Format("Content/Levels/{0}", FileName))) //#1 change here you level file path
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
                            string filename = "Textures/" + System.IO.Path.GetFileNameWithoutExtension(textureProperties.TexturePathRelativeToContentRoot); //3# change here your tile textures' file path
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
            Global.Camera.Zoom = 0.75f;

            Entity e = EntityWorld.CreateEntity("BodyTest", PhysicsWorld);
            Global.Camera.TrackingBody = e.GetComponent<WalkableBodyComponent>().Body;
            e.Refresh();
        }

        public override void HandleInput(InputHelper input)
        {
            if (input.IsPressed(Buttons.Back) || input.IsPressed(Buttons.B) || input.IsPressed(Keys.Escape))
            {
                ExitScreen();
            }

            float hori = input.CurrentGamepadState.ThumbSticks.Left.X;
            float vert = -input.CurrentGamepadState.ThumbSticks.Left.Y;

            if (input.IsHeldDown(Keys.Right)) { Global.Camera.Move(new Vector2(10, 0)); }
            if(input.IsHeldDown(Keys.Left)){Global.Camera.Move(new Vector2(-10, 0));}
            if(input.IsHeldDown(Keys.Up)){Global.Camera.Move(new Vector2(0, -10));}
            if(input.IsHeldDown(Keys.Down)){Global.Camera.Move(new Vector2(0, 10));}

            if (input.IsHeldDown(Buttons.RightTrigger) || input.IsHeldDown(Keys.Z))
            {
                Global.Camera.RotateRight();
                PhysicsWorld.Gravity = new Vector2(PhysicsWorld.Gravity.X + 0.05f, PhysicsWorld.Gravity.Y);
            }

            if (input.IsHeldDown(Buttons.LeftTrigger) || input.IsHeldDown(Keys.A))
            {
                Global.Camera.RotateLeft();
                PhysicsWorld.Gravity = new Vector2(PhysicsWorld.Gravity.X - 0.05f, PhysicsWorld.Gravity.Y);
            }

            if (input.IsHeldDown(Buttons.RightThumbstickUp) || input.IsHeldDown(Keys.PageUp))
            {
                Global.Camera.ZoomIn();
            }

            if (input.IsHeldDown(Buttons.RightThumbstickDown) || input.IsHeldDown(Keys.PageDown))
            {
                Global.Camera.ZoomOut();
            }

            if (input.IsPressed(Buttons.Y))
            {
                input.ShakePad(TimeSpan.FromMilliseconds(200), 0.0f, 1.0f);
            }

            if (input.IsHeldDown(Buttons.RightShoulder))
            {
                Entity e = EntityWorld.CreateEntity("Gib", PhysicsWorld);
                e.GetComponent<BodyComponent>().Body.Position = ConvertUnits.ToSimUnits(Global.Camera.Position);
                e.GetComponent<BodyComponent>().Body.LinearVelocity = new Vector2((new Random()).Next(-5, 5), -5f);
                e.Refresh();
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
            Global.Graphics.Clear(Color.AliceBlue);

            foreach (KeyValuePair<string, Layer> layer in layers)
            {
                layer.Value.Draw();
            }

            EntityWorld.SystemManager.UpdateSynchronous(ExecutionType.Draw);

            Global.SpriteBatch.Begin();
            Global.SpriteBatch.DrawString(Global.Fonts["DebugBuild"], String.Format("Level Testbed\n---------------------\n\nLSTICK - Move Camera\nLTRIGGER / RTRIGGER(A / Z) - Rotate Camera\nRSTICKUP/DOWN(PAGEUP/DOWN) - Zoom +/-\nA(SPACE) - Spawn balls at Center Screen\nB(ESC) - Back to Main Menu\n\nZoom Level: {0}\nCamera Position: {1}\nPhysics Bodies: {2}\nEntities: {3}", Global.Camera.Zoom, Global.Camera.Position, PhysicsWorld.BodyList.Count-6, EntityWorld.EntityManager.ActiveEntitiesCount), new Vector2(10, 10), Color.Black);
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
