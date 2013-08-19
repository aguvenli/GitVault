
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToW.Model.Providers;

namespace Model.Entities.Helpers
{
    public static class Constants
    { 
        public static int ScreenRatio;
        public static int TileSize;
        public static Rectangle Bounds;
        public static Rectangle ScreenBounds;
        public static int HORIZONTAL_OFFSET;
        public static int VERTICAL_OFFSET;
        public static int GRID_HORIZONTAL_OFFSET;
        public static int GRID_VERTICAL_OFFSET;
        public static int PlayerAreaGridWidth;
        public static int BuildingTickInterval;
        public static int ResourceTickInterval;
        public static int MineOwnershipTickInterval;
        public static float AvoidanceThreshold = 20;

        static Constants()
        {
            BuildingTickInterval = 800;
            ScreenRatio =1;
            TileSize = 50;
            PlayerAreaGridWidth = 3;
            Bounds = new Rectangle(-750, -300, 1500, 600);
            HORIZONTAL_OFFSET = (int)(Bounds.Width * Constants.ScreenRatio/2.0);
            VERTICAL_OFFSET = (int)(Bounds.Height * Constants.ScreenRatio/2.0);
            ScreenBounds = new Rectangle(-750, -300, 900, 600);
            GRID_HORIZONTAL_OFFSET=(int)Math.Round((Bounds.Width / TileSize / 2.0));
            GRID_VERTICAL_OFFSET = (int)Math.Round((Bounds.Height / TileSize / 2.0));
            ResourceTickInterval = 20;
            MineOwnershipTickInterval = 5;
        }

    }

    public static class Counter
    {
        public static int Count { get; set; }
    }

    public class Line
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }

        public float A
        {
            get
            {
                return Y2 - Y1;
            }
        }

        public float B
        {
            get
            {
                return X1 - X2;
            }
        }

        public float C
        {
            get
            {
                return A * X1 + B * Y1;
            }
        }


        public Vector2 ScreenStart
        {
            get
            {
                Location l = new Location();
                l.FromVector(new Vector() { X = X1, Y = Y1 });
                return l.Screen;
            }
        }

        public Vector2 ScreenEnd 
        {
            get 
            {
                Location l = new Location();
                l.FromVector(new Vector() { X = X2, Y = Y2 });
                return l.Screen;
            }
        }

        public Vector ToVector()
        {
            float X = X2 - X1;
            float Y = Y2 - Y1;
            return new Vector() { X = X, Y = Y };
        }

        public Vector ToWorldTarget()
        {
            return new Vector() { X = this.X2, Y = this.Y2 };
        }
    }

    public static class VectorOps
    {
        public static Vector Subtract(Vector v1, Vector v2)
        {
            Vector ret = new Vector(v1.X - v2.X, v1.Y - v2.Y);
            return ret;
        }

        public static float dot(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }


    }

    public static class Extensions
    {
        public static Vector ToVector(this Vector2 pf)
        {
            return new Vector() { X = pf.X, Y = pf.Y };
        }
    }

    public struct Vector
    {
        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }

        public override string ToString()
        {
            return string.Format("X:{0} | Y:{1}", this.X, this.Y);
        }

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;

        public void Add(Vector v)
        {
            X += v.X;
            Y += v.Y;
        }

        public Vector Add2(Vector v)
        {
            Vector ret = new Vector();
            ret.Y = Y + v.Y;
            ret.X = X + v.X;
            return ret;
        }


        public Vector Subtract(Vector v)
        {
            Vector ret = new Vector();
            ret.Y = Y - v.Y;
            ret.X = X - v.X;
            return ret;
        }

        public void Mult(float num)
        {
            X = X * num;
            Y = Y * num;
        }

        public Vector Perp(Vector v)
        {
            Vector First = new Vector() { X = -v.Y, Y = v.X };
            Vector Second = new Vector() { X = v.Y, Y = -v.X };

            Random r = new Random(DateTime.Now.Millisecond);
            int z = r.Next(0, 1);

            if (z == 0)
            {
                return First;
            }
            else
            {
                return Second;
            }
        }


        public Vector Mult2(float num)
        {
            Vector ret = new Vector();
            ret.Y = Y * num;
            ret.X = X * num;
            return ret;
        }

        public void Div(float num)
        {
            X = X / num;
            Y = Y / num;
        }

        public Vector Div2(float num)
        {
            X = X / num;
            Y = Y / num;
            return new Vector() { X = X, Y = Y };
        }

        public float Mag()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public float Len()
        {
            return Math.Abs(Mag());
        }

        public void Normalize()
        {
            float m = Mag();
            if (m > 0)
            {
                Div(m);
            }
        }
        public Vector Normalize2()
        {
            float m = Mag();
            if (m > 0)
            {
                Div(m);
            }
            return new Vector() { X = this.X, Y = this.Y };
        }


        public Vector Limit(float num)
        {
            var mag = Mag();

            if (mag > num)
            {
                float lim = Mag() / num;
                Div(lim);
            }
            return new Vector() { X = this.X, Y = this.Y };
        }

        public Line ToLine()
        {
            Line ret = new Line()
            {
                X1 = 0,
                X2 = X,
                Y1 = 0,
                Y2 = Y,
            };
            return ret;
        }

    }

    public struct Location
    {

        public void FromVector(Vector v)
        {
            World.Y = v.Y;
            World.X = v.X;
        }

        public Vector ToVector()
        {
            return new Vector(World.X, World.Y);
        }

        public Vector2 World;
        public Vector2 Screen
        {
            get
            {
                return new Vector2(World.X * Constants.ScreenRatio + Constants.HORIZONTAL_OFFSET , World.Y * Constants.ScreenRatio + Constants.VERTICAL_OFFSET  );
            }
            set
            {
                World = new Vector2(
                    ((float)value.X - Constants.HORIZONTAL_OFFSET) / Constants.ScreenRatio ,
                    ((float)value.Y - Constants.VERTICAL_OFFSET) / Constants.ScreenRatio 
                );
            }
        }
        public Vector2 ScreenWithScroll
        {
            get
            {
                return new Vector2(World.X * Constants.ScreenRatio + Constants.HORIZONTAL_OFFSET - MouseProvider.Current.ScrollValue, World.Y * Constants.ScreenRatio + Constants.VERTICAL_OFFSET);
            }
            set
            {
                World = new Vector2(
                    ((float)value.X - Constants.HORIZONTAL_OFFSET) / Constants.ScreenRatio + MouseProvider.Current.ScrollValue,
                    ((float)value.Y - Constants.VERTICAL_OFFSET) / Constants.ScreenRatio
                );
            }
        }
        public Vector2 Grid
        {
            get
            {
                return new Vector2(Convert.ToInt32(((World.X) / Constants.TileSize)), (Convert.ToInt32((World.Y) / Constants.TileSize)));
            }
            set
            {
                World = new Vector2(
                    (value.X * Constants.TileSize),
                    (value.Y * Constants.TileSize)
                );
            }
        }
    }

    public struct Area
    {
        public Rectangle World { get; set; }
        public Rectangle Screen
        {
            get
            {
                return new Rectangle(
                    ((World.X * Constants.ScreenRatio + Constants.HORIZONTAL_OFFSET)),
                    ((World.Y * Constants.ScreenRatio + Constants.VERTICAL_OFFSET)),
                    ((World.Width * Constants.ScreenRatio)),
                    ((World.Height * Constants.ScreenRatio))
                );
            }
            set
            {
                World = new Rectangle(
                      ((int)value.X - Constants.HORIZONTAL_OFFSET) / Constants.ScreenRatio,
                    ((int)value.Y - Constants.VERTICAL_OFFSET) / Constants.ScreenRatio,
                    ((int)value.X - Constants.HORIZONTAL_OFFSET) / Constants.ScreenRatio + (value.Width / Constants.ScreenRatio),
                    ((int)value.Y - Constants.VERTICAL_OFFSET) / Constants.ScreenRatio + (value.Height / Constants.ScreenRatio));
                    
            }
        }
        public Rectangle ScreenWithScroll
        {
            get
            {
                return new Rectangle(
                    ((World.X * Constants.ScreenRatio + Constants.HORIZONTAL_OFFSET - MouseProvider.Current.ScrollValue)),
                    ((World.Y * Constants.ScreenRatio + Constants.VERTICAL_OFFSET)),
                    ((World.Width * Constants.ScreenRatio)),
                    ((World.Height * Constants.ScreenRatio))
                );
            }
            set
            {
                World = new Rectangle(
                      ((int)value.X + Constants.HORIZONTAL_OFFSET - MouseProvider.Current.ScrollValue) / Constants.ScreenRatio,
                    ((int)value.Y - Constants.VERTICAL_OFFSET) / Constants.ScreenRatio,
                    ((int)value.X - Constants.HORIZONTAL_OFFSET) / Constants.ScreenRatio + (value.Width / Constants.ScreenRatio),
                    ((int)value.Y - Constants.VERTICAL_OFFSET) / Constants.ScreenRatio + (value.Height / Constants.ScreenRatio));

            }
        }
        public Rectangle Grid
        {
            get
            {
                return new Rectangle(
                    ((World.X / Constants.TileSize)),
                    ((World.Y / Constants.TileSize)),
                    ((World.Width / Constants.TileSize)),
                    ((World.Height / Constants.TileSize))
                );
            }
            set
            {
                World = new Rectangle(
                    value.X * Constants.TileSize,
                    value.Y * Constants.TileSize,
                    value.Width * Constants.TileSize,
                    value.Height * Constants.TileSize
                );
            }
        }

    }
}
