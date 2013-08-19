using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToWSpikeSpike.Entities
{
    public class UIElementEventArgs:EventArgs
    {
        public  MouseState MouseState { get; set; }
    }
}
