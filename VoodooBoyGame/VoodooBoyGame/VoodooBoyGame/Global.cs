using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyStorage;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VoodooBoyGame
{
    public class Global
    {
        public static IAsyncSaveDevice SaveDevice;
        public static string gameOptionsFileName = "GameOptions";
        public static string containerName = "VoodooBoySave";

        public static GraphicsDevice Graphics;
        public static SpriteBatch SpriteBatch;
        public static LineBatch LineBatch;
        public static ContentManager Content;
        public static Camera2D Camera;
        public static InputHelper Input;
        public static Dictionary<string, SpriteFont> Fonts;
        
        public static void SaveObject(string containerName, string fileName, object o){
            if (Global.SaveDevice.IsReady)
            {
                Global.SaveDevice.SaveAsync(
                    containerName,
                    fileName,
                    stream =>
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(stream, o);
                        }
                    });
            }
        }

        public static T LoadObject<T>(string containerName, string fileName)
        {
            T obj = default(T);

            if (Global.SaveDevice.FileExists(containerName, fileName))
            {
                Global.SaveDevice.Load(
                    containerName,
                    fileName,
                    stream =>
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            obj = (T)formatter.Deserialize(stream);
                        }
                    });
            }

            return obj;
        }
    }
}
