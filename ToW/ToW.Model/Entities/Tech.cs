using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Entities
{
    public class Tech
    {

        public Tech(string Modifier)
        {
            this.Cost = 100;
            ModifierKey = Modifier;
        }

        public int Cost { get; set; }

        public virtual bool IsAvailable(Player p)
        {
            if (p.Cash - this.Cost < 0)
            {
                return false;
            }

            if (p.Modifiers.Contains(this.ModifierKey))
            {
                return false;
            }

            return true;

            //todo: check modifiers
        }

        public virtual void Apply(Player p)
        {
            p.Modifiers.Add(ModifierKey);
        }

        public string ModifierKey { get; set; }

    }
    public class WeaponsUpgrade:Tech
    {
        public WeaponsUpgrade():base("wpn1")
        {

        }

        public override bool IsAvailable(Player p)
        {
           return base.IsAvailable(p);
        }

        public override void Apply(Player p)
        {
            base.Apply(p);
            var ExtantBuildings = p.MyArena.AllItems.Where(x => x is Factory).Cast<Factory>().Where(x => x.Owner.Up == p.Up).ToList();
            ExtantBuildings.AddRange(p.BuildingTemplates);

            foreach (var item in ExtantBuildings)
            {
                item.Product.Damage += item.Product.Damage / 10;
            }
        }
    }

    public class WeaponsUpgrade2 : Tech
    {

        public WeaponsUpgrade2():base("wpn2")
        {

        }

        public override bool IsAvailable(Player p)
        {
            return base.IsAvailable(p);
            //check p's modifiers
        }

        public override void Apply(Player p)
        {
            base.Apply(p);
            var ExtantBuildings = p.MyArena.AllItems.Where(x => x is Factory).Cast<Factory>().Where(x => x.Owner.Up == p.Up).ToList();
            ExtantBuildings.AddRange(p.BuildingTemplates);

            foreach (var item in ExtantBuildings)
            {
                item.Product.Damage += item.Product.Damage / 10;
            }
        }
    }



}
