//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: LevelContainer.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Handles everything about the game component of the program
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PASS4
{
    /// <summary>
    /// Used to store information about the players performance during the game (all levels)
    /// </summary>
    public class FinalResult
    {
        //final stats
        public int TotalScore = 0;
        public int TotalTime = 0;

        //Information about every level
        public LevelResult[] LevelResults = new LevelResult[LevelContainer.NUM_LEVELS];
    }

    /// <summary>
    /// Used to store information about the players performance durign one level
    /// </summary>
    public class LevelResult
    {
        public int Score;
        public int Time;

        /// <summary>
        /// Creates a new instance of LevelResult
        /// </summary>
        /// <param name="score">The players score during that level</param>
        /// <param name="time">The time it took the player to complete the level</param>
        public LevelResult(int score, int time)
        {
            Score = score;
            Time = time;
        }
    }

    class LevelContainer : ISection
    {
        /// <summary>
        /// Sotres the only instance instance of this class (private constructor)
        /// </summary>
        public static readonly LevelContainer Instance = new LevelContainer();

        //Stores information about the players performance during all the levels
        private FinalResult finalResult = new FinalResult();

        //Invoked when the player is done excecuting a command
        public event Action ExecutingNextCommand;

        //Invoked when the player finished executing all commands but did not win the level
        public event Action<string> RunCompleteFailed;

        //Invoked when the player beat the level
        public event Action<LevelResult> RunCompleteSuccess;

        //Invoked when the player beat all the levels
        public event Action<FinalResult> AllLevelsComplete;

        //Invoked when the number of collectables is known and every time a player collects a key or gem
        public Action<int, int> KeysAndGemsCounted;
        public Action<int> playerKeyCollected;
        public Action<int> playerGemCollected;
        
        //Stores the number of gems and keys the player has collected in the current level
        private int numGems = 0;
        private int numKeys = 0;

        //Stores the background image of the game
        private static Texture2D backgroundImg;

        //Sotres the gameobjects in the current level and has a refrence to the player
        private List<GameObject> gameObjects = new List<GameObject>();
        private Player player;

        //Stores the position and image of the flag
        private Point flagPos;
        private Texture2D flagImg;

        //Stores the width of the wall blocking the player from leaving the screen
        private const int SCREEN_WALL_WIDTH = 3;

        //Stores the number of tile on on each axis
        private const int NUM_CELLS_WIDTH = 20;
        private const int NUM_CELLS_HEIGHT = 9;

        //Stores the side length of each tile
        public const int CELL_SIDE_LENGTH = 45;

        //Calculates and stores the width and height of the game portion of the screen (not input)
        public const int WIDTH = NUM_CELLS_WIDTH * CELL_SIDE_LENGTH;
        public const int HEIGHT = NUM_CELLS_HEIGHT * CELL_SIDE_LENGTH;

        //Stores the screen to which the level is drawn to
        private Screen screen = new Screen(new Point(0, 0), WIDTH, HEIGHT);

        //stores all the commands that the player needs to excecute and the number of commands it is
        private Queue<char> commands = new Queue<char>();
        private int numCommands;

        //Stores the file path to the level folder as well as the current level number, the number of level, and a bool indicating if a new level has just started
        private const string LEVEL_PATH_SUFFIX = "Levels/Level";
        private int curLevel = 1;
        public const int NUM_LEVELS = 5;
        private bool startingNewLevel = true;

        //A timer used to time the player on each level
        public static Stopwatch timer = new Stopwatch();

        //Indicates if the game is currently paused
        private bool GamePaused = true;

        private LevelContainer()
        {

        }

        /// <summary>
        /// Loads the images and starts up the first level and timer
        /// </summary>
        public void LoadContent()
        {
            //loads the flag and background images
            flagImg = Helper.LoadImage("Images/flagImg");
            backgroundImg = Helper.LoadImage("Images/backGround");

            //Loads the level map and starts the timer
            LoadLevelFromFile($"{LEVEL_PATH_SUFFIX}{curLevel}.txt");
            timer.Start();
        }

        /// <summary>
        /// Resets all the variables before starting a new level
        /// </summary>
        public void Reset()
        {
            GamePaused = true;
            numCommands = 0;
            timer.Start();
            numKeys = 0;
            numGems = 0;
            curLevel = 1;
            ReStartLevel();
            commands.Clear();
        }

        /// <summary>
        /// Loads and saves the commands entered by the user
        /// </summary>
        /// <param name="commands">A queue of the commands in the order that they should be excecuted</param>
        public void LoadCommands(Queue<char> commands)
        {
            //Indicates the game is starting and saves the commands
            GamePaused = false;
            this.commands = commands;
            numCommands = commands.Count;

            //Enques a random command at the end because it will cause the players load command to be called after the last command 
            //(even though X will not be excecuted). I need the function to run once more at the end
            commands.Enqueue('X');
        }

        /// <summary>
        /// Updates all the gameobjects in the level
        /// </summary>
        public void Update()
        {
            //If the game is paused dont update anything
            if (GamePaused)
            {
                return;
            }

            //If all the objects are standing still and there is more commands left, run them
            if (!commands.IsEmpty && gameObjects.All(g => g.IsStandingStill()))
            {
                //give player the next command and notify Game that the next command is being excecuted
                player.LoadNextCommand(commands.Dequeue());
                ExecutingNextCommand.Invoke();

                //If the command just loaded is the last command check if the player won
                if (commands.IsEmpty)
                {
                    //pause the game
                    GamePaused = true;

                    //check if the player has reached the flag check if they collected all the gems
                    if (player.Box.Location == flagPos)
                    {
                        //if they did not collect all the gems then inform Game they failed the level for that reason
                        if (player.GemCount != numGems)
                        {
                            RunCompleteFailed.Invoke("You did not collect all the gems : press ENTER to try again.");
                        }
                        //the player won
                        else
                        {
                            //stop the timer
                            timer.Stop();

                            //Save the results for this level and update the final results
                            finalResult.LevelResults[curLevel - 1] = new LevelResult(timer.Elapsed.Milliseconds + numCommands * 100, timer.Elapsed.Seconds);
                            finalResult.TotalTime += finalResult.LevelResults[curLevel - 1].Time;
                            finalResult.TotalScore += finalResult.LevelResults[curLevel - 1].Score;

                            //check if that was the last level
                            if(curLevel == NUM_LEVELS)
                            {
                                //inform Game all levels complete and send it final reslts
                                AllLevelsComplete.Invoke(finalResult);
                            }
                            else
                            {
                                //inform Game that the level has been complete and send the correct level results
                                RunCompleteSuccess.Invoke(finalResult.LevelResults[curLevel - 1]);

                                //go to the next level and indicate that a new level is starting
                                ++curLevel;
                                startingNewLevel = true;
                            }
                        }
                    }
                    //the player did not reach the goal
                    else
                    {
                        //Inform Game that the level did not reach the goal
                        RunCompleteFailed.Invoke("A'w Shuc'ks Buddy ol Pal! Failed to reach goal : press ENTER to try again.");
                    }
                }
            }

            //Update all the gameobjects
            gameObjects.ForEach(g => g.Update());

            //Try to move all the gameobjects (while taking collision into account)
            MoveGameObjects();
        }

        /// <summary>
        /// Draw all the level
        /// </summary>
        public void Draw()
        {
            //Draw the background image
            screen.Draw(backgroundImg, new Rectangle(0, 0, WIDTH, HEIGHT));

            //Draw all the gameobjects and the flag
            gameObjects.ForEach(g => g.Draw(screen));
            screen.Draw(flagImg, new Rectangle(flagPos.X + 17, flagPos.Y, (int)Math.Round((double)(flagImg.Width * CELL_SIDE_LENGTH / flagImg.Height)), CELL_SIDE_LENGTH));
        }

        /// <summary>
        /// Moves all the gameobjects
        /// </summary>
        private void MoveGameObjects()
        {
            //loop backwards because some objects might be removed from the list during this proccess
            for (int i = gameObjects.Count - 1; i >= 0; --i)
            {
                //Move the current gameobject
                MoveGameObject(gameObjects[i]);
            }
        }

        /// <summary>
        /// Given an object, move it accourding to its velocity
        /// </summary>
        /// <param name="gameObject">The gameobject to move</param>
        private void MoveGameObject(GameObject gameObject)
        {
            //Dont move it if its velocity is zero
            if (gameObject.Velocity == Vector2.Zero)
            {
                return;
            }

            //Calculate the component of the velocity vector which the gameobject can move before colliding and then move it by that amount
            gameObject.Move(RestrictVelocity(gameObject, gameObject.Velocity));
        }

        /// <summary>
        /// Calculates the part of the velocity vector a given object can move by without colliding with any other objects
        /// </summary>
        /// <param name="gameObject">The gameobject that needs to be moved</param>
        /// <param name="wantedVelocity">The velocity the gameobject wants to move by</param>
        /// <param name="anotherPass">Indicates wheter this function should be called recursivly after restricting the velocity once (after finding collision on X, there could be a new collision on the Y)</param>
        /// <returns>The possible velcity the player should be moved by</returns>
        private Vector2 RestrictVelocity(GameObject gameObject, Vector2 wantedVelocity, bool anotherPass = true)
        {
            //Stores information about the collision that will happen first (Collisions that happen after will not actually happen) and stores the object it collides with
            RayCollisionInfo firstCollision;
            GameObject collidedGameObject;

            //Gets the collision info and the collided object
            (firstCollision, collidedGameObject) = FindCollision(gameObject, wantedVelocity);

            //If no collision happened (a collision on no sides). it is possible that an object passed through this object without hitting any of the corners
            ///<see cref="FindCollision"/>
            if (firstCollision.Sides.Count == 0)
            {
                //Get the rectangle where the object will be next frame
                Rectangle nextFrameRec = gameObject.Box;
                nextFrameRec.Location += wantedVelocity.ToPoint();

                //Check collision with all other gameObjectobjects
                foreach (GameObject otherGameObject in gameObjects)
                {
                    //Dont check collision with itself
                    if (gameObject == otherGameObject)
                    {
                        continue;
                    }

                    //If there is a collision (it must be on one of the sides - no corners)
                    if (nextFrameRec.Intersects(otherGameObject.Box))
                    {
                        //save the object collided with
                        collidedGameObject = otherGameObject;

                        //Record the collision info about the collision that happened (check which side it happened on and get the distance)
                        //moves the box back by some number to see what side it came from (10 is arbitrary)
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

            //Resolve the collision if there was (some sides were collided)
            if (firstCollision.Sides.Count != 0)
            {
                //save the rectangle of the collided gameobject before making the two objects react to the collision
                Rectangle collidedObjectBox = collidedGameObject.Box;

                //make both objects inform collision to eachother
                collidedGameObject.InformCollisionTo(gameObject, firstCollision.Sides.Select(s => s.Flip()));
                gameObject.InformCollisionTo(collidedGameObject, firstCollision.Sides);

                //if the collided gameobject is no longer there then the collision we found is no longer valid
                if (!gameObjects.Contains(collidedGameObject) || collidedGameObject.Box != collidedObjectBox)
                {
                    //Calculate new colllision
                    return RestrictVelocity(gameObject, wantedVelocity);
                }

                //If the collided object is not collidable (gem or key), dont procced to resolve the collision
                if (!collidedGameObject.IsCollidable)
                {
                    return wantedVelocity;
                }

                //Resolve any collision on the x
                if (firstCollision.Sides.Contains(Side.Left) || firstCollision.Sides.Contains(Side.Right))
                {
                    //set the objects X velocity to zero (because they collided on the X)
                    gameObject.Velocity.X = 0;

                    //calculate his possible X velocity as the X distance between the object and the collided gameobject
                    if(wantedVelocity.X != 0)
                    {
                        wantedVelocity.X = firstCollision.Distance.X;
                    }

                }

                //Resolve any collision on the Y
                if (firstCollision.Sides.Contains(Side.Top) || firstCollision.Sides.Contains(Side.Bottom))
                {
                    //set the objects Y velocity to zero (because they collided on the Y)
                    gameObject.Velocity.Y = 0;

                    //calculate his possible Y velocity as the Y distance between the object and the collided gameobject
                    if (wantedVelocity.Y != 0)
                    {
                        wantedVelocity.Y = firstCollision.Distance.Y;

                    }
                }

                //If we restricted the velocity, we now have a brand new velocity vector that can still collide on the other axis (restricted on X, can still collide moving on Y with new vector)
                if (anotherPass)
                {
                    //Check collision again (for the last time because after this there are no more axis that they have a new vector on)
                    return RestrictVelocity(gameObject, wantedVelocity, false);
                }
            }

            //Return the possible velocity
            return wantedVelocity;
        }


        /// <summary>
        /// Finds the first collision that will accour for a gameobject moving at a certain velocity
        /// </summary>
        /// <param name="gameObject">The gameobject that will be checked</param>
        /// <param name="wantedVelocity">The velocuty that object is moving by</param>
        /// <returns>Information about the collison and a refrence to the gameobject it collided with</returns>
        private (RayCollisionInfo, GameObject collidedGameObject) FindCollision(GameObject gameObject, Vector2 wantedVelocity)
        {
            //Stores the gameobject collided with, information about the first (actual) collision and the current collision being checked
            GameObject collidedGameObject = null;
            RayCollisionInfo firstCollision = new RayCollisionInfo();
            RayCollisionInfo curCollision;

            //Checks collison with all other gameobjects
            foreach (GameObject otherGameObject in gameObjects)
            {
                //dont check collision with itself
                if (gameObject == otherGameObject)
                {
                    continue;
                }

                //Shoot out a 'ray' from each corner of the gameoobject in the length and direction of the velocity vector
                //check collison between that ray and the other gameobject
                foreach (Vector2 rayStartPoint in Helper.GetVertecies(gameObject.Box))
                {
                    //Get information about the ray box collison
                    curCollision = Helper.RayBoxFirstCollision(new Line(rayStartPoint, rayStartPoint + wantedVelocity), otherGameObject.Box);

                    //Remove any impossible collisions (false positives)
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


                    //Check if that collision will be the first collision to happen
                    //if it is the shortest distance or collides on less sides (dont consider corner collisions if there is another collison that happenes at the same time not on a corner)
                    if (curCollision.Sides.Count != 0 && (firstCollision.Sides.Count == 0 || curCollision.Distance.Length() < firstCollision.Distance.Length()
                        || (curCollision.Distance.Length() == firstCollision.Distance.Length() && curCollision.Sides.Count < firstCollision.Sides.Count)))
                    {
                        //saves the current collision and the collided gameobject
                        collidedGameObject = otherGameObject;
                        firstCollision = curCollision;
                    }
                }
            }

            //returns the information about the collision
            return (firstCollision, collidedGameObject);
        }


        /// <summary>
        /// Loads all the gameobject in the level
        /// </summary>
        /// <param name="path">the level file path</param>
        private void LoadLevelFromFile(string path)
        {
            //Stores the number of keys in th elevel
            numKeys = 0;
            numGems = 0;

            //removes all the gameobjects from last level
            gameObjects.Clear();

            //stores all the lines in the file
            string[] lines = File.ReadAllLines(path);

            //loops over all the charachters
            for (int r = 0; r < NUM_CELLS_HEIGHT; ++r)
            {
                for (int c = 0; c < NUM_CELLS_WIDTH; ++c)
                {
                    //adds the corresponding gameobject
                    switch (lines[r][c])
                    {
                        case '0':
                            //save an addition refrence to the player before adding it
                            player = new Player(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH);
                            gameObjects.Add(player);
                            break;
                        case '1':
                            gameObjects.Add(new Wall(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH));
                            break;
                        case '2':
                            //Saves a refrance to the crate so it could subscribe to events
                            Crate crate = new Crate(c * CELL_SIDE_LENGTH, r * CELL_SIDE_LENGTH);

                            //Check that there are no gems or keys above whenever crate moves
                            crate.CrateMove += cr =>
                            {
                                IEnumerable<GameObject> objectsAbove = gameObjects.Where(g => Helper.IsPointInOrOnRectangle(g.TopLeftGridPoint.ToVector2(), new Rectangle(cr.Box.Location - new Point(0, cr.Box.Height), cr.Box.Size)));
                                return objectsAbove.Count() != 0 && (objectsAbove.First() is Gem || objectsAbove.First() is Key);
                            };
                            gameObjects.Add(crate);
                            break;
                        case '3':
                            //save the position of the flag
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

            //subscribe to the crate event for colliding with gems and keys (add them to the player count)
            gameObjects.OfType<Crate>().ToList().ForEach(c => c.CollideWithGem += () => player.AddGem());
            gameObjects.OfType<Crate>().ToList().ForEach(c => c.CollideWithKey += () => player.AddKey());

            //Notify Game of how many collectibles there are this level
            KeysAndGemsCounted.Invoke(numKeys, numGems);

            //add the 4 walls around the screem
            gameObjects.AddRange(new[]
            {
                new Wall(-SCREEN_WALL_WIDTH, 0, SCREEN_WALL_WIDTH, HEIGHT),
                new Wall(WIDTH, 0, SCREEN_WALL_WIDTH, HEIGHT),
                new Wall(0, -SCREEN_WALL_WIDTH, WIDTH, SCREEN_WALL_WIDTH),
                new Wall(0, HEIGHT, WIDTH, SCREEN_WALL_WIDTH)
            });

            //subscribe to the move and delete events of all the gameobjects
            gameObjects.ForEach(g => g.MoveReady += gameObject => MoveGameObject(gameObject));
            gameObjects.ForEach(g => g.DeleteReady += gameObject => gameObjects.Remove(gameObject));

            //subscribe to the players hit spike event with a function that ends the level
            player.HitSpike += () =>
            {
                //clears commands and pauses the game
                commands = new Queue<char>();
                GamePaused = true;

                //Notify game that the player hit a spike
                RunCompleteFailed.Invoke("The player hit a spike : press ENTER to try again.");
            };

            //subscribe to the players collect gem and key events (notify Game when happenes)
            player.KeyCollected += nk => playerKeyCollected.Invoke(nk);
            player.GemCollected += ng => playerGemCollected.Invoke(ng);

        }

        /// <summary>
        /// Restarts the level by loading the map and starting timer
        /// </summary>
        public void ReStartLevel()
        {
            //load the map
            LoadLevelFromFile($"{LEVEL_PATH_SUFFIX}{curLevel}.txt");

            if (startingNewLevel)
            {
                timer.Restart();
                startingNewLevel = false;
            }
        }

        /// <summary>
        /// Gets the highest X value the screen reaches
        /// </summary>
        /// <returns>The highest X value the screen reaches</returns>
        public int GetMaxX() => screen.GetMaxX();

        /// <summary>
        /// Gets the highest Y value the screen reaches
        /// </summary>
        /// <returns>The highest Y value the screen reaches</returns>
        public int GetMaxY() => screen.GetMaxY();
    }
}
