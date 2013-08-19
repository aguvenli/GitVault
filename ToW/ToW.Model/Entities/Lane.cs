using System;  
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities.Helpers;
using Microsoft.Xna.Framework;



namespace Model.Entities
{
    public class Lane
    {
        public Lane(int Id)
        {
            Area a = new Area()
            {
                World = new Rectangle(Constants.Bounds.Left * 2,Constants.Bounds.Top + Constants.Bounds.Height / 3 * Id,
                                       Constants.Bounds.Width * 2, Constants.Bounds.Height /3),
            };
            Bounds = a;
        }


        public Area Bounds { get; set; }
        public int Id { get; set; }
    }
}
