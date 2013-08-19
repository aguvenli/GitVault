using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Model.Entities;
using Model.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToW.Model.Providers;
using ToWSpikeSpike.Entities;

namespace Model.Entities
{
    public class UIElement
    {

        public Area Bounds { get; set; }
        public string Text { get; set; }
        public bool isVisible { get; set; }
        public event EventHandler<UIElementEventArgs> Click;
        public object Tag { get; set; }

        public List<UIElement> UIElements { get; set; }

        public UIElement()
        {
            isVisible = true;
            Text = string.Empty;
            UIElements = new List<UIElement>();
        }

        public virtual void Draw()
        {
            if (isVisible)
            {
                GraphicsProvider.Current.DrawButton(Bounds.Screen, Text);
                foreach (var e in UIElements)
                {
                    e.Draw();
                }
            }
        }

        public virtual void Update(MouseState ms)
        {

        }

        public virtual void OnClick(MouseState ms)
        {
            if (Click != null)
            {
                Click(this, new UIElementEventArgs() { MouseState = ms });
            }
            List<UIElement> ClickedElements = new List<UIElement>();

            Location l1 = new Location() { Screen = new Vector2(ms.X, ms.Y) };
            Location l2 = new Location() { Grid = l1.Grid };

            foreach (var item in UIElements)
            {
                if (item.isVisible && item.Bounds.Screen.Contains(new Microsoft.Xna.Framework.Point(ms.X, ms.Y)))
                {
                    ClickedElements.Add(item);
                }
            }
            foreach (var item in ClickedElements)
            {
                item.OnClick(ms);
            }

        }

    }
}
