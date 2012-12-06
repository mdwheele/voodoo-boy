using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace VoodooBoyGame
{
    [Serializable()]
    public class GameOptions
    {
        #region Fields

        int musicVolume;
        int soundVolume;

        #endregion

        #region Properties

        public int MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = value; }
        }

        public int SoundVolume
        {
            get { return soundVolume; }
            set { soundVolume = value; }
        }

        #endregion

        #region Initialize

        public GameOptions()
        {
            musicVolume = soundVolume = 5;
        }

        #endregion
    }
}
