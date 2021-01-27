using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    static class NameEntryMenu
    {
        public static Action InputComplete;

        private static string name = string.Empty;
        private const int MAX_NAME_LENGTH = 25;

        private static Texture2D nameEntryBg;
        private static Rectangle nameEntryBox = new Rectangle(Main.WIDTH / 2 - 150, Main.HEIGHT / 2 - 50, 300, 0);

        private static Texture2D bgImg;
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

        private static Button menuButton;

        private static Screen ResultDisplay = new Screen(new Point(nameEntryBox.X - 65, nameEntryBox.Y - 15));
        private static Point margins = new Point(10);
        private static int lineSpacing = 10;
        public static void LoadContent()
        {
            nameEntryBg = Helper.LoadImage("Images/NameEntryPanel");
            float neScaleFactor = 2f;
            int neWidth = (int)Math.Round(nameEntryBg.Width * neScaleFactor);
            int neHeight = (int)Math.Round(nameEntryBg.Height * neScaleFactor);

            nameEntryBox = new Rectangle(Main.WIDTH / 2 - neWidth/2, Main.HEIGHT / 2 - neHeight/2 - 10, neWidth, neHeight) ;

            bgImg = Helper.LoadImage("Images/MenuBackground");

            menuButton = new Button(new Rectangle(nameEntryBox.Left, nameEntryBox.Bottom + 25, nameEntryBox.Width, 100), () => InputComplete.Invoke());
        }

        public static void Update()
        {
            name = Helper.UpdateStringWithInput(name);
            name = Helper.TrimString(name, MAX_NAME_LENGTH);

            menuButton.Update();
        }

        public static void Draw()
        {
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);
            Helper.SpriteBatch.Draw(nameEntryBg, nameEntryBox, Color.White);
            Helper.SpriteBatch.DrawString(Helper.InputFont, name, nameEntryBox.Center.ToVector2() - new Vector2(0, 84) - (Helper.InputFont.MeasureString(name) * 0.5f), Color.White);

            DrawOnLine($"Total Score: {Game.FinalGameResult.TotalScore}", 0);
            DrawOnLine($"Total Time: {Game.FinalGameResult.TotalTime}", 0, false);

            for (int i = 0; i < 2; ++i)
            {
                DrawOnLine($"Level {i}) Score: {Game.FinalGameResult.LevelResults[i].Score}", i + 1);
                DrawOnLine($"Time: {Game.FinalGameResult.LevelResults[i].Time}", i + 1, false);

            }


            menuButton.Draw();
        }

        private static void DrawOnLine(string text, int lineNum, bool leftToRight = true)
           => ResultDisplay.DrawText(Helper.InputFont, text, new Vector2(leftToRight ? margins.X : nameEntryBox.Width - margins.X - Helper.InputFont.MeasureString(text).X, (Helper.InputFont.MeasureString("S").Y + lineSpacing) * lineNum + margins.Y), Color.White);
    }
}
