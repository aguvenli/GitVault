
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Model.Entities.Helpers;
using Model.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ToW.Model.Providers;

namespace Model.Entities
{
    #region Abilities and such

    public enum ArmorType
    {
        Plate,
        Leather,
        Cloth
    }
    public abstract class Ability
    {
        public int Cooldown = 10;

        public int Duration = 10;

        public bool IsActive = false;

        public int MaxCooldown { get; set; }

        public int MaxDuration { get; set; }

        public string Name { get; set; }
        public Unit Owner { get; set; }
        public abstract void AddEffect();

        public abstract void RemoveEffect();

        public void Update()
        {
            if (Cooldown <= 0)
            {
                List<Unit> EnemyList=Owner.GetEnemyUnitsInSightRange();
                if (EnemyList.Count>0)
                {
                    IsActive = true;
                    Cooldown = MaxCooldown;
                    Duration = MaxDuration;
                    AddEffect();
                }
            }

            if (!IsActive)
            {
                Cooldown--;
            }
            else
            {
                Duration--;
            }
            if (Duration <= 0)
            {
                IsActive = false;
                RemoveEffect(); 
                Duration = MaxDuration;
            }
        }
    }
    public class ChargeAbility:Ability
    {
        public override void AddEffect()
        {
            Owner.MaxSpeed *= 2;
        }
        public override void RemoveEffect()
        {
            Owner.MaxSpeed /= 2F;
        }
    }
    #endregion

    public class Unit : MapItem
    {
        #region Properties
        public Vector DebugTarget { get; set; }
        public Vector DebugSeek { get; set; }
        public string DEBUG_INFO_ClosestCollision { get; set; }
        public int Id { get; set; }

        public Unit CurrentEnemy;

        public Vector Desired;

        public bool isObstacleCourse = false;

        public Location TargetLocation;

        private bool Attacking = false;

        public Unit()
        {
            WeaponRange = 0.4F;
            Damage = 3;
            HitPoints = 200;
            TargetLocation.World = new Vector2(0,0);

            MaxSpeed = 0.022F;
            Velocity = new Vector(0, 0);
            AbilityList = new List<Ability>();
            Id = int.MinValue;
        }

        public List<Ability> AbilityList { get; set; }

        public Vector Acceleration { get; set; }

        public ArmorType ArmorType { get; set; }

        public Vector AvoidingToNormal { get; set; }

        public Vector AvoidingCollisions { get; set; }

        public float Damage { get; set; }

        public float DamageAgainstNonPlateRatio { get; set; }

        public float DamageAgainstPlateRatio { get; set; }

        public Vector EvasionForce { get; set; }

        public string FriendlyName { get; set; }
        public float MaxAcceleration { get; set; }

        //public Location World;
        public float MaxSpeed { get; set; }

        public string Name { get; set; }

        public Vector PreviousAcceleration { get; set; }
        public Vector SeekingForce { get; set; }

        public float SightRange { get; set; }

        public float WeaponRange { get; set; }
        #endregion

        #region Behaviour
        public void Act()
        {
            foreach (var Ability in AbilityList)
            {
                Ability.Update();
            }

            if (isObstacleCourse && this.Owner.Up == 2)
            {
                return;
            }
            //outputs acceleration
            Acceleration = new Vector();

            Attacking = false;
            var EnemyUnitsInSightRange = GetEnemyUnitsInSightRange();

            if (isObstacleCourse || EnemyUnitsInSightRange.Count == 0)
            {
                //this.Velocity = this.Owner.DefaultVelocity;
                this.TargetLocation.World.X = this.Owner.TargetLocation.World.X;
                this.TargetLocation.World.Y = this.Location.World.Y;
                //this.TargetLocation.World.Y = float.Epsilon;
            }
            else
            {
                var closestEnemy = EnemyUnitsInSightRange.OrderBy(a => Distance(a)).Where(a => a.HitPoints > 0).FirstOrDefault();
                if (closestEnemy != null)
                {
                    if (Distance(closestEnemy) <= this.WeaponRange)
                    {
                        CurrentEnemy = closestEnemy;

                        //Acceleration = new Vector(0,0); //S T O P!
                        //Velocity = new Vector(0,0); //we will need to change this later; we do not deal with velocity
                        Attacking = true;
                    }
                    else
                    {
                        TargetLocation.World = closestEnemy.Location.World;
                    }
                }
            }

            //double angle = Math.Atan2(TargetLocation.World.Y - this.Location.World.Y, TargetLocation.World.X - this.Location.World.X);
            SeekingForce = Seek(TargetLocation.ToVector());

            if (!Attacking)
            {
                //EvasionForce = Evade();
                //AvoidingCollisions = AvoidCollisions();
            }
            else
            {
                //EvasionForce = new Vector(0, 0);
                //AvoidingCollisions = AvoidCollisions();
            }

            SeekingForce = SeekingForce.Normalize2();
            SeekingForce = SeekingForce.Mult2(MaxAcceleration);

            Vector IntendedVelocity = GetIntendedVelocity(SeekingForce);

            //Vector IntendedVelocity = TargetLocation.World.ToVector();
            IntendedVelocity.Normalize();
            IntendedVelocity.Mult(MaxSpeed);

            AvoidingCollisions = AvoidCollisions(IntendedVelocity);

            AvoidingCollisions = AvoidingCollisions.Normalize2();
            AvoidingCollisions = AvoidingCollisions.Mult2(MaxAcceleration);

            //without evasion
            Acceleration = SeekingForce;
            if (AvoidingCollisions.Mag() > 0)
            {
                Acceleration = Acceleration.Div2(1F);
                Acceleration = Acceleration.Add2(AvoidingCollisions);
            }

        }

        public enum Side
        {
            Left,
            Top,
            Right,
            Bottom,
        }

        public class SideDef
        {
            public Vector2 MidVector2 { get; set; }
            public Side Side { get; set; }
        }

        public Vector2 GetIntersectionVector2(Line Line1, Line Line2)
        {
            float det = Line1.A * Line2.B - Line2.A * Line1.B;

            if (det == 0)
                return new Vector2(float.MaxValue,float.MaxValue);
            
            float X = (Line2.B * Line1.C - Line1.B * Line2.C) / det;
            float Y = (Line1.A * Line2.C - Line2.A * Line1.C) / det;

            return new Vector2() { X = X, Y = Y };
        }

        public float min(float x, float y)
        {
            if (x < y) return x;
            return y;
        }

        public float max(float x, float y)
        {
            if (x > y) return x;
            return y;
        }

        private bool VerifyIntersection(Line Line, Vector2 Vector2)
        {
            float Tolerance = 0.1F;

            bool c1 = min(Line.X1, Line.X2) - Tolerance <= Vector2.X && Vector2.X <= max(Line.X1, Line.X2) + Tolerance;
            bool c2 = min(Line.Y1, Line.Y2) - Tolerance <= Vector2.Y && Vector2.X <= max(Line.Y1, Line.Y2) + Tolerance;

            return c1 && c2;
        }


       


        public Vector GetNormal(Line LineFrom)
        {
            float diffX = LineFrom.X2 - LineFrom.X1;
            float diffY = LineFrom.Y2 - LineFrom.Y1;

            Vector FirstNormal = new Vector() { X = -diffY, Y = diffX };
            Vector SecondNormal = new Vector() { X = diffY, Y = -diffX };

            float FirstDistance = Distance(this.Location.ToVector(), FirstNormal);
            float SecondDistance = Distance(this.Location.ToVector(), SecondNormal);

            FirstNormal.Normalize();
            FirstNormal.Mult(0.2F);

            SecondNormal.Normalize();
            SecondNormal.Mult(0.2F);

            

            if (FirstDistance < SecondDistance)
            {
                return FirstNormal;
            }
            else
            {
                return SecondNormal;
            }
        }

        public List<Line> Rays { get; set; }
        public Vector DesiredDebug { get; set; }

        public class RayInfo
        {
            public RayInfo()
            {
                CollisionDistance = float.MinValue;
                HasCollision = false;
            }

            public Line Line { get; set; }
            public bool HasCollision { get; set; }
            public Unit CollidingUnit { get; set; }
            public float CollisionDistance { get; set; }
            public double Bearing { get; set; }
        }

        public Vector AvoidCollisions(Vector IntendedVelocity)
        {
            //store the target that collides then, and other stuff
            //that we will need and can avoid recalculating
            //store the first collision time
            float ShortestTime = float.NegativeInfinity;
            Unit FirstTarget = null;
            float FirstMinSeperation = 0;
            float FirstDistance = 0;
            Vector FirstRelativePos = new Vector();
            Vector FirstRelativeVel = new Vector();

            var UnitsInRange = GetAllUnitsInAvoidanceRange();
            float Distance = 0;

            Vector RelativePos = new Vector();

            float CollisionRadiusToAvoid = 0F;

            foreach (var unit in UnitsInRange)
            {
                //calc min coll. range
                float CollisionRadius = (float)this.Radius + (float)unit.Radius;


                //Calculate time to collision
                RelativePos = unit.Location.ToVector().Subtract(this.Location.ToVector());
                Distance = RelativePos.Len();

                Vector RelativeVel = unit.Velocity.Subtract(this.Velocity);

                //Vector RelativeVel = unit.Velocity.Subtract(IntendedVelocity);

                float RelativeSpeed = RelativeVel.Mag();
                float TimeToCollision = VectorOps.dot(RelativePos, RelativeVel) / (RelativeSpeed * RelativeSpeed);
                TimeToCollision = -TimeToCollision;

                if (TimeToCollision == double.NaN || TimeToCollision > 10 || TimeToCollision < 0)
                {
                    continue;
                }

                if (ShortestTime < TimeToCollision)
                    ShortestTime = TimeToCollision;

                //Check if it is going to be a collision at all

                //var MyPositionAtCollision = (this.Location.World.ToVector()).Add2(this.Velocity.Mult2(TimeToCollision));
                var MyPositionAtCollision = (this.Location.World.ToVector()).Add2(IntendedVelocity.Mult2(TimeToCollision));
                var HisPositionAtCollision = (unit.Location.World.ToVector()).Add2(unit.Velocity.Mult2(TimeToCollision));

                float MinSeperation = (MyPositionAtCollision.Subtract(HisPositionAtCollision)).Len();
                //float MinSeperation = Distance - RelativeSpeed * ShortestTime;



                if (MinSeperation > CollisionRadius) continue; //skip to the next iteration

                if (TimeToCollision > 0 && TimeToCollision <= ShortestTime)
                {
                    ShortestTime = TimeToCollision;
                    FirstTarget = unit;
                    FirstMinSeperation = MinSeperation;
                    FirstDistance = Distance;
                    FirstRelativePos = RelativePos;
                    FirstRelativeVel = RelativeVel;
                    CollisionRadiusToAvoid = CollisionRadius;
                }
            }

            if (FirstTarget == null) return new Vector(); //if we have no target, exit

            //if we're going to hit, or if we're hitting already, then do steering based on current pos
            if (FirstMinSeperation <= 0 || Distance < CollisionRadiusToAvoid)
            {
                RelativePos = FirstTarget.Location.ToVector().Subtract(this.Location.ToVector());
            }
            else
            {
                Vector vMultipliedFirstRelativeVel = FirstRelativeVel.Mult2(ShortestTime);

                RelativePos = FirstRelativePos.Add2(vMultipliedFirstRelativeVel);
            }
            RelativePos.Normalize();
            RelativePos.Mult(MaxSpeed);

            RelativePos.Y = -RelativePos.Y;
            RelativePos.X = -RelativePos.X;

            //float dot = VectorOps.dot(RelativePos.Normalize2(), Velocity.Normalize2());
            //if (dot >= 0.01 || dot <= -0.01) //means our velocity is close to perpendicular to collision line
            ////if (true)
            //{
            //    //todo: consider the special case of midVector2
            //    //otherwise the target is to my right
            //    RelativePos = this.Location.World.ToVector().Add2(GetNormal(Velocity.ToLine()));
            //}

            return RelativePos;
            //apply steering


        }

        public Vector GetIntendedVelocity(Vector Force)
        {
            if (!Attacking)
            {
                Vector ExagerratedAcceleration = Acceleration.Mult2(1);
                Velocity = Velocity.Add2(ExagerratedAcceleration);
                Velocity = Velocity.Limit(MaxSpeed);

                return Velocity;
            }
            else
            {
                return new Vector(0, 0);
            }
        }

        public override void Update()
        {
            Act();
            if (!Attacking)
            {
                Velocity = Velocity.Add2(Acceleration);
                Velocity = Velocity.Limit(MaxSpeed);

                Vector vn = this.Location.ToVector();
                vn.Add(Velocity);
               
                Location.FromVector(vn);

                PreviousAcceleration = Acceleration;
                Acceleration = Acceleration.Mult2(0);
            }

            if (Attacking)
            {
                Velocity = new Vector(0, 0);
                float DamageRatio = 0;
                if (CurrentEnemy.ArmorType == Entities.ArmorType.Plate)
                {
                    DamageRatio = this.DamageAgainstPlateRatio;
                }
                else
                {
                    DamageRatio = this.DamageAgainstNonPlateRatio;
                }

                CurrentEnemy.HitPoints -= (int)(this.Damage * DamageRatio);

                if (CurrentEnemy.HitPoints <= 0)
                {
                    CurrentEnemy = null;
                    Attacking = false;
                }
            }

            var UnitsThatIMightHit = MyArena.AllUnits.Where(x => x.Distance(x) <= (this.Radius + x.Radius) * 2F).ToList();
            UnitsThatIMightHit.Remove(this);

            foreach (var unit in UnitsThatIMightHit)
            {
                if (Distance(this.Location.ToVector(), unit.Location.ToVector()) < this.Radius + unit.Radius)
                {
                    Vector v = unit.Location.ToVector().Subtract(this.Location.ToVector());
                    var nDistance = Distance(this.Location.ToVector(), unit.Location.ToVector());
                    var Overlap = (this.Radius + unit.Radius) - nDistance;

                    v.Normalize();
                    v.Mult((float)Overlap / 2);

                    this.Location.FromVector(this.Location.ToVector().Subtract(v));
                    unit.Location.FromVector(unit.Location.ToVector().Add2(v));
                }

            }
        }

        #endregion

        #region Other
        public void ApplyForce(Vector force)
        {
            force.Limit(MaxAcceleration);
            Acceleration = Acceleration.Add2(force);
        }



        public object Clone()
        {
            Unit tmp = new Unit();
            
            ///TODO:Clone here
            tmp.Name = this.Name;
            tmp.Owner = this.Owner;
            tmp.TargetLocation= this.TargetLocation;
            tmp.Acceleration= this.Acceleration;
            tmp.ArmorType = this.ArmorType;
            tmp.Location = this.Location;
            tmp.MaxAcceleration = this.MaxAcceleration;
            tmp.MaxSpeed = this.MaxSpeed;
            tmp.MyArena = this.MyArena;
            tmp.SightRange = this.SightRange;
            tmp.Radius = this.Radius;
            tmp.SoftRadius = this.SoftRadius;
            tmp.WeaponRange = this.WeaponRange;
            tmp.Damage = this.Damage;
            tmp.DamageAgainstNonPlateRatio = this.DamageAgainstNonPlateRatio;
            tmp.DamageAgainstPlateRatio = this.DamageAgainstPlateRatio;

            tmp.AbilityList = new List<Ability>(this.AbilityList);

            return tmp;
        }

        public float Distance(Unit to)
        {
            return Arenas.Arena.GetDistance(this.Location.World, to.Location.World);
        }

        public float Distance(Vector From, Vector To)
        {
            return Arenas.Arena.GetDistance(From.ToVector2(), To.ToVector2());
        }

        public Vector Evade()
        {
            var UnitsInRange = GetAllUnitsInCollisionRange();
            Vector RelativePos = new Vector(0,0);
            //take immediate action away from closest unit
            if (UnitsInRange.Count() > 0)
            {
                var UnitToAvoid = UnitsInRange.OrderBy(x => x.Distance(x)).FirstOrDefault();

                RelativePos =  this.Location.ToVector().Subtract(UnitToAvoid.Location.ToVector());


                RelativePos.Normalize();

                //RelativePos = RelativePos.Perp(RelativePos);

                RelativePos.Mult(MaxSpeed);

              
            }
            else
            {
                return RelativePos;
            }
            return RelativePos;
            //apply steering
        }     
        //public Vector Evade()
        //{
        //    var units = GetUnitsInCollisionRange();
        //    Vector sum = new Vector(0, 0);
        //    int count = 0;
        //    foreach (var unit in units)
        //    {
        //        Vector diff = this.Location.ToVector().Subtract(unit.Location.ToVector());
        //        diff.Normalize();
        //        sum.Add(diff);
        //        count++;
        //    }

        //    Vector steer = new Vector(0, 0);
        //    if (count > 0)
        //    {
        //        sum.Div(count);
        //        sum.Normalize();
        //        sum.Mult(MaxSpeed);
        //        steer = sum.Subtract(Velocity);
        //    }

        //    return steer;
        //}

        public List<Unit> GetEnemyUnitsInSightRange()
        {
            return MyArena.AllUnits.Where(x => x.Owner.Up != this.Owner.Up && Arenas.Arena.GetDistance(this.Location.World, x.Location.World) <= this.SightRange).ToList();
        }

        public void GetOutofMyWayBitch(Unit u)
        {
            float d = Distance(u);
            float rSum = (float)(u.Radius + this.Radius);
            float push = (rSum - d);
            double angle = Math.Atan2(u.Location.World.Y - this.Location.World.Y, u.Location.World.X - this.Location.World.X);

            float pushX = (float)Math.Cos(angle) * push;
            float pushY = (float)Math.Sin(angle) * push;

            if (Attacking && !u.Attacking)
            {
                this.Location.World.X -= 50 * pushX / 100;
                this.Location.World.Y -= 50 * pushY / 100;
                u.Location.World.X += 50 * pushX / 100;
                u.Location.World.Y += 50 * pushY / 100;
            }
            else
            {
                this.Location.World.X -= pushX / 2;
                this.Location.World.Y -= pushY / 2;
                u.Location.World.X += pushX / 2;
                u.Location.World.Y += pushY / 2;
            }
        }

        public override void Draw()
        {
            string UnitSpriteName= "knight";

            if (!string.IsNullOrEmpty(Name))
            {
                UnitSpriteName = Name.ToLower();
            }

            string ActionString = "Walk";
            string DirectionString = "Left";

            if (TargetLocation.World.X>Location.World.X)
            {
                DirectionString = "Right";
            }
            if (Attacking)
            {
                ActionString = "Attack";
            }

            GraphicsProvider.Current.DrawSpriteSheet(UnitSpriteName, ActionString + DirectionString, Bounds.Screen.Location);
        }       


        public Vector Seek(Vector target)
        {
            DebugTarget = target;

            Desired = target.Subtract(Location.ToVector());
            Desired.Normalize();
            Desired.Mult(MaxSpeed);
            Vector steer = Desired.Subtract(Velocity);

            DebugSeek = steer;
            return steer;
        }
        
        

        
        private List<Unit> GetAllUnitsInRayDetectionRange()
        {
            float AvoidanceThreshold = 1F;

            var copy = MyArena.AllUnits.ToList();
            copy.Remove(this);
            var ret = copy.Where(x => this.Distance(x) < AvoidanceThreshold && x.Velocity.X == 0 && x.Velocity.Y == 0).ToList();
            return ret;
        }

        private List<Unit> GetAllUnitsInAvoidanceRange()
        {
            float AvoidanceThreshold = Constants.AvoidanceThreshold;

            var copy = MyArena.AllUnits.ToList();
            copy.Remove(this);
            var ret = copy.Where(x => this.Distance(x) < AvoidanceThreshold &&  x.Owner.Up == this.Owner.Up).ToList();
            return ret;
        }

        private List<Unit> GetAllUnitsInCollisionRange()
        {
            var copy = MyArena.AllUnits.ToList();
            copy.Remove(this);
            var ret = copy.Where(x => this.Distance(x) < this.SoftRadius + x.Radius && x.Owner.Up == this.Owner.Up).ToList();
            return ret;
        }
        private List<Unit> GetUnitsInAvoidanceRange()
        {
            float AvoidanceThreshold = 0.5F;

            var copy = MyArena.AllUnits.ToList();
            copy.Remove(this);
            var ret = copy.Where(x => x.Owner.Up == this.Owner.Up && this.Distance(x) < AvoidanceThreshold).ToList();
            return ret;
        }

        private List<Unit> GetUnitsInCollisionRange()
        {
            var copy = MyArena.AllUnits.ToList();
            copy.Remove(this);
            var ret = copy.Where(x => x.Owner.Up == this.Owner.Up && this.Distance(x) < this.Radius + x.Radius).ToList();
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AvoidEnemies">If false, avoids only friendlies. If true, avoids both.</param>
        /// <returns></returns>
        private List<Unit> AvoidToNormalCandidates(bool AvoidEnemies = false)
        {
            var copy = MyArena.AllUnits.Where(x => x.Velocity.X == 0 && x.Velocity.Y == 0).ToList();
            if (AvoidEnemies)
            {
                copy = copy.ToList();
            }
            else
            {
                copy = copy.Where(x => x.Owner == this.Owner).ToList();
            }
            
            
            copy.Remove(this);
            return copy;
        }

        #endregion
    }
}