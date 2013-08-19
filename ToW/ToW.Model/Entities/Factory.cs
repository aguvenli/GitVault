using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ToW.Model.Providers;

namespace Model.Entities
{
    public class Factory:Building
    {
        public Unit Product { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        public Factory(Arenas.Arena MyArena)
        {
            this.MyArena = MyArena;
            Velocity = new Vector();
        }

        public override void Draw()
        {
            if (MyArena.isDesignMode)
            {
                string UnitSpriteName = "knight";

                if (!string.IsNullOrEmpty(Name))
                {
                    UnitSpriteName = Name.ToLower();
                }

                string ActionString = "Walk";
                string DirectionString = "Left";

                if (this.Owner.Up==1)
                {
                    DirectionString = "Right";
                }
                Area a = new Area() { Grid = new Rectangle(Bounds.Grid.X+10, Bounds.Grid.Y, 1, 1) };

                GraphicsProvider.Current.DrawSpriteSheet(UnitSpriteName, ActionString + DirectionString, a.Screen.Location);
            }
        }
        public override void Update()
        {
            Velocity = new Vector();
            if (MyArena.TickCount % Constants.BuildingTickInterval == 0 && Counter.Count < 2)
            {
                Unit u = (Unit)this.Product.Clone();
                u.Location = this.Location;
                u.Owner = this.Owner;
                
                if(u.Name=="Horseman")
                {
                    u.AbilityList.Add(new ChargeAbility() { MaxCooldown = 100, MaxDuration = 40 });
                }
                foreach (var Ability in u.AbilityList)
                {
                    Ability.Owner = u;
                }
                u.FriendlyName = NameHelper.NextName;
                u.Id = NameHelper.GetNextId();
                float TargetX = this.Owner.Up == 1 ? Constants.GRID_HORIZONTAL_OFFSET : -Constants.GRID_HORIZONTAL_OFFSET;
                u.TargetLocation = new Helpers.Location() { World = new Vector2(TargetX, this.Location.World.Y) };
                MyArena.NewUnits.Add(u);
                u.MyArena = this.MyArena;
            }
        }


        public object Clone()
        {
            Factory tmp = new Factory(MyArena);
           
           ///TODO: Clone here

            tmp.Cost = this.Cost;
            tmp.HitPoints = this.HitPoints;
            tmp.Location = this.Location;
            tmp.MyArena = this.MyArena;
            tmp.Name = this.Name;
            tmp.Owner = this.Owner;
            tmp.Priority = this.Priority;
            tmp.Product = (Unit)this.Product.Clone();
            tmp.Radius = this.Radius;
            tmp.SoftRadius = this.SoftRadius;
            tmp.Velocity = this.Velocity;

            return tmp;
        }
    }
}
