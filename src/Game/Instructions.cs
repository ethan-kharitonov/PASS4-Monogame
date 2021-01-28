//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Instructions.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Displayes the instructions screem

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    static class Instructions
    {
        //store the instructions image and rec
        private static Texture2D instructionImg = Helper.LoadImage("Images/instructions");
        private static Rectangle bgBox = new Rectangle(0, 0, Main.WIDTH, Main.HEIGHT);

        //store the back to menu button
        private static Button backToMenuBtn;

        //invokes when button presses
        public static event Action BackToMenu;

        /// <summary>
        /// loads the button
        /// </summary>
        public static void LoadContent()
        {
            backToMenuBtn = new Button(new Rectangle(Main.WIDTH / 2 - 200, 450, 400, 100), BackToMenu, "RETURN TO MENU");
        }

        //updates the button
        public static void Update()
        {
            backToMenuBtn.Update();
        }

        //draws the instructions and the button
        public static void Draw()
        {
            Helper.SpriteBatch.Draw(instructionImg, bgBox, Color.White);
            backToMenuBtn.Draw();
        }
    }
}
