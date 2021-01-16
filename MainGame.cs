using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace PASS4
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private List<GameObject> gameObjects = new List<GameObject>();

        public const int CELL_SIDE_LENGTH = 45;

        public const int NUM_CELLS_WIDTH = 20;
        public const int NUM_CELLS_HEIGHT = 9;

        public const int WIDTH = NUM_CELLS_WIDTH * CELL_SIDE_LENGTH;
        public const int HEIGHT = NUM_CELLS_HEIGHT * CELL_SIDE_LENGTH;


        public MainGame()
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

            Helper.Content = Content;

            LoadLevelFromFile("Level");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            gameObjects.ForEach(g => g.Update());
            MoveGameObjects();

            base.Update(gameTime);
        }

        private void MoveGameObjects()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.Velocity == Vector2.Zero)
                {
                    continue;
                }

                Vector2 possibleVelocity = RestrictVelocity(gameObject, gameObject.Velocity);
                gameObject.Move(possibleVelocity);
            }
        }

        private Vector2 RestrictVelocity(GameObject gameObject, Vector2 wantedVelocity, bool anotherPass = true)
        {
            RayCollisionInfo firstCollision = new RayCollisionInfo();
            RayCollisionInfo curCollision;

            GameObject collidedGameObject = null;
            bool collisionOnVertex = false;

            foreach (GameObject otherGameObject in gameObjects)
            {
                if (gameObject == otherGameObject)
                {
                    continue;
                }

                foreach (Vector2 rayStartPoint in GetRayStatingPointsOnBox(gameObject.Box))
                {
                    curCollision = Helper.RayBoxFirstCollision(new Line(rayStartPoint, rayStartPoint + wantedVelocity), otherGameObject.Box);

                    if (curCollision.Sides.Contains(Side.Left) && rayStartPoint.X == gameObject.Box.Left)
                    {
                        continue;
                    }

                    if (curCollision.Sides.Contains(Side.Right) && rayStartPoint.X == gameObject.Box.Right)
                    {
                        continue;
                    }

                    if (curCollision.Sides.Contains(Side.Top) && rayStartPoint.Y == gameObject.Box.Top)
                    {
                        continue;
                    }

                    if (curCollision.Sides.Contains(Side.Bottom) && rayStartPoint.Y == gameObject.Box.Bottom)
                    {
                        continue;
                    }

                    if (curCollision.Sides.Count != 0 && (firstCollision.Sides.Count == 0 || curCollision.Distance.Length() < firstCollision.Distance.Length()))
                    {
                        collidedGameObject = otherGameObject;
                        firstCollision = curCollision;

                        if (rayStartPoint == new Vector2(gameObject.Box.Top, gameObject.Box.Left) ||
                            rayStartPoint == new Vector2(gameObject.Box.Top, gameObject.Box.Right) ||
                            rayStartPoint == new Vector2(gameObject.Box.Bottom, gameObject.Box.Left) ||
                            rayStartPoint == new Vector2(gameObject.Box.Bottom, gameObject.Box.Right))
                        {
                            collisionOnVertex = true;
                        }
                    }
                }
            }

           /* if(firstCollision.Sides.Count == 2 && !collisionOnVertex)
            {
                if (firstCollision.Sides.Contains(Side.Right) && gameObject.Box.Right > collidedGameObject.Box.Left)
                {
                    firstCollision.Sides.Remove(Side.Right);
                }

                if (firstCollision.Sides.Contains(Side.Left) && gameObject.Box.Left < collidedGameObject.Box.Right)
                {
                    firstCollision.Sides.Remove(Side.Left);
                }

                if (firstCollision.Sides.Contains(Side.Bottom) && gameObject.Box.Top > collidedGameObject.Box.Bottom)
                {
                    firstCollision.Sides.Remove(Side.Bottom);
                }

                if (firstCollision.Sides.Contains(Side.Top) && gameObject.Box.Bottom < collidedGameObject.Box.Top)
                {
                    firstCollision.Sides.Remove(Side.Top);
                }
            }*/

            if (firstCollision.Sides.Count != 0)
            {
                Rectangle collidedObjectBox = collidedGameObject.Box;

                collidedGameObject.InformCollisionTo(gameObject, firstCollision.Sides);
                collidedGameObject.InformCollisionTo(gameObject, firstCollision.Sides);

                if (collidedGameObject == null || collidedGameObject.Box != collidedObjectBox)
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

            for (int c = 0; c < 3; ++c)
            {
                for (int r = 0; r < 3; ++r)
                {
                    if (c == 1 && r == 1)
                    {
                        continue;
                    }

                    yield return new Vector2(box.Location.X + c * box.Width / 2, box.Location.Y + r * box.Height / 2);
                }
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            gameObjects.ForEach(g => g.Draw(spriteBatch));

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
                    }
                }
            }
        }
    }
}
