using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PASS4
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

        private Stage stage = Stage.Menu;

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


            Game.LoadContent();
            Game.AllLevelsComplete += score =>
            {
                stage = Stage.NameEntry;
                NameEntryMenu.Start(score);
            };

            NameEntryMenu.LoadContent();
            NameEntryMenu.InputComplete += () => stage = Stage.Menu;

            MainMenu.playButtonPressed += () =>
            {
                stage = Stage.Game;
                Game.Reset();
            };
            MainMenu.instructionsButtonPressed += () => stage = Stage.Instructions;
            MainMenu.highScoresButtonPressed += () => stage = Stage.HighScores;
            MainMenu.exitButtonPressed += () => Exit();
            MainMenu.LoadContent();

            HighScores.BackToMenu += () => stage = Stage.Menu;
            HighScores.LoadContent();
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

            switch (stage)
            {
                case Stage.Instructions:
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
