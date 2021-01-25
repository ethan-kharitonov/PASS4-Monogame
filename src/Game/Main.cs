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

        private Texture2D legend;

        private ISection[] sections = new ISection[]
        {
            MainGame.Instance,
            InputMenu.Instance
        };

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = sections.Max(s => s.GetMaxX());
            graphics.PreferredBackBufferHeight = sections.Max(s => s.GetMaxY());
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen.spriteBatch = spriteBatch;

            Helper.Content = Content;
            Helper.graphics = graphics;

            MainGame.Instance.RunComplete += m => InputMenu.Instance.StartInputProcess(m);
            InputMenu.Instance.CommandReadingComplete += q => MainGame.Instance.LoadCommands(q);

            MainGame.Instance.ExecutingNextCommand += () => InputMenu.Instance.ShowNextCommand();

            InputMenu.Instance.CommandReadingStarting += () => MainGame.Instance.ReStartLevel();

            foreach (ISection section in sections)
            {
                section.LoadContent();
            }


            legend = Helper.LoadImage("Images/command legend ethan");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach(ISection section in sections)
            {
                section.Update();
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            foreach (ISection section in sections)
            {
                section.Draw();
            }

            if (InputMenu.Instance.ShowLegend)
            {
                spriteBatch.Draw(legend, Vector2.Zero, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
