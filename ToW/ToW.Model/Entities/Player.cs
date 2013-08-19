using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Entities.Helpers;
using Model.Arenas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToW.Model.Providers;
using Microsoft.Xna.Framework.Input;


namespace Model.Entities
{
    public class Player:UIElement
    {
        public Player()
        {
            Modifiers = new List<string>();
            Cash = 1000;
            float DefaultRadius = Constants.TileSize/2F;
            List<Factory> _buildingTemplates = new List<Factory>();

            float _MaxAcceleration = Constants.TileSize / 5F ;
            float DefaultMaxSpeed = Constants.TileSize / 40F;
            float DefaultWeaponRange = Constants.TileSize * 2 + 4;
            float DefaultSightRange = Constants.TileSize * 12;
            float DefaultSoftRadius = DefaultRadius * 1.2F;
          

            _buildingTemplates.Add(new Factory(this.MyArena)
            {
                Name = "Horseman",
                HitPoints = 10000,
                Owner = this,
                Cost = 300,
                Product = new Unit()
                {
                    Name = "Horseman",
                    MaxAcceleration = _MaxAcceleration,
                    MaxSpeed = DefaultMaxSpeed,
                    Radius = DefaultRadius,
                    SoftRadius = DefaultSoftRadius,
                    WeaponRange = DefaultWeaponRange/2,
                    SightRange = DefaultSightRange,
                    Damage = 7, HitPoints = 3000,
                    ArmorType=Entities.ArmorType.Plate, DamageAgainstPlateRatio = .70F,DamageAgainstNonPlateRatio = 1.00F,
                }
            });
            _buildingTemplates.Add(new Factory(this.MyArena)
            {
                Name = "Spearman",
                HitPoints = 10000,
                Owner = this,
                Cost = 120,
                Product = new Unit() { Name = "Spearman",
                                       MaxAcceleration = _MaxAcceleration,
                                       MaxSpeed = DefaultMaxSpeed,
                                       Radius = DefaultRadius,
                                       SoftRadius = DefaultSoftRadius,
                                       WeaponRange = DefaultWeaponRange,
                                       SightRange = DefaultSightRange,

                    Damage = 2, HitPoints = 2000, 
                                       DamageAgainstPlateRatio = 1.00F,
                                       DamageAgainstNonPlateRatio = 0.90F,
                }
            });

            _buildingTemplates.Add(new Factory(this.MyArena)
            {
                Name = "Archer",
                HitPoints = 10000,
                Owner = this,
                Cost = 140,
                Product = new Unit() { Name = "Archer",

                                       MaxAcceleration = _MaxAcceleration,
                                       MaxSpeed = DefaultMaxSpeed,
                                       Radius = DefaultRadius,
                                       SoftRadius = DefaultSoftRadius,
                                       WeaponRange = DefaultWeaponRange * 2,
                                       SightRange = DefaultSightRange,
                                       
                                       Damage = 6,
                                       HitPoints = 1300,
                                       DamageAgainstPlateRatio = .80F,
                                       DamageAgainstNonPlateRatio = 0.80F,
                }
            });


            _buildingTemplates.Add(new Factory(this.MyArena)
            {
                Name = "Mage",
                HitPoints = 10000,
                Owner = this,
                Cost = 325,
                Product = new Unit() { Name = "Mage",
                                       MaxAcceleration = _MaxAcceleration,
                                       MaxSpeed = DefaultMaxSpeed,
                                       Radius = DefaultRadius,
                                       SoftRadius = DefaultSoftRadius,
                                       WeaponRange = DefaultWeaponRange * 3,
                                       SightRange = DefaultSightRange,
                                       
                                       Damage = 12,
                                       HitPoints = 500,
                                       DamageAgainstPlateRatio = 0.50F,
                                       DamageAgainstNonPlateRatio = 1.00F,
                }
            });
            _buildingTemplates.Add(new Factory(this.MyArena)
            {
                Name = "Infantry",
                HitPoints = 10000,
                Owner = this,
                Cost = 150,
                Product = new Unit()
                {
                    Name = "Swordsmen",
                    MaxAcceleration = _MaxAcceleration,
                    MaxSpeed = DefaultMaxSpeed,
                    Radius = DefaultRadius,
                    SoftRadius = DefaultSoftRadius,
                    WeaponRange = DefaultWeaponRange/2,
                    SightRange = DefaultSightRange,
                    DamageAgainstPlateRatio = .70F,
                    Damage = 7,
                    HitPoints = 3000,
                    DamageAgainstNonPlateRatio = 1.0F,
                }
            });

            this.BuildingTemplates = _buildingTemplates;

             TechTemplates = new List<Tech>();

             TechTemplates.Add(new WeaponsUpgrade());
             TechTemplates.Add(new WeaponsUpgrade2());

        }

        public Location MousePosition { get; set; }

        public Color Color
        {
            get
            {
                switch (Up)
                {
                    case 1:
                        return Color.Blue;
                    case 2:
                        return Color.Red;
                    default:
                        return Color.White;
                }
            }
        }
        public Player(Arena MyArena,int Up):this()
        {
            this.MyArena = MyArena;
            this.Up = Up;
            switch (Up)
            {
                case 1:
                    TargetLocation = new Location() { Grid = new Vector2(MyArena.Bounds.Grid.Right, 0) };
                    break;
                case 2:
                    TargetLocation =  new Location() { Grid = new Vector2(MyArena.Bounds.Grid.Left, 0) };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private Player _Enemy;
        public Player Enemy {
            get
            {
                foreach (Player item in MyArena.PlayerList)
                {
                    if(item.Up!=this.Up)
                    {
                        _Enemy = item;
                        break;
                    }
                }
                return _Enemy;
            }
        }
        public int Up { get; private set; }
        public Vector DefaultVelocity { get; private set; }
        public Location TargetLocation { get; private set; }

        public int UnitsPassed { get; set; }

        public int Cash { get; set; }
        public List<String> Modifiers { get; set; }
        public List<Factory> BuildingTemplates { get; set; }

        public List<Tech> TechTemplates { get; set; }

        public Building SelectedBuilding { get; set; }

        public override void Draw()
        {
            int wheelval = MouseProvider.Current.MouseState.ScrollWheelValue;
            //g.Draw(Bounds.Screen, Color.White);
            string mouse_pos_debug = string.Format("X: {0}, Y: {1}, W: {2}, S:{3}", MousePosition.Screen.X, MousePosition.Screen.Y, wheelval, MouseProvider.Current.ScrollValue);
            string bounds_debug = string.Format("X: {0}, Y: {1}, Width: {2}, Height: {3}", Bounds.Screen.X, Bounds.Screen.Y, Bounds.Screen.Width, Bounds.Screen.Height);
            GraphicsProvider.Current.DrawString(mouse_pos_debug, new Vector2(20, 20));

            GraphicsProvider.Current.DrawString(bounds_debug, new Vector2(20, 50));    
        }
        public override void Update(Microsoft.Xna.Framework.Input.MouseState ms) 
        {

            MousePosition = new Location() { Screen = new Vector2() { X = ms.X, Y = ms.Y } };

            Location Location = new Location() { Grid = new Vector2(MousePosition.Grid.X, MousePosition.Grid.Y) };

            Bounds = new Area() { World = new Rectangle((int)Location.World.X, (int)Location.World.Y, (int)Location.World.X + Constants.TileSize, (int)Location.World.Y + Constants.TileSize) };

            base.Update(ms);
        }
        public override void OnClick(MouseState ms)
        {
            Location mouse = new Location() { Screen = new Vector2(ms.X,ms.Y) };
            Factory b = (Factory)BuildingTemplates[0].Clone();
            Location l = new Location() { Grid = mouse.Grid };

            if (MyArena.PlayerList[0].BuildingArea.Screen.Contains((int)mouse.Screen.X, (int)mouse.Screen.Y))
                {
                    if (MyArena.AllItems.Count(a => a.Location.Grid == mouse.Grid)==0)
                    {
                        if (MyArena.PlayerList[0].Cash >= b.Cost)
                        {
                            b.Location = l;
                            b.Owner = MyArena.PlayerList[0];
                            b.Radius = b.Product.Radius;
                            b.HitPoints = 1000;
                            MyArena.PlayerList[0].Cash -= b.Cost;
                            MyArena.AllItems.Add(b);
                        }
                    }
                }

        }
        float Radius = 1F;

        public Arenas.Arena MyArena { get; set; }
        public Area BuildingArea {
            get
            {
                Area Result;
                switch (Up)
                {
                    case 1:
                        Result = new Area() { Grid = new Rectangle() { X = -Constants.GRID_HORIZONTAL_OFFSET, Y = -Constants.GRID_VERTICAL_OFFSET, Width = Constants.PlayerAreaGridWidth, Height = Constants.GRID_VERTICAL_OFFSET * 2 } };
                        break;
                    case 2:
                        Result = new Area() { Grid = new Rectangle() { X = Constants.GRID_HORIZONTAL_OFFSET - Constants.PlayerAreaGridWidth, Y = -Constants.GRID_VERTICAL_OFFSET, Width = Constants.PlayerAreaGridWidth, Height = Constants.GRID_VERTICAL_OFFSET * 2 } };
                        break;
                    default:
                        throw new ArgumentException();
                }
                return Result;
            }
        }
        public Area HomeArea
        {
            get
            {
                Area Result;
                switch (Up)
                {
                    case 1:
                        Result = new Area() { Grid = new Rectangle() { X = -Constants.GRID_HORIZONTAL_OFFSET, Y = -Constants.GRID_VERTICAL_OFFSET, Width = Constants.GRID_HORIZONTAL_OFFSET, Height = Constants.GRID_VERTICAL_OFFSET * 2 } };
                        break;
                    case 2:
                        Result = new Area() { Grid = new Rectangle() { X = 0, Y = -Constants.GRID_VERTICAL_OFFSET, Width = Constants.PlayerAreaGridWidth, Height = (int)( Constants.GRID_VERTICAL_OFFSET / Constants.TileSize )} };
                        break;
                    default:
                        throw new ArgumentException();
                }
                return Result;
            }
        }
        public Area EnemyArea
        {
            get
            {
                Area Result;
                switch (Up)
                {
                    case 1:
                        Result = new Area() { Grid = new Rectangle() { X = 0, Y = -Constants.GRID_VERTICAL_OFFSET, Width = Constants.GRID_HORIZONTAL_OFFSET, Height = Constants.GRID_VERTICAL_OFFSET * 2 } };
                        break;
                    case 2:
                        Result = new Area() { Grid = new Rectangle() { X = -Constants.GRID_HORIZONTAL_OFFSET, Y = -Constants.GRID_VERTICAL_OFFSET, Width = Constants.GRID_HORIZONTAL_OFFSET, Height = Constants.GRID_VERTICAL_OFFSET * 2 } };
                        break;
                    default:
                        throw new ArgumentException();
                }
                return Result;
            }
        }
    }
}
