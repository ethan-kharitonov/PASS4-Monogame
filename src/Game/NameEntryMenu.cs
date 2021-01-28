using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace PASS4
{
    static class NameEntryMenu
    {
        public static Action InputComplete;

        private static string name = string.Empty;
        private const int MAX_NAME_LENGTH = 25;

        private static Texture2D nameEntryBg;
        private static Rectangle nameEntryBox = new Rectangle(Main.WIDTH / 2 - 150, Main.HEIGHT / 2 - 50, 300, 80);

        private static Texture2D bgImg;
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

        private static Button menuButton;

        private static int score = 123;

        private const string SCORE_FILE_PATH = "../../../NamesAndScores.txt";

        public static void LoadContent()
        {
            nameEntryBg = Helper.LoadImage("Images/NameEntryPanel");
            float neScaleFactor = 2f;
            int neWidth = (int)Math.Round(nameEntryBg.Width * neScaleFactor);
            int neHeight = (int)Math.Round(nameEntryBg.Height * neScaleFactor);

            nameEntryBox = new Rectangle(Main.WIDTH / 2 - neWidth/2, Main.HEIGHT / 2 - neHeight/2 - 10, neWidth, neHeight) ;

            bgImg = Helper.LoadImage("Images/MenuBackground");

            menuButton = new Button(new Rectangle(nameEntryBox.Center.X - (nameEntryBox.Width - 14)/2, nameEntryBox.Top + 130, nameEntryBox.Width - 14, 100), OnButtonClick, "SAVE AND GO TO MENU");
        }

        public static void Start(int score)
        {
            NameEntryMenu.score = score;
            name = string.Empty;
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

            Helper.SpriteBatch.DrawString(Helper.InputFont, $"Score:", new Vector2(nameEntryBox.Left + 15, nameEntryBox.Top + 90), Color.White);
            Helper.SpriteBatch.DrawString(Helper.InputFont, score.ToString(), new Vector2(nameEntryBox.Right - Helper.InputFont.MeasureString(score.ToString()).X - 15, nameEntryBox.Top + 90), Color.White);


            menuButton.Draw();
        }

        private static void OnButtonClick()
        {
            InputComplete.Invoke();
            StreamWriter outFile = File.AppendText(SCORE_FILE_PATH);
            outFile.WriteLine($"{name},{score}");
            outFile.Close();
        }

        //private static void DrawOnLine(string text, int lineNum, bool leftToRight = true)
        //   => ResultDisplay.DrawText(Helper.InputFont, text, new Vector2(leftToRight ? margins.X : nameEntryBox.Width - margins.X - Helper.InputFont.MeasureString(text).X, (Helper.InputFont.MeasureString("S").Y + lineSpacing) * lineNum + margins.Y), Color.White);
    }
}
