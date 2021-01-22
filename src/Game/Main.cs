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

            foreach (ISection section in sections)
            {
                section.LoadContent();
            }
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

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
