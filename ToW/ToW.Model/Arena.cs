using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToW.Model.Providers;

namespace ToW.Model
{
    public class Arenaasd
    {
        public void Draw()
        {
            GraphicsProvider.Current.DrawString("dnm", new Vector2(20, 20));
            int i = 0;
            foreach (var spriteSheet in GraphicsProvider.Current.SpriteSheetList.Values)
            {
                i++;
                spriteSheet.Draw("WalkLeft", new Vector2(100 * i, 100));
                spriteSheet.Draw("WalkRight", new Vector2(100 * i, 200));
                spriteSheet.Draw("AttackLeft", new Vector2(100 * i, 300));
                spriteSheet.Draw("AttackRight", new Vector2(100 * i, 400));
            }
        }
    }
}
