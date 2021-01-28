//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: NameEntryMenu.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Lets the user input their name to save their score
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace PASS4
{
    static class NameEntryMenu
    {
        //The action on input complete
        public static Action InputComplete;

        //the name and the max charachters
        private static string name = string.Empty;
        private const int MAX_NAME_LENGTH = 15;

        //the name entry panel image and its rec
        private static Texture2D nameEntryBg;
        private static Rectangle nameEntryBox = new Rectangle(Main.WIDTH / 2 - 150, Main.HEIGHT / 2 - 50, 300, 80);

        //the bg image and its rec
        private static Texture2D bgImg;
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

        //the return to menu button
        private static Button menuButton;

        //the score of the user from the game
        private static int score;

        //the file path to the score file
        private const string SCORE_FILE_PATH = "NamesAndScores.txt";

        /// <summary>
        /// Loads all the content for the name entry menu
        /// </summary>
        public static void LoadContent()
        {
            //loads the name entry panel builds its rectangle
            nameEntryBg = Helper.LoadImage("Images/NameEntryPanel");
            float neScaleFactor = 2f;
            int neWidth = (int)Math.Round(nameEntryBg.Width * neScaleFactor);
            int neHeight = (int)Math.Round(nameEntryBg.Height * neScaleFactor);
            nameEntryBox = new Rectangle(Main.WIDTH / 2 - neWidth/2, Main.HEIGHT / 2 - neHeight/2 - 10, neWidth, neHeight) ;

            //loads the bg image
            bgImg = Helper.LoadImage("Images/MenuBackground");

            //loads the return to menu button
            menuButton = new Button(new Rectangle(nameEntryBox.Center.X - (nameEntryBox.Width - 14)/2, nameEntryBox.Top + 130, nameEntryBox.Width - 14, 100), OnButtonClick, "SAVE AND GO TO MENU");
        }

        /// <summary>
        /// Called when entered
        /// </summary>
        /// <param name="score"></param>
        public static void Start(int score)
        {
            //get the score and reset the name
            NameEntryMenu.score = score;
            name = string.Empty;
        }

        /// <summary>
        /// updates the menu
        /// </summary>
        public static void Update()
        {
            //gets user input for the name
            name = Helper.UpdateStringWithInput(name);
            name = Helper.TrimString(name, MAX_NAME_LENGTH);

            //only updates button if there is a name written
            if(name.Length != 0)
            {
                menuButton.Update();
            }
        }

        /// <summary>
        /// draws the menu
        /// </summary>
        public static void Draw()
        {
            //draws the bg image, name panel and user input
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);
            Helper.SpriteBatch.Draw(nameEntryBg, nameEntryBox, Color.White);
            Helper.SpriteBatch.DrawString(Helper.InputFont, name, nameEntryBox.Center.ToVector2() - new Vector2(0, 84) - (Helper.InputFont.MeasureString(name) * 0.5f), Color.White);

            //displayes the users score
            Helper.SpriteBatch.DrawString(Helper.InputFont, $"Score:", new Vector2(nameEntryBox.Left + 15, nameEntryBox.Top + 90), Color.White);
            Helper.SpriteBatch.DrawString(Helper.InputFont, score.ToString(), new Vector2(nameEntryBox.Right - Helper.InputFont.MeasureString(score.ToString()).X - 15, nameEntryBox.Top + 90), Color.White);

            //displays the button
            menuButton.Draw();
        }

        /// <summary>
        /// The function called when the button is clicked
        /// </summary>
        private static void OnButtonClick()
        {
            //invoke input comeplet
            InputComplete.Invoke();

            //write the name to the file with the scores
            StreamWriter outFile = File.AppendText(SCORE_FILE_PATH);
            outFile.WriteLine($"{name},{score}");
            outFile.Close();

            //update the live list
            HighScores.SortStatFile();
        }

    }
}
