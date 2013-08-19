using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToW.Model.Providers;

namespace ToW.Model.Graphics
{
    public class SpriteSheet
    {
        public Texture2D Texture { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public const float Scale = 2F;
        public Dictionary<string,FrameSet> FrameSetList { get; set; }
        public SpriteSheet()
        {
            FrameSetList = new Dictionary<string, FrameSet>();
        }
        public void LoadFrameSet(string Name, int FrameStart, int FrameCount)
        {
            var frm=new FrameSet();
            frm.FrameStart = FrameStart;
            frm.FrameCount = FrameCount;
            FrameSetList[Name]= frm;
        }
        public void Draw(string FrameSetName, Rectangle rect)
        {
            FrameSet frm=FrameSetList[FrameSetName];

            var FrameNo = frm.FrameStart + GameTimeProvider.Current.FrameTick % frm.FrameCount;
            var FrameWidth = Texture.Bounds.Width / ColumnCount;
            var FrameHeight = Texture.Bounds.Height / RowCount;
            var FrameRowNo = FrameNo / ColumnCount;
            var FrameColumnNo = FrameNo % ColumnCount;
            var Clip = new Rectangle(FrameColumnNo * FrameWidth, FrameRowNo * FrameHeight, FrameWidth, FrameHeight);
            GraphicsProvider.Current.Draw(Texture, rect, Clip);
        }

        public void Draw(string FrameSetName, Vector2 Position)
        {
            Draw(FrameSetName, Position, false);
        }
        public void Draw(string FrameSetName, Vector2 Position, bool IsSwordsman)
        {
            FrameSet frm = FrameSetList[FrameSetName];
            var FrameNo = frm.FrameStart + GameTimeProvider.Current.FrameTick % frm.FrameCount;
            var FrameWidth = Texture.Bounds.Width / ColumnCount;
            var FrameHeight = Texture.Bounds.Height / RowCount;
            var FrameRowNo = FrameNo / ColumnCount;
            var FrameColumnNo = FrameNo % ColumnCount;
            var Clip = new Rectangle(FrameColumnNo * FrameWidth, FrameRowNo * FrameHeight, FrameWidth, FrameHeight);
            var rect = new Rectangle() { X = (int)Position.X, Y = (int)Position.Y, Width = (int)(Clip.Width / Scale), Height = (int)(Clip.Height / Scale) };
            if (IsSwordsman)
            {
                FrameWidth = 80;
                FrameHeight = 80;
                Clip = new Rectangle(FrameColumnNo * FrameWidth, FrameRowNo * FrameHeight, FrameWidth, FrameHeight);
                rect = new Rectangle() { X = (int)Position.X, Y = (int)Position.Y, Width = (int)(Clip.Width), Height = (int)(Clip.Height) };
            }
            GraphicsProvider.Current.Draw(Texture, rect, Clip);
        }
    }
    
    public class FrameSet
    {
        public int FrameStart { get; set; }
        public int FrameCount { get; set; }
    }
}