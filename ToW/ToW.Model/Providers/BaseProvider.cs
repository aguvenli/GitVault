using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToW.Model.Providers
{
    public class BaseProvider<T>
    {
        public static T Current { get; set; }
        public Game Game { get; set; }
    }
}
