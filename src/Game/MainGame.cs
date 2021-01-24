using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game
{
    class MainGame : ISection
    {
        public static readonly MainGame Instance = new MainGame();

        private List<GameObject> gameObjects = new List<GameObject>();

        private const int NUM_CELLS_WIDTH = 20;
        private const int NUM_CELLS_HEIGHT = 9;

        public const int CELL_SIDE_LENGTH = 45;

        public const int WIDTH = NUM_CELLS_WIDTH * CELL_SIDE_LENGTH;
        public const int HEIGHT = NUM_CELLS_HEIGHT * CELL_SIDE_LENGTH;

        private Screen screen = new Screen(new Point(0, 0), WIDTH, HEIGHT);

        public delegate void Notify();
        public event Notify OutOfCommands;
        public event Notify ExecutingNextCommand;

        private Player player;
        private Queue<char> commands = new Queue<char>();

        private MainGame()
        {

        }

        public void LoadCommands(Queue<char> commands)
        {
            this.commands = commands;
            commands.Enqueue('X');
        }
        public void LoadContent()
        {
            LoadLevelFromFile("Level.txt");

            gameObjects.ForEach(g => g.MoveReady += gameObject => MoveGameObject(gameObject));
            gameObjects.ForEach(g => g.DeleteReady += gameObject => gameObjects.Remove(gameObject));
        }

        public void Update()
        {
            if (!commands.IsEmpty && gameObjects.All(g => g.IsStandingStill()))
            {
                player.LoadNextCommand(commands.Dequeue());
                ExecutingNextCommand.Invoke();
                if (commands.IsEmpty)
                {
                    OutOfCommands.Invoke();
                }
            }
            gameObjects.ForEach(g => g.Update());

            MoveGameObjects();
        }

        public void Draw()
        {
            gameObjects.ForEach(g => g.Draw(screen));
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
            RayCollisionInfo firstCollision;
            GameObject collidedGameObject;
            (firstCollision, collidedGameObject) = FindCollision(gameObject, wantedVelocity);

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

                        if (gameObject.Box.Bottom - 10 <= otherGameObject.Box.Top)
                        {
                            firstCollision.Distance.Y = otherGameObject.Box.Top - gameObject.Box.Bottom;
                            firstCollision.Sides.Add(Side.Top);
                        }

                        if (gameObject.Box.Top + 10 >= otherGameObject.Box.Bottom)
                        {
                            firstCollision.Distance.Y = otherGameObject.Box.Bottom - gameObject.Box.Top;
                            firstCollision.Sides.Add(Side.Bottom);
                        }

                        if (gameObject.Box.Right - 10 <= otherGameObject.Box.Left)
                        {
                            firstCollision.Distance.X = otherGameObject.Box.Left - gameObject.Box.Right;
                            firstCollision.Sides.Add(Side.Left);
                        }

                        if (gameObject.Box.Left + 10 >= otherGameObject.Box.Right)
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

                if (firstCollision.Sides.Contains(Side.Left) || firstCollision.Sides.Contains(Side.Right))
                {
                    gameObject.Velocity.X = 0;
                    wantedVelocity.X = firstCollision.Distance.X;
                }
                 
                if (firstCollision.Sides.Contains(Side.Top) || firstCollision.Sides.Contains(Side.Bottom))
                {
                    gameObject.Velocity.Y = 0;
                    wantedVelocity.Y = firstCollision.Distance.Y;
                }


                if (anotherPass)
                {
                    return RestrictVelocity(gameObject, wantedVelocity, false);
                }
            }

            return wantedVelocity;
        }

        private (RayCollisionInfo, GameObject collidedGameObject) FindCollision(GameObject gameObject, Vector2 wantedVelocity)
        {
            GameObject collidedGameObject = null;
            RayCollisionInfo firstCollision = new RayCollisionInfo();
            RayCollisionInfo curCollision;
            foreach (GameObject otherGameObject in gameObjects)
            {
                if (gameObject == otherGameObject)
                {
                    continue;
                }

                foreach (Vector2 rayStartPoint in Helper.GetVertecies(gameObject.Box))
                {
                    curCollision = Helper.RayBoxFirstCollision(new Line(rayStartPoint, rayStartPoint + wantedVelocity), otherGameObject.Box);

                    if (curCollision.Sides.Contains(Side.Left) && rayStartPoint.X == gameObject.Box.Left)
                    {
                        curCollision.Sides.Remove(Side.Left);
                    }

                    if (curCollision.Sides.Contains(Side.Right) && rayStartPoint.X == gameObject.Box.Right)
                    {
                        curCollision.Sides.Remove(Side.Right);
                    }

                    if (curCollision.Sides.Contains(Side.Top) && rayStartPoint.Y == gameObject.Box.Top)
                    {
                        curCollision.Sides.Remove(Side.Top);
                    }

                    if (curCollision.Sides.Contains(Side.Bottom) && rayStartPoint.Y == gameObject.Box.Bottom)
                    {
                        curCollision.Sides.Remove(Side.Bottom);
                    }


                    if (curCollision.Sides.Count != 0 && (firstCollision.Sides.Count == 0 || curCollision.Distance.Length() < firstCollision.Distance.Length()
                        || (curCollision.Distance.Length() == firstCollision.Distance.Length() && curCollision.Sides.Count < firstCollision.Sides.Count)))
                    {
                        collidedGameObject = otherGameObject;
                        firstCollision = curCollision;
                    }
                }
            }

            return (firstCollision, collidedGameObject);
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
                            player = new Player(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH);
                            gameObjects.Add(player);
                            break;
                        case '1':
                            gameObjects.Add(new Wall(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '2':
                            gameObjects.Add(new Crate(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '4':
                            gameObjects.Add(new Door(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '5':
                            gameObjects.Add(new Spike(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '6':
                            gameObjects.Add(new Gem(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '7':
                            gameObjects.Add(new Key(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                    }
                }
            }
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();
    }
}
