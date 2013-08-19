using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Model.Entities
{
    public class Mine:Building
    {
        public int Income { get; set; }

        Area? EnemyArea1;
        Area? EnemyArea2;

        public Mine():base()
        {
            Income = 3;
        }
        public Area EnemySide(int Up)
        {
        
            switch (Up)
            {
                case 2:
                     if (EnemyArea1.HasValue)
                    {
                        return EnemyArea1.Value;
                    }           
                    EnemyArea1 = new Area()
                    {
                        World = new Rectangle(Constants.Bounds.Left,MyLane.Bounds.World.Top,(int)Location.World.X + Constants.Bounds.Width/2,MyLane.Bounds.World.Height)
                    };
                    return EnemyArea1.Value;
                case 1:
                    if (EnemyArea2.HasValue)
                    {
                        return EnemyArea2.Value;
                    }           
                    EnemyArea2 = new Area()
                    {
                        World = new Rectangle((int)Location.World.X, MyLane.Bounds.World.Top, Constants.Bounds.Width - (int)Location.World.X, (int)MyLane.Bounds.World.Height)
                    };
                    return EnemyArea2.Value;    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public Area MySide(int Up)
        {
            return EnemySide(Up == 1 ? 2 : 1); 
        }
        public override void Draw()
        {
            //Pen p1 = new Pen(Color.Aqua);
            //Pen p2 = new Pen(Color.Pink);
            //this.Color = this.Owner == null ? Color.LightGray : this.Owner.Color;
            //g.DrawRectangle(p1, this.MySide(1).Screen);
            //g.DrawRectangle(p2, this.MySide(2).Screen);
            //base.Paint(g);
        }
    }
}
