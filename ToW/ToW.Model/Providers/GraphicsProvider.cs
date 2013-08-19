using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Model.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToW.Model.Graphics;

namespace ToW.Model.Providers
{
    public class GraphicsProvider:BaseProvider<GraphicsProvider>
    {
        public SpriteBatch SpriteBatch { get; set; }
        public SpriteFont Font { get; set; }
        public Texture2D Button { get; set; }
        public Texture2D Background { get; set; }

        public Texture2D Flat;
        public Dictionary<string,SpriteSheet> SpriteSheetList;
        public string[] SpriteSheetNameList = new string[] { 
            "swordsmen", "ninja", "robot", "wizard", "skullchick",
            "horseman", "infantry", "mage", "spearman", "archer"
        };
        public void Initialize()
        {
            SpriteSheetList = new Dictionary<string, SpriteSheet>();
            Flat = new Texture2D(SpriteBatch.GraphicsDevice, 1, 1);
            Flat.SetData(new Color[]{Color.White});
            Game.IsMouseVisible = true;
            Font = Game.Content.Load<SpriteFont>("font\\game");
            Button = Game.Content.Load<Texture2D>("sprites\\button");
            Background = Game.Content.Load<Texture2D>("sprites\\background");
            foreach (var SpriteSheetName in SpriteSheetNameList)
            {
                var TempSpriteSheet = new SpriteSheet();
                TempSpriteSheet.Texture = Game.Content.Load<Texture2D>("SpriteSheets\\" + SpriteSheetName);
                //HACK
                if (SpriteSheetName.Equals("swordsmen"))
                {
                    TempSpriteSheet.ColumnCount = 40;
                    TempSpriteSheet.RowCount = 6;
                    TempSpriteSheet.LoadFrameSet("WalkLeft", 80, 12);
                    TempSpriteSheet.LoadFrameSet("WalkRight", 200, 12);
                    TempSpriteSheet.LoadFrameSet("AttackLeft", 0, 32);
                    TempSpriteSheet.LoadFrameSet("AttackRight", 120, 32);
                }
                else
                {
                    TempSpriteSheet.ColumnCount = 8;
                    TempSpriteSheet.RowCount = 5;
                    TempSpriteSheet.LoadFrameSet("WalkLeft", 8, 8);
                    TempSpriteSheet.LoadFrameSet("WalkRight", 16, 8);
                    TempSpriteSheet.LoadFrameSet("AttackLeft", 24, 4);
                    TempSpriteSheet.LoadFrameSet("AttackRight", 28, 4);
                }
                //HACK
                
                SpriteSheetList[SpriteSheetName] = TempSpriteSheet;
            }
        }

        public void DrawBackground(Texture2D texture, Rectangle rect)
        {

            SpriteBatch.Draw(texture, rect, Color.White);
        }
        public void Draw(Texture2D texture, Rectangle rect, Rectangle Clip)
        {
            SpriteBatch.Draw(texture, rect, Clip, Color.White);
        }
        public void Draw(Texture2D texture, Rectangle rect)
        {
            SpriteBatch.Draw(texture, rect, Color.White);
        }
        public void DrawButton(Rectangle rect,string Text)
         {
            SpriteBatch.Draw(Button, rect, Color.White);
            Vector2 p = new Vector2() { X = rect.Location.X, Y = rect.Location.Y };
            DrawString(Text, p);
        }
        public void DrawString(string text, Vector2 Point)
        {
            SpriteBatch.DrawString(Font, text, Point, Color.White);
        }
    
        public void DrawSpriteSheet(string Name, string FrameSetName, Point Position)
        {
            Name = "swordsmen";
            Vector2 vec = new Vector2() { X = Position.X - MouseProvider.Current.ScrollValue, Y = Position.Y };
            SpriteSheetList[Name].Draw(FrameSetName, vec, Name == "swordsmen");
        }

        public void FillRectangle(Rectangle rect, Color color)
        {
            SpriteBatch.Draw(Flat, rect, color);
        }
    }
}
