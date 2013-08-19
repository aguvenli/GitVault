using Microsoft.Xna.Framework.Input;
using Model.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToW.Model.Providers
{
    public class MouseProvider:BaseProvider<MouseProvider>
    {
        public MouseState MouseState { get; set; }

        private int scrollValue;
        private int ScrollOffset;
        private const int ScrollRange=600;
        public int ScrollValue
        {
            get
            {
                int swl = MouseState.ScrollWheelValue / 2 + ScrollOffset;
                if (swl >= ScrollRange)
                {
                    ScrollOffset = ScrollRange - swl;
                }
                if (swl <= 0)
                {
                    ScrollOffset = - swl;
                }
                if (swl <= ScrollRange && swl>=0)
                {
                    scrollValue = swl;
                }
                return scrollValue;
            }
        }
        
    }



}
