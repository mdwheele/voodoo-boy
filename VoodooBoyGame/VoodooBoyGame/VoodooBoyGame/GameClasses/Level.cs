using System.Collections.Generic;
using Artemis;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml.Linq;
using Gleed2D.InGame;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace VoodooBoyGame.GameClasses
{
    public class Level
    {
        #region Fields

        private Camera2D camera;
        private EntityWorld entityWorld;
        private World physicsWorld;
        private Dictionary<string, Layer> layers;
        private string fileName;

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

        public Camera2D Camera
        {
            get { return camera; }
        }

        #endregion

        public Level()
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
            systemManager.InitializeAll();

            #endregion

            layers = new Dictionary<string, Layer>();
        }

        public static Level FromGleed2D(string fileName)
        {
            Level level = new Level();
            level.FileName = fileName;

            Gleed2D.InGame.Level gleedLevel;

            using (Stream stream = TitleContainer.OpenStream(String.Format("Content/Levels/{0}", fileName))) //#1 change here you level file path
            {
                XElement xml = XElement.Load(stream);
                gleedLevel = LevelLoader.Load(xml);
            }

            foreach (Gleed2D.InGame.Layer layer in gleedLevel.Layers)
            {
                level.layers.Add(layer.Properties.Name, new Layer(layer.Properties.Name, layer.Properties.ScrollSpeed, level));

                foreach (LayerItem item in layer.Items)
                {
                    if (item.Properties is TextureItemProperties)
                    {
                        TextureItemProperties textureProperties = item.Properties as TextureItemProperties;
                        string filename = "Textures/" + Path.GetFileNameWithoutExtension(textureProperties.TexturePathRelativeToContentRoot); //3# change here your tile textures' file path
                        Texture2D texture = Global.Content.Load<Texture2D>(filename);

                        level.Layer(layer.Properties.Name).Add(new LayerTexture(texture, textureProperties.Position, textureProperties.Rotation, textureProperties.Scale));
                    }
                }
            }

            return level;
        }
    }
}
