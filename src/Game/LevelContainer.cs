using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PASS4
{
    class LevelContainer : ISection
    {
        public static readonly LevelContainer Instance = new LevelContainer();

        public event Action ExecutingNextCommand;
        public event Action<string> RunComplete;
        public event Action AllLevelsComplete;

        public Action<int, int> KeysAndGemsCounted;
        public Action<int> playerKeyCollected;
        public Action<int> playerGemCollected;

        private static Texture2D backgroundImg;

        private List<GameObject> gameObjects = new List<GameObject>();
        private Player player;

        private Point flagPos;
        private Texture2D flagImg;

        private const int SCREEN_WALL_WIDTH = 3;

        private const int NUM_CELLS_WIDTH = 20;
        private const int NUM_CELLS_HEIGHT = 9;

        public const int CELL_SIDE_LENGTH = 45;

        public const int WIDTH = NUM_CELLS_WIDTH * CELL_SIDE_LENGTH;
        public const int HEIGHT = NUM_CELLS_HEIGHT * CELL_SIDE_LENGTH;

        private Screen screen = new Screen(new Point(0, 0), WIDTH, HEIGHT);

        private Queue<char> commands = new Queue<char>();

        private const string LEVEL_PATH_SUFFIX = "../../../Levels/Level";
        private int curLevel = 1;

        private const int NUM_LEVELS = 2;

        private int numKeys = 0;
        private int numGems = 0;

        public static Stopwatch timer = new Stopwatch();
        private bool startingNewLevel = true;

        private int numCommands;

        private LevelContainer()
        {

        }

        public void LoadContent()
        {
            flagImg = Helper.LoadImage("Images/flagImg");
            backgroundImg = Helper.LoadImage("Images/backGround");

            LoadLevelFromFile($"{LEVEL_PATH_SUFFIX}{curLevel}.txt");
            timer.Start();
        }
        public void LoadCommands(Queue<char> commands)
        {
            this.commands = commands;
            numCommands = commands.Count - 1;
        }

        public void Update()
        {
            if (!commands.IsEmpty && gameObjects.All(g => g.IsStandingStill()))
            {
                player.LoadNextCommand(commands.Dequeue());
                ExecutingNextCommand.Invoke();

                if (commands.IsEmpty)
                {
                    if (player.Box.Location == flagPos)
                    {
                        if (player.GemCount != numGems)
                        {
                            RunComplete.Invoke("You did not collect all the gems : press ENTER to try again.");
                        }
                        else
                        {
                            timer.Stop();

                            Game.FinalGameResult.LevelResults[curLevel - 1] = new Game.LevelResult(timer.Elapsed.Milliseconds + numCommands * 100, timer.Elapsed.Seconds);
                            Game.FinalGameResult.TotalTime += Game.FinalGameResult.LevelResults[curLevel - 1].Time;
                            Game.FinalGameResult.TotalScore += Game.FinalGameResult.LevelResults[curLevel - 1].Score;

                            if(curLevel == NUM_LEVELS)
                            {
                                AllLevelsComplete.Invoke();
                            }
                            else
                            {
                                RunComplete.Invoke("Success! You've reached the goal : Press ENTER to continue to the next level.");

                                ++curLevel;
                                startingNewLevel = true;
                            }
                        }
                    }
                    else
                    {
                        RunComplete.Invoke("Failed to reach goal : press ENTER to try again.");
                    }
                }
            }
            gameObjects.ForEach(g => g.Update());

            MoveGameObjects();
        }

        public void Draw()
        {
            screen.Draw(backgroundImg, new Rectangle(0, 0, WIDTH, HEIGHT));

            gameObjects.ForEach(g => g.Draw(screen));
            screen.Draw(flagImg, new Rectangle(flagPos.X + 17, flagPos.Y, (int)Math.Round((double)(flagImg.Width * CELL_SIDE_LENGTH / flagImg.Height)), CELL_SIDE_LENGTH));
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
            gameObject.Move(RestrictVelocity(gameObject, gameObject.Velocity));
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

                if (!collidedGameObject.IsCollidable)
                {
                    return wantedVelocity;
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
            numKeys = 0;
            numGems = 0;
            gameObjects.Clear();
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
                            Crate crate = new Crate(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH);
                            crate.CrateMove += c =>
                            {
                                IEnumerable<GameObject> objectsAbove = gameObjects.Where(g => Helper.IsPointInOrOnRectangle(g.TopLeftGridPoint.ToVector2(), new Rectangle(c.Box.Location - new Point(0, c.Box.Height), c.Box.Size)));
                                return !(objectsAbove.Count() != 0 && (objectsAbove.First() is Gem || objectsAbove.First() is Key));
                            };
                            gameObjects.Add(crate);
                            break;
                        case '3':
                            flagPos = new Point(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH);
                            break;
                        case '4':
                            gameObjects.Add(new Door(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '5':
                            gameObjects.Add(new Spike(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '6':
                            gameObjects.Add(new Gem(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            ++numGems;
                            break;
                        case '7':
                            gameObjects.Add(new Key(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            ++numKeys;
                            break;
                    }
                }
            }

            gameObjects.OfType<Crate>().ToList().ForEach(c => c.CollideWithGem += () => player.AddGem());

            KeysAndGemsCounted.Invoke(numKeys, numGems);

            gameObjects.AddRange(new[]
            {
                new Wall(-SCREEN_WALL_WIDTH, 0, SCREEN_WALL_WIDTH, HEIGHT),
                new Wall(WIDTH, 0, SCREEN_WALL_WIDTH, HEIGHT),
                new Wall(0, -SCREEN_WALL_WIDTH, WIDTH, SCREEN_WALL_WIDTH),
                new Wall(0, HEIGHT, WIDTH, SCREEN_WALL_WIDTH)
            });

            gameObjects.ForEach(g => g.MoveReady += gameObject => MoveGameObject(gameObject));
            gameObjects.ForEach(g => g.DeleteReady += gameObject => gameObjects.Remove(gameObject));

            player.HitSpike += () =>
            {
                commands = new Queue<char>();
                RunComplete.Invoke("The player hit a spike : press ENTER to try again.");
            };

            player.KeyCollected += nk => playerKeyCollected.Invoke(nk);
            player.GemCollected += ng => playerGemCollected.Invoke(ng);

        }

        public void ReStartLevel()
        {
            LoadLevelFromFile($"{LEVEL_PATH_SUFFIX}{curLevel}.txt");
            if (startingNewLevel)
            {
                timer.Restart();
                startingNewLevel = false;
            }
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();
    }
}
