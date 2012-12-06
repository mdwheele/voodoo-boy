using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Gleed2D;
using Gleed2D.InGame;
using FarseerPhysics.Collision.Shapes;
using Artemis;

namespace VoodooBoyGame
{
    public class GameLevel
    {
        #region Declarations

        Texture2D tex;

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public World FromWorld
        {
            get { return world; }
        }
        World world;

        Gleed2D.InGame.Level level;

        CollisionTile tile;
        CollisionPath pathTile;
        LayerTexture myTexture;

        List<CollisionTile> tiles;
        List<CollisionPath> pathTiles;
        List<LayerTexture> textures;

        List<Body> circles;
        Texture2D gib;

        bool paused = false;

        #endregion

        public GameLevel(ContentManager serviceProvider)
        {
            content = serviceProvider;

            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            if (world == null)
            {
                world = new World(new Vector2(0f, 10f));
            }
            else
            {
                world.Clear();
            }

            circles = new List<Body>();
            gib = content.Load<Texture2D>("Textures/gib");

            LoadTiles();
        }

        /// <summary>
        /// Tile loading method, reads gleed2d xml-file and sorts out paths and textures
        /// </summary>
        protected void LoadTiles()
        {
            using (Stream stream = TitleContainer.OpenStream("Content/Levels/gleedtest.gleed")) //#1 change here you level file path
            {
                XElement xml = XElement.Load(stream);
                level = LevelLoader.Load(xml);
            }

            tiles = new List<CollisionTile>();
            pathTiles = new List<CollisionPath>();
            textures = new List<LayerTexture>();

            foreach (Gleed2D.InGame.Layer layer in level.Layers)
            {
                if (layer.Properties.Name.Equals("Collision")) //2# change here your layer's name
                {
                    foreach (LayerItem item in layer.Items)
                    {
                        if (item.Properties is PathItemProperties)
                        {
                            PathItemProperties pathProperties = item.Properties as PathItemProperties;
                            pathTile = new CollisionPath(pathProperties.LocalPoints, pathProperties.Position, world);
                            pathTiles.Add(pathTile);
                        }
                        if (item.Properties is RectangleItemProperties)
                        {
                            RectangleItemProperties pathProperties = item.Properties as RectangleItemProperties;
                            tile = new CollisionTile(CollisionType.Impassable, pathProperties.Width, pathProperties.Height, pathProperties.Position, pathProperties.Rotation, world);
                            tiles.Add(tile);
                        }
                    }
                }

                if (layer.Properties.Name.Equals("Textures"))
                {
                    foreach (LayerItem item in layer.Items)
                    {
                        if (item.Properties is TextureItemProperties)
                        {
                            TextureItemProperties textureProperties = item.Properties as TextureItemProperties;
                            string filename = "Textures/" + Path.GetFileNameWithoutExtension(textureProperties.TexturePathRelativeToContentRoot); //3# change here your tile textures' file path
                            Texture2D texture = content.Load<Texture2D>(filename);
                            myTexture = new LayerTexture(texture, textureProperties.Position, textureProperties.Rotation, textureProperties.Scale);
                            textures.Add(myTexture);
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!paused)
            {
                Body body = BodyFactory.CreateCircle(world, 0.10f, 3.0f);
                body.BodyType = BodyType.Dynamic;
                body.Position = new Vector2(4.5f, 6.5f);
                body.LinearVelocity = new Vector2((new Random()).Next(1, 3), -10f);
                circles.Add(body);

                world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            }
        }

        public void HandleInput(InputHelper input)
        {
            if (input.IsPressed(Buttons.Start))
            {
                paused = !paused;
            }

            if (input.IsPressed(Buttons.A))
            {
                for (int i = 0; i < 10; i++)
                {
                    Body body = BodyFactory.CreateCircle(world, 0.10f, 3.0f);
                    body.BodyType = BodyType.Dynamic;
                    body.Position = new Vector2(4.5f, 6.5f);
                    body.LinearVelocity = new Vector2((new Random()).Next(1, 3), -10f);
                    circles.Add(body);
                }
            }

            if (input.IsPressed(Buttons.X))
            {
                for (int i = 0; i < 25; i++)
                {
                    Body body = BodyFactory.CreateCircle(world, 0.10f, 3.0f);
                    body.BodyType = BodyType.Dynamic;
                    body.Position = new Vector2(4.5f, 6.5f);
                    body.LinearVelocity = new Vector2((new Random()).Next(1, 3), -10f);
                    circles.Add(body);
                }
            }
        }

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, LineBatch lineBatch)
        {
            foreach (CollisionTile temp in tiles)
            {
                //temp.Draw(spriteBatch, tex);
            }

            foreach (LayerTexture t in textures)
            {
                t.Draw();
            }

            for (int i = 0; i < circles.Count; ++i)
            {
                Body body = circles[i];

                if (ConvertUnits.ToDisplayUnits(body.Position.X) > 1280 || ConvertUnits.ToDisplayUnits(body.Position.X) < 0 || ConvertUnits.ToDisplayUnits(body.Position.Y) > 720)
                {
                    body.Dispose();
                    circles.Remove(body);
                }

                Vector2 pos = ConvertUnits.ToDisplayUnits(circles[i].Position);
                spriteBatch.Draw(gib, new Rectangle((int)pos.X, (int)pos.Y, 8, 8), null, Color.White, circles[i].Rotation, new Vector2(gib.Width/2, gib.Height/2), SpriteEffects.None, 0f);
            }
        }
        #endregion

    }
}
