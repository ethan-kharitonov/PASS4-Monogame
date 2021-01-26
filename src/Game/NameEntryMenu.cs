using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    static class NameEntryMenu
    {
        private static string name = string.Empty;
        private const int MAX_NAME_LENGTH = 25;

        private static Texture2D nameEntryBg;
        private static Rectangle nameEntryBox = new Rectangle(Main.WIDTH / 2 - 150, Main.HEIGHT / 2 - 50, 300, 80);

        private static Texture2D bgImg;
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);
        public static void LoadContent()
        {
            nameEntryBg = Helper.LoadImage("Images/panel_brown");
            bgImg = Helper.LoadImage("Images/MenuBg");
        }

        public static void Update()
        {
            name = Helper.UpdateStringWithInput(name, MAX_NAME_LENGTH);
        }

        public static void Draw()
        {
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);
            Helper.SpriteBatch.Draw(nameEntryBg, nameEntryBox, Color.White);
            Helper.SpriteBatch.DrawString(Helper.InputFont, name, nameEntryBox.Center.ToVector2() - (Helper.InputFont.MeasureString(name) * 0.5f), Color.White);
        }
    }
}
