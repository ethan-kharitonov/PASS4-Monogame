using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    interface ISection
    {
        public void LoadContent();
        public void Update();
        public void Draw();
        public int GetMaxX();
        public int GetMaxY();

    }
}
