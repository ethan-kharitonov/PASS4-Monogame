using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    static class MainMenu
    {
        private static Texture2D bgImg;
        private static Rectangle bgBox;

        private const int BUTTON_WIDTH = 350;
        private const int BUTTON_HEIGHT = 100;

        private const int SPACE_BETWEEN_BUTTONS = 10;
        private const int STARTING_BITTON_HEIGHT = 150;

        private static Button playButton;
        private static Button instructionsButton;
        private static Button highScoresButton;
        private static Button exitButton;

        public static event Action playButtonPressed;
        public static event Action instructionsButtonPressed;
        public static event Action highScoresButtonPressed;
        public static event Action exitButtonPressed;


        public static void LoadContent()
        {
            bgImg = Helper.LoadImage("Images/MenuBackground");
            bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

            playButton = new Button(new Rectangle(Main.WIDTH / 2 - BUTTON_WIDTH / 2, STARTING_BITTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), playButtonPressed, "PLAY");
            instructionsButton = new Button(new Rectangle(Main.WIDTH / 2 - BUTTON_WIDTH / 2, STARTING_BITTON_HEIGHT + (BUTTON_HEIGHT + SPACE_BETWEEN_BUTTONS), BUTTON_WIDTH, BUTTON_HEIGHT), instructionsButtonPressed, "VIEW INSTRUCTIONS");
            highScoresButton = new Button(new Rectangle(Main.WIDTH / 2 - BUTTON_WIDTH / 2, STARTING_BITTON_HEIGHT + (BUTTON_HEIGHT + SPACE_BETWEEN_BUTTONS) * 2, BUTTON_WIDTH, BUTTON_HEIGHT), highScoresButtonPressed, "VEIW HIGHSCORES");
            exitButton = new Button(new Rectangle(Main.WIDTH / 2 - 100, STARTING_BITTON_HEIGHT + (BUTTON_HEIGHT + SPACE_BETWEEN_BUTTONS) * 3, 200, 66), exitButtonPressed, "EXIT");
        }

        public static void Update()
        {
            playButton.Update();
            instructionsButton.Update();
            highScoresButton.Update();
            exitButton.Update();
        }

        public static void Draw()
        {
            Helper.SpriteBatch.Draw(bgImg, bgBox, Color.White);

            playButton.Draw();
            instructionsButton.Draw();
            highScoresButton.Draw();
            exitButton.Draw();
        }
    }
}
