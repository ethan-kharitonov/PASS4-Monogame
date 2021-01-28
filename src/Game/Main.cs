//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: MainMenu.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Handles everything about the main menu
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PASS4
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //indicates the stage of the game
        enum Stage
        {
            Instructions,
            Menu,
            Game,
            NameEntry,
            HighScores
        }

        //indicates the stage of the game
        private Stage stage = Stage.Menu;


        //dimensions of the screen
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
            //sets the dimentions of the screen
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// loads all the content of the game
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen.spriteBatch = spriteBatch;

            //assigne helper variables
            Helper.Content = Content;
            Helper.graphics = graphics;
            Helper.SpriteBatch = spriteBatch;
            Helper.InputFont = Content.Load<SpriteFont>("Fonts/InputFont");

            //load the game
            Game.LoadContent();
            Game.AllLevelsComplete += score =>
            {
                stage = Stage.NameEntry;
                NameEntryMenu.Start(score);
            };

            //load the name entry menu
            NameEntryMenu.LoadContent();
            NameEntryMenu.InputComplete += () => stage = Stage.Menu;

            //load the main menu and sign up to events
            MainMenu.playButtonPressed += () =>
            {
                stage = Stage.Game;
                Game.Reset();
            };
            MainMenu.instructionsButtonPressed += () => stage = Stage.Instructions;
            MainMenu.highScoresButtonPressed += () => stage = Stage.HighScores;
            MainMenu.exitButtonPressed += () => Exit();
            MainMenu.LoadContent();

            //load high scores
            HighScores.BackToMenu += () => stage = Stage.Menu;
            HighScores.LoadContent();

            //load high scores
            Instructions.BackToMenu += () => stage = Stage.Menu;
            Instructions.LoadContent();

            //sign up to games back to menu event
            Game.BackToMenu += () => stage = Stage.Menu;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Helper.UpdateKeyBoard();

            //update the correct stage
            switch (stage)
            {
                case Stage.Instructions:
                    Instructions.Update();
                    break;

                case Stage.Menu:
                    MainMenu.Update();
                    break;

                case Stage.Game:
                    Game.Update();
                    break;

                case Stage.NameEntry:
                    NameEntryMenu.Update();
                    break;

                case Stage.HighScores:
                    HighScores.Update();
                    break;
            }


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            //draw the correct stage
            switch (stage)
            {
                case Stage.Instructions:
                    Instructions.Draw();
                    break;

                case Stage.Menu:
                    MainMenu.Draw();
                    break;

                case Stage.Game:
                    Game.Draw();
                    break;

                case Stage.NameEntry:
                    NameEntryMenu.Draw();
                    break;

                case Stage.HighScores:
                    HighScores.Draw();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}
