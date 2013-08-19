using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities;
using Model.Entities.Interfaces;
using Model.Entities.Helpers;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ToWSpikeSpike;
using ToW.Model.Providers;


namespace Model.Arenas
{
    public class Arena : UIElement
    {
        public SpriteFont font { get; set; }
        public List<Lane> Lanes { get; set; }
        public List<Item> AllItems { get; set; }
        UIElement DesignMode = new UIElement();

        public List<Unit> AllUnits
        {
            get
            {
                return AllItems.Where(a => a is Unit).Cast<Unit>().ToList();
            }
        }
        public long TickCount = 0;
        public List<Player> PlayerList = new List<Player>();
        public Area Bounds { get; set; }
        public List<Unit> NewUnits { get; set; }
        public bool isDesignMode = false;
        public GameTime LastEventProcessTime = new GameTime();
        public List<UIElement> UIElements { get; set; }
        public UIElement UnitPlacementPanel = new UIElement();
        public UIElement UnitSelectionPanel = new UIElement();
        public Arena()
        {
            AllItems = new List<Item>();
            NewUnits = new List<Unit>();
            Bounds = new Area() { World = Constants.Bounds };
            UIElements = new List<UIElement>();
            Lanes = new List<Lane>();

        }
        public Player p1;
        public Player p2;

        public void Initialize()
        {
            p1 = new Player(this, 1);
            p2 = new Player(this, 2);

            this.UIElements.Add(p1);
            this.UIElements.Add(p2);

            this.PlayerList.Add(p1);
            this.PlayerList.Add(p2);

            for (int i = 0; i < 3; i++)
            {
                Lane lane = new Lane(i);
                Lanes.Add(lane);
            }
            DesignMode.Bounds = new Area() { World = new Rectangle() { X = Constants.ScreenBounds.Right - Constants.TileSize, Y = Constants.ScreenBounds.Bottom - Constants.TileSize, Width = Constants.TileSize, Height = Constants.TileSize } };
            DesignMode.Text = "Design";
            UIElements.Add(DesignMode);
            DesignMode.Click += DesignMode_Click;

            UnitPlacementPanel.Bounds = new Area() { World = new Rectangle() { X = Constants.ScreenBounds.Left, Y = Constants.ScreenBounds.Top, Width = Constants.TileSize * 12, Height = Constants.ScreenBounds.Height } };
            UnitPlacementPanel.isVisible = false;
            UIElements.Add(UnitPlacementPanel);
            UnitPlacementPanel.Click += ControlPanel_Click;

            UnitSelectionPanel.Bounds = new Area() { Grid = new Rectangle() { X = 0, Y = 0, Width = 3, Height = 3 } };
            UnitSelectionPanel.isVisible = false;

            this.UIElements.Add(UnitSelectionPanel);

            int oX = UnitSelectionPanel.Bounds.Grid.X;
            int oY = UnitSelectionPanel.Bounds.Grid.Y;
            int r = 0;
            foreach (var Building in PlayerList[0].BuildingTemplates)
            {
                UIElement btnBuilding = new UIElement();
                btnBuilding.Bounds = new Area() { Grid = new Rectangle() { X = oX + (r % 3), Y = oY + (r / 3), Width = 1, Height = 1 } };
                btnBuilding.Tag = Building;
                btnBuilding.Click += btnBuilding_Click;
                UnitSelectionPanel.UIElements.Add(btnBuilding);
                r++;
            }
        }
        Location UnitPlacementLocation;
        void btnBuilding_Click(object sender, ToWSpikeSpike.Entities.UIElementEventArgs e)
        {
            UIElement btn = (UIElement)sender;
            Factory b = (Factory)btn.Tag;
            b = (Factory)b.Clone();

            b.Location = UnitPlacementLocation;
            b.MyArena = this;
            this.AllItems.Add(b);
            UnitSelectionPanel.isVisible = false;
        }

        void ControlPanel_Click(object sender, ToWSpikeSpike.Entities.UIElementEventArgs e)
        {
            if (!UnitSelectionPanel.isVisible)
            {
                Location l = new Location() { Screen = new Vector2(e.MouseState.X, e.MouseState.Y) };
                l = new Location() { Grid = new Vector2(l.Grid.X - 10, l.Grid.Y) };
                UnitSelectionPanel.isVisible = true;
                UnitPlacementLocation = l;
            }
        }

        void DesignMode_Click(object sender, ToWSpikeSpike.Entities.UIElementEventArgs e)
        {
            isDesignMode = !isDesignMode;
            UnitPlacementPanel.isVisible = isDesignMode;
            UnitSelectionPanel.isVisible = false;
        }

        public override void Draw()
        {
            {
                GraphicsProvider.Current.FillRectangle(Bounds.Screen, Color.GhostWhite);
                GraphicsProvider.Current.DrawBackground(GraphicsProvider.Current.Background,Bounds.ScreenWithScroll);

                string text = string.Empty;
                var DrawItems = AllItems.OrderBy(a => a.Location.World.Y);
                foreach (var item in DrawItems)
                {
                   // if (Bounds.Screen.Contains((int)item.Location.Screen.X,(int)item.Location.Screen.Y))
                    {
                        item.Draw();
                    }
                    //text += string.Format("({0:000},{1:000})\n", item.Bounds.World.Left, item.Bounds.World.Top);
                }
                text = ToNetworkString();
                GraphicsProvider.Current.DrawString(text, new Vector2(1200, 200));
               
            }

            foreach (var item in UIElements)
            {
                item.Draw();
            }
            foreach (var item in AllItems.Where(a => a is Factory))
            {
                item.Draw();
            }
            Location l1 = new Location() { Screen = new Vector2(LastMouseState.X, LastMouseState.Y) };
            Location l2 = new Location() { Grid = l1.Grid };


            Rectangle rect = new Rectangle((int)l2.Screen.X, (int)l2.Screen.Y, (int)Constants.TileSize, (int)Constants.TileSize);
            GraphicsProvider.Current.DrawButton(rect, "");
        }
        public override void OnClick(MouseState ms)
        {
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
        MouseState LastMouseState;
        public override void Update(MouseState MouseState)
        {
            TickCount++;
            if (TickCount % 2 == 0)
            {
                LastMouseState = MouseState;
                foreach (var item in AllItems)
                {
                    item.MyArena = this;
                    item.Update();
                }

                List<Item> CloneItems = AllItems.Where(a => (a.HitPoints > 0 &&
                        (a.Location.World.X > this.Bounds.World.Left || a.Owner.Up == 1) &&
                        (a.Location.World.X < this.Bounds.World.Right || a.Owner.Up == 2)) || a is Building
                    ).ToList();

                PlayerList[0].UnitsPassed += CloneItems.Count(a => a.Owner != null && a.Owner.Up == 1 && PlayerList[1].BuildingArea.World.Contains((int)a.Location.World.X, (int)a.Location.World.Y));
                PlayerList[1].UnitsPassed += CloneItems.Count(a => a.Owner != null && a.Owner.Up == 2 && PlayerList[0].BuildingArea.World.Contains((int)a.Location.World.X, (int)a.Location.World.Y));
                CloneItems = CloneItems.Except(
                    CloneItems.Where(a => a.Owner != null && PlayerList[a.Owner.Up % 2].BuildingArea.World.Contains((int)a.Location.World.X, (int)a.Location.World.Y))
                ).ToList();
                foreach (var a in NewUnits)
                {
                    a.MyArena = this;
                }

                AllItems = CloneItems.Union<Item>(NewUnits.Cast<Item>()).ToList();
                NewUnits.Clear();

                if (TickCount % Constants.ResourceTickInterval == 0)
                {
                    UpdateMineOwners();
                }

                if (TickCount % Constants.ResourceTickInterval == 0)
                {
                    foreach (var item in PlayerList)
                    {
                        item.Cash += 10;
                    }
                    foreach (var item in AllItems.Where(a => a is Mine).Cast<Mine>())
                    {
                        if (item.Owner != null)
                        {
                            item.Owner.Cash += item.Income;
                        }
                    }
                }

            }

            if (MouseState.LeftButton == ButtonState.Pressed && (GameTimeProvider.Current.GameTime.TotalGameTime.TotalMilliseconds - LastEventProcessTime.TotalGameTime.TotalMilliseconds) > 100)
            {
                LastEventProcessTime = GameTimeProvider.Current.GameTime;
                OnClick(MouseState);
            }
            if (MouseState.LeftButton == ButtonState.Released)
            {
                LastEventProcessTime = new GameTime();
            }
            foreach (var item in UIElements)
            {
                item.Update(MouseState);
            }
        }

        private void UpdateMineOwners()
        {
            foreach (Mine mine in AllItems.Where(x => x is Mine).Cast<Mine>())
            {
                mine.Owner = null;
                foreach (var item in PlayerList)
                {
                    int FriendlyCount = this.AllUnits.Count(x => x.Owner.Up == item.Up && mine.EnemySide(item.Up).World.Contains((int)x.Location.World.X, (int)x.Location.World.Y));
                    int EnemyCount = this.AllUnits.Count(x => x.Owner.Up != item.Up && mine.MySide(item.Up).World.Contains((int)x.Location.World.X, (int)x.Location.World.Y));
                    if (FriendlyCount > 0 && EnemyCount == 0)
                    {
                        mine.Owner = item;
                    }
                }
            }
        }

        public void DoAILoop()
        {
            foreach (var unit in AllUnits)
            {
                unit.Act();
            }
            AllItems = AllItems.OrderBy(a => Math.Abs(a.Location.Grid.X)).ToList();
        }

        public static float GetDistance(Vector2 from, Vector2 to)
        {
            return (float)Math.Sqrt((from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y));
        }

        public string ToNetworkString()
        {
            StringBuilder sb = new StringBuilder();


            for (int j = -Constants.GRID_HORIZONTAL_OFFSET; j < Constants.GRID_HORIZONTAL_OFFSET; j++)
            {
                for (int i = -Constants.GRID_HORIZONTAL_OFFSET - 10; i < -Constants.GRID_HORIZONTAL_OFFSET; i++)
                {
                    Location l = new Location() { Grid = new Vector2(i, j) };
                    Factory f = (Factory)this.AllItems.FirstOrDefault(a => a is Factory && a.Location.Grid.X == i && a.Location.Grid.Y == j);
                    if (f == null)
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.Append(f.Name[0]);
                    }
                }
                sb.Append('\n');
            }

            return sb.ToString();
        }
        public void ParseNetworkString(string str)
        {
            
            string[] lines = str.Split('\n');
            for (int j = 0; j < lines.Length; j++)
            {
                for (int i = 0; i < lines[j].Length; i++)
                {
                    char c = lines[j][i];
                    if (c != ' ')
                    {
                        Factory f = (Factory)p2.BuildingTemplates.FirstOrDefault(a => a.Name.StartsWith(c.ToString()));
                        if (f != null)
                        {
                            f = (Factory)f.Clone();
                            f.Location = new Location() { Grid = new Vector2(Constants.GRID_HORIZONTAL_OFFSET + 10 - i, j) };
                            f.MyArena = this;
                            this.AllItems.Add(f);
                        }
                    }
                }
            }
        }
    }
}
