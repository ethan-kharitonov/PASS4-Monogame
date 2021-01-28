//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Button.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: A button visual display that invokes a given action when pressed.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS4
{
    class Button
    {
        //Stores the clicked and unclicked button images
        private static readonly Texture2D button = Helper.LoadImage("Images/Button BG shadow");
        private static readonly Texture2D buttonPressed = Helper.LoadImage("Images/Button BG");

        //Stores the text  writen on the button and the font it is writen in
        private static readonly SpriteFont buttonFont = Helper.Content.Load<SpriteFont>("Fonts/ButtonFont");
        private string text;

        //Indicates whether the mouse is hovering over the button and whether it is pressing the button
        private bool isHoverd = false;
        private bool isPressed = false;

        //Stores the rectangle where the button will be drawn
        private Rectangle box;

        //Stores the given action that will be invoked when the player presses the button
        private Action onClickAction;

        /// <summary>
        /// Creates a new button with a rectangle, on click action and a text to be writen
        /// </summary>
        /// <param name="box">Where the button will be drawn</param>
        /// <param name="onClickAction">The action invoked on click</param>
        /// <param name="text">The text writen on the button</param>
        public Button(Rectangle box, Action onClickAction, string text)
        {
            this.text = text;
            this.box = box;
            this.onClickAction = onClickAction;
        }

        /// <summary>
        /// Updates the buttons isHovering and isPressed variables
        /// </summary>
        public void Update()
        {
            //Checks if the button has just been released
            if (isPressed && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                //invokes the action
                onClickAction.Invoke();
            }

            //Checks if the mouse is in the button
            if (Helper.IsPointInOrOnRectangle(Mouse.GetState().Position.ToVector2(), box))
            {
                //Records that the mouse is hovering over the button and updates isPressed
                isHoverd = true;
                isPressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
            }
            else
            {
                //Records that the mouse is not hovering or pressing the button
                isHoverd = false;
                isPressed = false;
            }
        }

        /// <summary>
        /// Draws the button
        /// </summary>
        public void Draw()
        {
            //Draws the correct button body depending on whether it is pressed and draws the text over it
            Helper.SpriteBatch.Draw(isPressed ? buttonPressed : button, box, isHoverd ? Color.Gainsboro : Color.White);
            Helper.SpriteBatch.DrawString(buttonFont, text, box.Center.ToVector2() - buttonFont.MeasureString(text) * 0.5f, isHoverd ? Color.Gainsboro : Color.White);
        }

    }
}
