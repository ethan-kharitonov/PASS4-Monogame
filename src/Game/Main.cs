using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        enum Stage
        {
            Instructions,
            Menu,
            Game,
            NameEntry,
            HighScores
        }

        private Stage stage = Stage.NameEntry;

        public const int WIDTH = 900;
        public const int HEIGHT = 555;


        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen.spriteBatch = spriteBatch;

            Helper.Content = Content;
            Helper.graphics = graphics;
            Helper.SpriteBatch = spriteBatch;
            Helper.InputFont = Content.Load<SpriteFont>("Fonts/InputFont");
           

            MainGame.LoadContent();
            MainGame.AllLevelsComplete += () => stage = Stage.NameEntry;

            NameEntryMenu.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Helper.UpdateKeyBoard();

            switch (stage)
            {
                case Stage.Instructions:
                    break;

                case Stage.Menu:
                    break;

                case Stage.Game:
                    MainGame.Update();
                    break;

                case Stage.NameEntry:
                    NameEntryMenu.Update();
                    break;

                case Stage.HighScores:
                    break;
            }


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (stage)
            {
                case Stage.Instructions:
                    break;

                case Stage.Menu:
                    break;

                case Stage.Game:
                    MainGame.Draw();
                    break;

                case Stage.NameEntry:
                    NameEntryMenu.Draw();
                    break;

                case Stage.HighScores:
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
