//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: MainMenu.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Handles everything about the main menu

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    static class MainMenu
    {
        //the bg image and box
        private static Texture2D bgImg;
        private static Rectangle bgBox;

        //the button dimentions
        private const int BUTTON_WIDTH = 350;
        private const int BUTTON_HEIGHT = 100;

        //space between buttons and starting height of first button
        private const int SPACE_BETWEEN_BUTTONS = 10;
        private const int STARTING_BITTON_HEIGHT = 150;

        //Stores all the buttons
        private static Button playButton;
        private static Button instructionsButton;
        private static Button highScoresButton;
        private static Button exitButton;

        //stores the action of each button
        public static event Action playButtonPressed;
        public static event Action instructionsButtonPressed;
        public static event Action highScoresButtonPressed;
        public static event Action exitButtonPressed;

        //stores the forn used for the title
        private static SpriteFont font;
        private static string title = "PATHFINDER";
        public static void LoadContent()
        {
            //loads the bg image and rec
            bgImg = Helper.LoadImage("Images/MenuBackground");
            bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

            //loads all the buttons
            playButton = new Button(new Rectangle(Main.WIDTH / 2 - BUTTON_WIDTH / 2, STARTING_BITTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), playButtonPressed, "PLAY");
            instructionsButton = new Button(new Rectangle(Main.WIDTH / 2 - BUTTON_WIDTH / 2, STARTING_BITTON_HEIGHT + (BUTTON_HEIGHT + SPACE_BETWEEN_BUTTONS), BUTTON_WIDTH, BUTTON_HEIGHT), instructionsButtonPressed, "VIEW INSTRUCTIONS");
            highScoresButton = new Button(new Rectangle(Main.WIDTH / 2 - BUTTON_WIDTH / 2, STARTING_BITTON_HEIGHT + (BUTTON_HEIGHT + SPACE_BETWEEN_BUTTONS) * 2, BUTTON_WIDTH, BUTTON_HEIGHT), highScoresButtonPressed, "VEIW HIGHSCORES");
            exitButton = new Button(new Rectangle(Main.WIDTH / 2 - 100, STARTING_BITTON_HEIGHT + (BUTTON_HEIGHT + SPACE_BETWEEN_BUTTONS) * 3, 200, 66), exitButtonPressed, "EXIT");

            //loads the font
            font = Helper.Content.Load<SpriteFont>("Fonts/TitleFont");
        }

        /// <summary>
        /// updates all the buttons
        /// </summary>
        public static void Update()
        {
            playButton.Update();
            instructionsButton.Update();
            highScoresButton.Update();
            exitButton.Update();
        }

        //draw the menu
        public static void Draw()
        {
            //draw bg image and title
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);
            Helper.SpriteBatch.DrawString(font, title, new Vector2(Main.WIDTH / 2 - font.MeasureString(title).X / 2, 35), Color.White);

            //draw all the buttons
            playButton.Draw();
            instructionsButton.Draw();
            highScoresButton.Draw();
            exitButton.Draw();
        }
    }
}
