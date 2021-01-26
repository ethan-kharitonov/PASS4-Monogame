using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
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
