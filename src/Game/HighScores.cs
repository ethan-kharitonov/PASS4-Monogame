//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: HighScores.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Handles everything about highscores and displayig them
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS4
{
    static class HighScores
    {
        //The background image and rec
        private static Texture2D bgImg = Helper.LoadImage("Images/MenuBackground");
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

        //The stats sorted by name and score
        private static string[] statsByName;
        private static (string name, string score)[] statsByScore;

        //Everything used to draw the scores on the screen
        private static SpriteFont scoreFont;
        private static Vector2 margins = new Vector2(175, 30);
        private const int LINE_SPACING = 5;
        private const int NUM_SCORES_TO_DISPLAY = 10;
        private const int SCORES_STARTING_LINE = 3;

        //the font used for the title
        private static SpriteFont titleFont;

        //the return to menu button and the event invoked when its clicked
        private static Button returnToMenuButton;
        public static event Action BackToMenu;

        //the user input for serching names
        private static string userInput = string.Empty;

        private static string searchResult = String.Empty;

        /// <summary>
        /// Loads everything for the HighScores
        /// </summary>
        public static void LoadContent()
        {
            //sores the file and saves it
            SortStatFile();

            //loads the fonts
            scoreFont = Helper.Content.Load<SpriteFont>("Fonts/HighScoreFont");
            titleFont = Helper.Content.Load<SpriteFont>("Fonts/TitleFont");

            //loads the button
            returnToMenuButton = new Button(new Rectangle(Main.WIDTH / 2 - 200, 440, 400, 85), BackToMenu, "RETURN TO MENU");
        }

        /// <summary>
        /// Sorts the file and saves it
        /// </summary>
        public static void SortStatFile()
        {
            //reset search result before entering
            searchResult = string.Empty;

            //Gets the lines of the scores from file
            string[] stats = File.ReadAllLines("../../../NamesAndScores.txt");

            //sorts the file and saves it
            statsByName = Helper.MergeSort(stats, s => s.Split(',')[0]);
            statsByScore = Helper.MergeSort(stats, s => s.Split(',')[1]).Select(l => (l.Split(',')[0], l.Split(',')[1])).ToArray();
        }


        /// <summary>
        /// Updates the highscore menu
        /// </summary>
        public static void Update()
        {
            //updates the button
            returnToMenuButton.Update();

            //Gets user input
            userInput = Helper.UpdateStringWithInput(userInput);
            userInput = Helper.TrimString(userInput, 15);

            //check for enter
            if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
            {
                //if search happened reset it
                if(searchResult != string.Empty)
                {
                    userInput = string.Empty;
                    searchResult = string.Empty;
                }
                else
                {
                    //search the list for matching name
                    List<(string name, string score)> mathcingStats = statsByScore.Where(s => s.name == userInput).ToList();
                    if (mathcingStats.Count() == 0)
                    {
                        searchResult = "No mathing name";
                    }
                    else
                    {
                        searchResult = $"{mathcingStats.First().name} : {mathcingStats.First().score}";
                    }
                }
                
            }
        }

        /// <summary>
        /// Draw the menu
        /// </summary>
        public static void Draw()
        {
            //draws the backgruound and title
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);
            Helper.SpriteBatch.DrawString(titleFont, "HIGH SCORES", new Vector2(Main.WIDTH / 2 - titleFont.MeasureString("HIGH SCORES").X / 2, 10), Color.White);

            //displays the mini titles
            DrawOnLine($"NAMES", SCORES_STARTING_LINE);
            DrawOnLine("SCORES", SCORES_STARTING_LINE, false);

            //Displayes all the scores
            for (int i = 0; i < NUM_SCORES_TO_DISPLAY; ++i)
            {
                if (i < statsByScore.Length)
                {
                    DrawOnLine($"{i + 1}) {statsByScore[i].name}", i + SCORES_STARTING_LINE + 1);
                    DrawOnLine(statsByScore[i].score, i + SCORES_STARTING_LINE + 1, false);
                }
                else
                {
                    DrawOnLine($"{i + 1}) ", i + SCORES_STARTING_LINE + 1);
                }
            }

            //Dipslayes the users searhc
            Helper.SpriteBatch.DrawString(scoreFont, "Enter Name:", new Vector2(Main.WIDTH / 2 - scoreFont.MeasureString("Enter Name:").X / 2, 100), Color.White);
            Helper.SpriteBatch.DrawString(scoreFont, userInput, new Vector2(Main.WIDTH / 2 - scoreFont.MeasureString(userInput).X / 2, 120), Color.White);
            Helper.SpriteBatch.DrawString(scoreFont, searchResult, new Vector2(Main.WIDTH / 2 - scoreFont.MeasureString(searchResult).X / 2, 140), Color.White);


            //displays the button
            returnToMenuButton.Draw();
        }

        /// <summary>
        /// Draws text to a line on the screen
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="lineNum">vertical space between lines</param>
        /// <param name="leftToRight">Indicates if the text should be drawn left to righ or right to left</param>
        private static void DrawOnLine(string text, int lineNum, bool leftToRight = true)
           => Helper.SpriteBatch.DrawString(scoreFont, text, new Vector2(leftToRight ? margins.X : Main.WIDTH - margins.X - scoreFont.MeasureString(text).X, (scoreFont.MeasureString("S").Y + LINE_SPACING) * lineNum + margins.Y), Color.White);

    }
}
