using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS4
{
    class Button
    {
        private static readonly Texture2D button = Helper.LoadImage("Images/Button/Button BG shadow");
        private static readonly Texture2D buttonPressed = Helper.LoadImage("Images/Button/Button BG");

        private bool isPressed = false;
        private Rectangle box;

        private Action onClickAction;

        public Button(Rectangle box, Action onClickAction)
        {
            this.box = box;
            this.onClickAction = onClickAction;
        }

        public void Update()
        {
            if(Helper.IsPointInOrOnRectangle(Mouse.GetState().Position.ToVector2(), box))
            {
                if(isPressed && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    onClickAction.Invoke();
                }

                isPressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
            }
        }

        public void Draw()
        {
            Helper.SpriteBatch.Draw(isPressed ? buttonPressed : button, box, Color.White);
        }

    }
}
