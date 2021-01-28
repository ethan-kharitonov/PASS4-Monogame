using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace PASS4
{
    static class HighScores
    {
        private static Texture2D bgImg = Helper.LoadImage("Images/MenuBackground");
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

        private static string[] statsByName;
        private static (string name, string score)[] statsByScore;

        private static SpriteFont scoreFont;
        private static Vector2 margins = new Vector2(175, 30);
        private const int LINE_SPACING = 5;
        private const int NUM_SCORES_TO_DISPLAY = 10;
        private const int SCORES_STARTING_LINE = 3;

        private static SpriteFont titleFont;

        private static Button returnToMenuButton;
        public static event Action BackToMenu;

        public static void LoadContent()
        {
            SortStatFile();
            scoreFont = Helper.Content.Load<SpriteFont>("Fonts/HighScoreFont");
            titleFont = Helper.Content.Load<SpriteFont>("Fonts/TitleFont");

            returnToMenuButton = new Button(new Rectangle(Main.WIDTH / 2 - 200, 440, 400, 85), BackToMenu, "RETURN TO MENU");
        }

        public static void SortStatFile()
        {
            string[] stats = File.ReadAllLines("../../../NamesAndScores.txt");
            statsByName = Helper.MergeSort(stats, s => s.Split(',')[0]);
            statsByScore = Helper.MergeSort(stats, s => s.Split(',')[1]).Select(l => (l.Split(',')[0], l.Split(',')[1])).Reverse().ToArray();
        }

        


        public static void Update()
        {
            returnToMenuButton.Update();
        }


        public static void Draw()
        {
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);

            Helper.SpriteBatch.DrawString(titleFont, "HIGH SCORES", new Vector2(Main.WIDTH / 2 - titleFont.MeasureString("HIGH SCORES").X / 2, 10), Color.White);


            DrawOnLine($"NAMES", SCORES_STARTING_LINE);
            DrawOnLine("SCORES", SCORES_STARTING_LINE, false);


            for (int i = 0; i < NUM_SCORES_TO_DISPLAY; ++i)
            {
                if(i < statsByScore.Length)
                {
                    DrawOnLine($"{i + 1}) {statsByScore[i].name}", i + SCORES_STARTING_LINE + 1);
                    DrawOnLine(statsByScore[i].score, i + SCORES_STARTING_LINE + 1, false);
                }
                else
                {
                    DrawOnLine($"{i + 1}) ", i + SCORES_STARTING_LINE + 1);
                }
            }

            returnToMenuButton.Draw();
        }

        private static void DrawOnLine(string text, int lineNum, bool leftToRight = true)
           => Helper.SpriteBatch.DrawString(scoreFont, text, new Vector2(leftToRight ? margins.X : Main.WIDTH - margins.X - scoreFont.MeasureString(text).X, (scoreFont.MeasureString("S").Y + LINE_SPACING) * lineNum + margins.Y), Color.White);

    }
}
