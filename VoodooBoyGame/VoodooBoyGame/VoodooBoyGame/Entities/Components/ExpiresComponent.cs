using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;

namespace VoodooBoyGame
{
    public class ExpiresComponent : Component
    {
        private int lifeTime;

        public int LifeTime
        {
            get { return lifeTime; }
            set { lifeTime = value; }
        }

        public bool IsExpired
        {
            get { return lifeTime <= 0; }
        }

        public ExpiresComponent() { }

        public ExpiresComponent(int lifeTime)
        {
            LifeTime = lifeTime;
        }

        public void ReduceLifeTime(int lifeTime)
        {
            LifeTime -= lifeTime;
        }
    }
}
