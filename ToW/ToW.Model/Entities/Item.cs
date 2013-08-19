using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities.Interfaces;

using Model.Arenas;

using Model.Entities.Helpers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToW.Model.Providers;

namespace Model.Entities
{
    public class Item
    {
        public Location Location;
        public Arena MyArena { get; set; }
        public int HitPoints { get; set; }

        
        public double Radius { get; set; }
        public double SoftRadius { get; set; }
        public Vector Velocity { get; set; }

        public List<AngleRange> CollisionRangeList;
        public Player Owner { get; set; }
     
        
        public int Priority;
        

        private Lane _MyLane;
        public Lane MyLane
        {
            get
            {
                if (_MyLane == null)
                {
                    foreach (var lane in MyArena.Lanes)
                    {


                        if (lane.Bounds.World.Contains((int)Location.World.X, (int)Location.World.Y))
                        {
                            _MyLane = lane;
                            return lane;
                        }
                    }
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    return _MyLane;
                }
            }
        }


        public Item()
        {
            Random r=new Random();
            Priority = r.Next();
            CollisionRangeList = new List<AngleRange>();
            HitPoints = 100;
        }
        public Area Bounds
        {
            get
            {
                return new Area()
                {
                    Screen = new Rectangle()
                        {
                            X = (int)Location.Screen.X - Convert.ToInt32(Radius * Constants.ScreenRatio),
                            Y = (int)Location.Screen.Y - Convert.ToInt32(Radius * Constants.ScreenRatio),
                            Width = Convert.ToInt32(Radius * (float)Constants.ScreenRatio),
                            Height = Convert.ToInt32(Radius * (float)Constants.ScreenRatio),
                        }
                };
            }
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
            string UnitSpriteName = "knight";
            string WalkString = "WalkLeft";
            if (Owner.Up == 1)
            {
                UnitSpriteName = "wizard";
                WalkString = "WalkRight";
            }
            else
            { 
            }
            GraphicsProvider.Current.DrawSpriteSheet(UnitSpriteName, WalkString, Bounds.Screen.Location);
            //sb.Draw(texture, Bounds.Screen, texture.Behaviours.First().AnimFrames[((int)MyArena.TickCount/30) % 2].Rectangle, Color.White);
            //sb.Draw(texture, Bounds.Screen, Color.White);
        }
        public Vector2 Add(Vector2 Left, Vector2 Right)
        {
            return new Vector2(Left.X + Right.X, Left.Y + Right.Y);
        }

    }
}
