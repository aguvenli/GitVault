using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToW.Model.Providers
{
    public class GameTimeProvider:BaseProvider<GameTimeProvider>
    {
        public GameTime GameTime { get ; set; }
        public int FrameTick { get { return (int)(GameTime.TotalGameTime.TotalMilliseconds / 33); } }
    }
}
