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
        enum GameState
        {
            Input,
            Game
        }
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private List<GameObject> gameObjects = new List<GameObject>();

        public const int CELL_SIDE_LENGTH = 45;

        public const int NUM_CELLS_WIDTH = 20;
        public const int NUM_CELLS_HEIGHT = 9;

        public const int WIDTH = NUM_CELLS_WIDTH * CELL_SIDE_LENGTH;
        public const int HEIGHT = NUM_CELLS_HEIGHT * CELL_SIDE_LENGTH;

        private const int INPUT_MENU_HEIGHT = 150;

        private GameState gameState = GameState.Input;

        private string input = string.Empty;
        private List<Keys> keysPressedLastFrame = new List<Keys>();
        SpriteFont inputFont;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT + INPUT_MENU_HEIGHT;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen.spriteBatch = spriteBatch;

            Helper.Content = Content;
            Helper.graphics = graphics;

            inputFont = Content.Load<SpriteFont>("Fonts/InputFont");

            LoadLevelFromFile("Level.txt");
            gameObjects.ForEach(g => g.MoveReady += gameObject => MoveGameObject(gameObject));
            gameObjects.ForEach(g => g.DeleteReady += gameObject => gameObjects.Remove(gameObject));

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            switch (gameState)
            {
                case GameState.Input:
                    var newKyes = Keyboard.GetState().GetPressedKeys();
                    keysPressedLastFrame = newKyes.Union(keysPressedLastFrame).ToList();

                    for (int i = 0; i < keysPressedLastFrame.Count; i++)
                    {
                        Keys key = keysPressedLastFrame[i];
                        if (!Keyboard.GetState().IsKeyDown(key))
                        {
                            keysPressedLastFrame.Remove(key);
                            if (key == Keys.Back)
                            {
                                if(input.Count() == 0)
                                {
                                    continue;
                                }

                                input = input.Substring(0, input.Count() - 1);
                            }
                            else
                            {
                                if (key.ToString().Count() > 1)
                                {
                                    continue;
                                }

                                input += key.ToString();
                            }
                        }
                    }
                    break;
                case GameState.Game:
                    gameObjects.ForEach(g => g.Update());
                    MoveGameObjects();
                    break;
            }
           

            base.Update(gameTime);
        }

        private void MoveGameObjects()
        {
            for (int i = gameObjects.Count - 1; i >= 0; --i)
            {
                MoveGameObject(gameObjects[i]);
            }
        }

        private void MoveGameObject(GameObject gameObject)
        {
            if (gameObject.Velocity == Vector2.Zero)
            {
                return;
            }

            Vector2 possibleVelocity = RestrictVelocity(gameObject, gameObject.Velocity);
            gameObject.Move(possibleVelocity);
        }

        private Vector2 RestrictVelocity(GameObject gameObject, Vector2 wantedVelocity, bool anotherPass = true)
        {
            RayCollisionInfo firstCollision = new RayCollisionInfo();
            RayCollisionInfo curCollision;

            GameObject collidedGameObject = null;

            foreach (GameObject otherGameObject in gameObjects)
            {
                if (gameObject == otherGameObject)
                {
                    continue;
                }

                foreach (Vector2 rayStartPoint in GetRayStatingPointsOnBox(gameObject.Box))
                {
                    curCollision = Helper.RayBoxFirstCollision(new Line(rayStartPoint, rayStartPoint + wantedVelocity), otherGameObject.Box);

                    if (curCollision.Sides.Contains(Side.Left) && rayStartPoint.X == gameObject.Box.Left ||
                        curCollision.Sides.Contains(Side.Right) && rayStartPoint.X == gameObject.Box.Right ||
                        curCollision.Sides.Contains(Side.Top) && rayStartPoint.Y == gameObject.Box.Top ||
                        curCollision.Sides.Contains(Side.Bottom) && rayStartPoint.Y == gameObject.Box.Bottom)
                    { continue; }

                    if (curCollision.Sides.Count != 0 && (firstCollision.Sides.Count == 0 || curCollision.Distance.Length() < firstCollision.Distance.Length() 
                        || (curCollision.Distance.Length() == firstCollision.Distance.Length() && curCollision.Sides.Count < firstCollision.Sides.Count)))
                    {
                        collidedGameObject = otherGameObject;
                        firstCollision = curCollision;
                    }
                }
            }

            if (firstCollision.Sides.Count == 0)
            {
                Rectangle nextFrameRec = gameObject.Box;
                nextFrameRec.Location += wantedVelocity.ToPoint();
                foreach (GameObject otherGameObject in gameObjects)
                {
                    if (gameObject == otherGameObject)
                    {
                        continue;
                    }

                    if (nextFrameRec.Intersects(otherGameObject.Box))
                    {
                        collidedGameObject = otherGameObject;

                        if (gameObject.Box.Bottom <= otherGameObject.Box.Top)
                        {
                            firstCollision.Distance.Y = otherGameObject.Box.Top - gameObject.Box.Bottom;
                            firstCollision.Sides.Add(Side.Top);
                        }

                        if (gameObject.Box.Top >= otherGameObject.Box.Bottom)
                        {
                            firstCollision.Distance.Y = otherGameObject.Box.Bottom - gameObject.Box.Top;
                            firstCollision.Sides.Add(Side.Bottom);
                        }

                        if (gameObject.Box.Right <= otherGameObject.Box.Left)
                        {
                            firstCollision.Distance.X = otherGameObject.Box.Left - gameObject.Box.Right;
                            firstCollision.Sides.Add(Side.Left);
                        }

                        if (gameObject.Box.Left >= otherGameObject.Box.Right)
                        {
                            firstCollision.Distance.X = otherGameObject.Box.Right - gameObject.Box.Left;
                            firstCollision.Sides.Add(Side.Right);
                        }
                    }
                }
            }


            if (firstCollision.Sides.Count != 0)
            {
                Rectangle collidedObjectBox = collidedGameObject.Box;

                collidedGameObject.InformCollisionTo(gameObject, firstCollision.Sides.Select(s => s.Flip()));
                gameObject.InformCollisionTo(collidedGameObject, firstCollision.Sides);

                if (!gameObjects.Contains(collidedGameObject) || collidedGameObject.Box != collidedObjectBox)
                {
                    return RestrictVelocity(gameObject, wantedVelocity);
                }

                if (firstCollision.Sides.Contains(Side.Top) || firstCollision.Sides.Contains(Side.Bottom))
                {
                    gameObject.Velocity.Y = 0;
                    wantedVelocity.Y = firstCollision.Distance.Y;
                }

                if (firstCollision.Sides.Contains(Side.Left) || firstCollision.Sides.Contains(Side.Right))
                {
                    gameObject.Velocity.X = 0;
                    wantedVelocity.X = firstCollision.Distance.X;
                }

                if (anotherPass)
                {
                    return RestrictVelocity(gameObject, wantedVelocity, false);
                }
            }

            return wantedVelocity;
        }

        public static IEnumerable<Vector2> GetRayStatingPointsOnBox(Rectangle box)
        {

            for (int c = 0; c < 2; ++c)
            {
                for (int r = 0; r < 2; ++r)
                {
                    yield return new Vector2(box.Location.X + c * box.Width, box.Location.Y + r * box.Height);
                }
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            gameObjects.ForEach(g => g.Draw(spriteBatch));

            spriteBatch.Draw(Helper.GetRectTexture(WIDTH, INPUT_MENU_HEIGHT, Color.Black), new Vector2(0, HEIGHT), Color.White);
            spriteBatch.DrawString(inputFont, input, new Vector2(5, 410), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevelFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int r = 0; r < NUM_CELLS_HEIGHT; ++r)
            {
                for (int c = 0; c < NUM_CELLS_WIDTH; ++c)
                {
                    switch (lines[r][c])
                    {
                        case '0':
                            gameObjects.Add(new Player(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '1':
                            gameObjects.Add(new Wall(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '2':
                            gameObjects.Add(new Crate(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '6':
                            gameObjects.Add(new Gem(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                    }
                }
            }
        }
    }
}
