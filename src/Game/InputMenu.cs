//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: InputMenu.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Handles all user input during the game stage. Displays messages.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace PASS4
{
    public class InputMenu : ISection
    {
        /// <summary>
        /// Used to keep track of loops in the command string
        /// </summary>
        class LoopInfo
        {
            //Indicates that nested loops are not allowed
            bool allowNestedLoops = false;

            //The start index (on the string) and the number of iterations for this loop
            private int startIndex = -1;
            private int numIterations;

            //This loops nested loop
            private LoopInfo nestedLoop;

            /// <summary>
            /// Recursivly returns the start index of the most nested loop
            /// </summary>
            public int StartIndex { get => nestedLoop != null ? nestedLoop.StartIndex : startIndex; }

            /// <summary>
            /// Starts a new loop
            /// </summary>
            /// <param name="startIndex">The index on the string where the loop starts</param>
            /// <param name="numIterations">The number of iterations of the loop</param>
            public void StartLoop(int startIndex, int numIterations)
            {
                //If this loop is not started, start it, otherwise start the nested loop
                if (this.startIndex == -1)
                {
                    //Store start index and number of iterations
                    this.startIndex = startIndex;
                    this.numIterations = numIterations;
                }
                else
                {
                    //If attempted to start nested loops when not allowed, throw exception
                    if (!allowNestedLoops)
                    {
                        throw new Exception("Nested loops are not allowed");
                    }

                    //If the nested loop has not been made, make it
                    if (nestedLoop == null)
                    {
                        nestedLoop = new LoopInfo();
                    }

                    //Start the nested loop
                    nestedLoop.StartLoop(startIndex, numIterations);
                }
            }

            /// <summary>
            /// Decreament the number of iterations left
            /// </summary>
            /// <returns>True, if the loop has more iterations left, false otherwise</returns>
            public bool DecrementNumIterations()
            {
                //If there is a nested loop, decremeant it
                if (nestedLoop != null)
                {
                    if (nestedLoop.DecrementNumIterations())
                    {
                        return true;
                    }

                    nestedLoop = null;
                    return false;
                }

                //If there are no more iterations left reset the loop and return false
                if (numIterations == 0)
                {
                    Reset();
                    return false;
                }

                //decremeant and return true
                --numIterations;
                return true;

            }

            /// <summary>
            /// Resets the loop
            /// </summary>
            private void Reset()
            {
                //Resets the starting index and nested loop
                startIndex = -1;
                nestedLoop = null;
            }

        }

        /// <summary>
        /// Indicating the current stage of the input proccess
        /// </summary>
        private enum Stage
        {
            Waiting,
            Input,
            Results,
            WaitingForEnter
        }
        //Indicating the current stage of the input proccess
        private Stage stage = Stage.Input;

        //Stores the only instance of InputMenu (I did not want to make it static because I have it inside an array in game and putting static classes inside array seems wierd)
        public static readonly InputMenu Instance = new InputMenu();

        //Stores the height of the input menu
        private const int HEIGHT = 150;

        //Stores the background image of the menu
        private Texture2D bgImg;

        //Stores input and any message that might be written to the user
        private string input = string.Empty;
        private string inputMessage = string.Empty;

        //Stores the command arrow and its position
        private string commandArrow = string.Empty;
        private float commandArrowXPosition;

        //Stores queue which shows the index the arrow has to point at
        private Queue<int> commandOrder = new Queue<int>();
        private int curCommandNumber = 0;

        //Stores the progress bar that indicates how far along the excuting proccess is
        private Bar progressBar;

        //Invoked when the user is starting to inptut and when the user has inputed a valid command swquance and 
        public event Action CommandReadingStarting;
        public event Action<Queue<char>> CommandReadingComplete;

        /// <summary>
        /// True if the legend should be displayed, false otherwise
        /// </summary>
        public bool ShowLegend { get; private set; } = false;

        //Stores the string that informs the user how much gems and keys they have
        private string numKeysDisplay;
        private string numGemsDisplay;

        //Invoked when the player is ready to leave the game stage
        public event Action<int> PlayerReadyToExistMainGame;

        //Stores the number of commands the user has inputed and the max number they can input
        private int numCommands = 0;
        private const int MAX_COMMANDS = 68;

        //Stores the margins from the side and top of the menu as well as the space between lines
        private Point margins = new Point(10, 10);
        private const int LINE_SPACING = 5;

        //Stores all the valid charachters the user can input (excluding numbers)
        private char[] validChars = new[]
        {
            'A',
            'D',
            'S',
            'F',
            'Q',
            'E',
            '+',
            '-',
            'C'
        };

        /// <summary>
        /// Returns true the charachter is a digit bigger than 0, false otherwise
        /// </summary>
        Predicate<char> isCharValidDigit = key =>
        {
            int keyNumber = Convert.ToInt32(key) - '0';
            return 0 < keyNumber && keyNumber < 10;
        };

        //True if the input menu is waiting for the user to press enter. The action is what will happen once they press enter
        private bool waitingForEnter = false;
        private Action ActionOnEnter;

        //Information about each level and full game that is desplayed between levels and at the end
        private FinalResult finalResults = null;
        private LevelResult levelResults = null;

        //invoked when player presses x
        public event Action QuitGame;

        private InputMenu()
        {
        }

        /// <summary>
        /// Resets all the values of the input menu
        /// </summary>
        public void Reset()
        {
            stage = Stage.Input;
            numCommands = 0;
            ShowLegend = false;
            progressBar.Update(0);
            curCommandNumber = 0;
            commandOrder.Clear();
            commandArrow = string.Empty;
            inputMessage = string.Empty;
            input = string.Empty;
            CommandReadingStarting.Invoke();
            finalResults = null;
            levelResults = null;
        }


        //The screen that the input menu draws to
        private Screen screen = new Screen(new Point(0, LevelContainer.HEIGHT), LevelContainer.WIDTH, HEIGHT);

        /// <summary>
        /// Tells the input menu how many keys and gems there are in a level
        /// </summary>
        /// <param name="totalNumKeys">Number of keys</param>
        /// <param name="totalnumGems">Number of gems</param>
        public void SetNumKeys(int totalNumKeys, int totalnumGems)
        {
            //Makes the display strings
            numKeysDisplay = $"0/{totalNumKeys} Keys";
            numGemsDisplay = $"0/{totalnumGems} Gems";
        }

        /// <summary>
        /// Updates the number of keys the user has
        /// </summary>
        /// <param name="numCollectedKeys">new number</param>
        public void UpdateNumCollectedKeys(int numCollectedKeys)
        {
            numKeysDisplay = $"{numCollectedKeys}{numKeysDisplay.Substring(1, numKeysDisplay.Length - 1)}";
        }

        /// <summary>
        /// Updates the number of gems the user has
        /// </summary>
        /// <param name="numCollectedGems">new number</param>
        public void UpdateNumCollectedGems(int numCollectedGems)
        {
            numGemsDisplay = $"{numCollectedGems}{numGemsDisplay.Substring(1, numGemsDisplay.Length - 1)}";
        }

        /// <summary>
        /// Loads the images and progress bar for the input menu
        /// </summary>
        public void LoadContent()
        {
            progressBar = new Bar(new Rectangle(10, 100, 200, 30));
            bgImg = Helper.LoadImage("Images/MenuBackground");
        }

        /// <summary>
        /// Updates the input menu
        /// </summary>
        public void Update()
        {
            //If currently waiting for enter go in here then exit update
            if (waitingForEnter)
            {
                //if enter is pressed invoke the on enter action and record that no longer waiting for enter
                if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                {
                    ActionOnEnter.Invoke();
                    waitingForEnter = false;
                }

                return;
            }

            //Runs the code corresponding to each stage
            switch (stage)
            {
                case Stage.Input:
                    //If L is pressed toggle the legend
                    if (Helper.KeysReleasedThisFrame.Contains(Keys.L))
                    {
                        ShowLegend = !ShowLegend;
                    }

                    if (Helper.KeysReleasedThisFrame.Contains(Keys.X))
                    {
                        QuitGame.Invoke();
                    }

                    //update the input and limit it to only valid keys and digits. Update the number of commands
                    input = string.Concat(Helper.UpdateStringWithInput(input).Where(k => validChars.Contains(k) || isCharValidDigit(k)).Take(MAX_COMMANDS));
                    numCommands = input.Length;

                    //If the user pressed enter, read their input
                    if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                    {
                        try
                        {
                            //Get the queue of commands and invoke event signaling command reading is done
                            Queue<char> commands = ConvertInputIntoCommands(input);
                            CommandReadingComplete.Invoke(commands);

                            //Enqueue the last command again for the arrow display
                            commandOrder.Enqueue(input.Length - 1);

                            //set the full amount of the progress bar and set its current amount to 0
                            progressBar.FullAmount = commandOrder.Count;
                            progressBar.Update(0);

                            //toggle legend off and set stage to waiting
                            ShowLegend = false;
                            stage = Stage.Waiting;
                        }
                        catch (Exception e)
                        {
                            //notify the user why their input has failed and set waiting for enter to true
                            inputMessage = e.Message + " : Please press ENTER to try again.";
                            waitingForEnter = true;

                            //Set the on enter action to restart the input process
                            ActionOnEnter = () =>
                            {
                                input = string.Empty;
                                inputMessage = string.Empty;
                            };
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Converts the user entered command string into a queue of commands
        /// </summary>
        /// <param name="input">The command string</param>
        /// <returns>The command queue</returns>
        public Queue<char> ConvertInputIntoCommands(string input)
        {
            //create a new queue and loop infor and reset the command order (for the arrow)
            Queue<char> commands = new Queue<char>();
            LoopInfo loopInfo = new LoopInfo();
            commandOrder.Clear();

            //check each key for validity
            for (int i = 0; i < input.Length; ++i)
            {
                //Check that there are charachters left in input before checking for charachters
                if (i < input.Length && input[i] == 'F')
                {
                    //Invalid if no loop is open
                    if (loopInfo.StartIndex == -1)
                    {
                        throw new FormatException("Loops must start with 'S'");
                    }

                    //invalid if the end of the loop is the start of the loop
                    if (i == loopInfo.StartIndex)
                    {
                        throw new FormatException("Loops cannot be empty");
                    }

                    //Decremeant the number of iterations if the loop is valid
                    if (loopInfo.DecrementNumIterations())
                    {
                        //go back to the biggining of the loop
                        i = loopInfo.StartIndex - 1;
                    }
                }
                else if (input[i] == 'S')
                {
                    //check that there is space left for a loop body, throw exception if not
                    if (i < input.Length - 1)
                    {
                        //convert the next char to a number and throw acception if it is not valid
                        int num = Convert.ToInt32(input[i + 1]) - '0';
                        if (num < 0 || num > 9)
                        {
                            throw new FormatException("All loop starts (S) must be followed with a digit (1 - 9)");
                        }
                    }
                    else
                    {
                        throw new FormatException("All loop starts (S) must be followed with a digit (1 - 9)");
                    }

                    //Start the loop if it is valid
                    loopInfo.StartLoop(i + 2, Convert.ToInt32(input[i + 1]) - '0' - 1);
                    i += 1;
                }
                else if (input[i] == 'A' || input[i] == 'C' || input[i] == 'D' || input[i] == 'E' || input[i] == 'Q' || input[i] == '+' || input[i] == '-')
                {
                    //Add the command if it is valid
                    commands.Enqueue(input[i]);
                    commandOrder.Enqueue(i);
                }
                else
                {
                    //if invalid charachters check that they are either numbers after a loop (> 9) or just invalid
                    if (i > 0)
                    {
                        int curNum = Convert.ToInt32(input[i]) - '0';
                        int lastNum = Convert.ToInt32(input[i - 1]) - '0';
                        if (Helper.IsBetween(1, curNum, 9) && Helper.IsBetween(1, lastNum, 9))
                        {
                            throw new FormatException("Loops can have a maximum of 9 iterations");
                        }
                    }
                    throw new FormatException($"{input[i]} is not a valid input charchter");
                }
            }

            //throw exception if open loop
            if (loopInfo.StartIndex != -1)
            {
                throw new FormatException("All loops must be closed with an 'F'");
            }

            //throw exception if no commands
            if (commands.IsEmpty)
            {
                throw new Exception("The input cannot be empty");
            }

            //Return the command queue
            return commands;
        }

        /// <summary>
        /// Draws the input menu
        /// </summary>
        public void Draw()
        {
            //Draw the background
            screen.Draw(bgImg, new Rectangle(0, 0, Main.WIDTH, HEIGHT));

            //draw the correct stage
            switch (stage)
            {
                case Stage.Input:
                    //draw the commands and number of commands
                    DrawOnLine($"Command: {input}", 0);
                    DrawOnLine($"{numCommands}/{MAX_COMMANDS} Commands", 1, false);

                    //draw error if there is one
                    if (inputMessage != string.Empty)
                    {
                        DrawOnLine($"Error: {inputMessage}", 1);
                    }

                    //draw the quit and toggle legend options
                    DrawOnLine("X to Quit", 5);
                    DrawOnLine("L to Toggle Legend", 5, false);
                    break;
                case Stage.Waiting:
                    //draw the commands and the current action
                    DrawOnLine($"Command: {input}", 0);
                    DrawOnLine("Action       : ", 1);
                    DrawOnLine(commandArrow, 1, (int)commandArrowXPosition);
                    DrawOnLine($"{numCommands}/{MAX_COMMANDS} Commands", 0, false);

                    //display the number of gems and keys
                    DrawOnLine(numGemsDisplay, 2);
                    DrawOnLine(numKeysDisplay, 2, false);

                    //draw the bar
                    progressBar.Draw(screen);

                    break;
                case Stage.Results:
                    //draw the message
                    DrawOnLine($"{inputMessage}", 0);

                    //if beat last leve display all the scores
                    if (finalResults != null)
                    {
                        DrawOnLine($"Total Time: {finalResults.TotalTime} seconds, Total Score: {finalResults.TotalScore}", 0, false);

                        for (int i = 0; i < finalResults.LevelResults.Length; ++i)
                        {
                            DrawOnLine($"Level {i + 1}) Total Time: {finalResults.LevelResults[i].Time} seconds", i + 1);
                            DrawOnLine($"Total Score: {finalResults.LevelResults[i].Score}", i + 1, false);
                        }
                    }
                    else if (levelResults != null)
                    {
                        //if beat any level display this levels scores
                        DrawOnLine($"Total Time: {levelResults.Time} seconds", 1);
                        DrawOnLine($"Total Score: {levelResults.Score}", 1, false);
                    }

                    break;
            }
        }

        /// <summary>
        /// gets the biggest x point of the menu
        /// </summary>
        /// <returns>the biggest x point of the menu</returns>
        public int GetMaxX() => screen.GetMaxX();

        /// <summary>
        /// gets the biggest y point of the menu
        /// </summary>
        /// <returns>the biggest y point of the menu</returns>
        public int GetMaxY() => screen.GetMaxY();

        /// <summary>
        /// Handles loosing a command squance
        /// </summary>
        /// <param name="message"></param>
        public void ShowResultsForRoundFailed(string message)
        {
            inputMessage = message;
            FinishRound();
        }

        /// <summary>
        /// ends the round
        /// </summary>
        private void FinishRound()
        {
            //goes to results and waits for enter
            stage = Stage.Results;
            waitingForEnter = true;

            //resets everything on enter
            ActionOnEnter = () =>
            {
                stage = Stage.Input;
                inputMessage = string.Empty;
                input = string.Empty;
                curCommandNumber = 0;
                CommandReadingStarting.Invoke();
                commandArrow = string.Empty;
            };
        }

        /// <summary>
        /// handles showing results for beating a level
        /// </summary>
        /// <param name="levelResults">the stats for that level</param>
        public void ShowResultsForRoundSuccess(LevelResult levelResults)
        {
            //finish the round and update the messge
            FinishRound();
            inputMessage = "Success! You've reached the goal : Press ENTER to continue to the next level.";

            //store results
            this.levelResults = levelResults;
        }

        /// <summary>
        /// Handle input for beating all levels
        /// </summary>
        /// <param name="finalResults"></param>
        public void ShowResultsAllLevelsComplete(FinalResult finalResults)
        {
            //update messag and go to resuls
            inputMessage = "You've finised all the levels! : press ENTER to add name and save.";
            stage = Stage.Results;

            //go to menu on enter
            waitingForEnter = true;
            ActionOnEnter = () => PlayerReadyToExistMainGame.Invoke(finalResults.TotalScore);

            //store final results
            this.finalResults = finalResults;
        }

        /// <summary>
        /// shows the next command
        /// </summary>
        public void ShowNextCommand()
        {
            //if string is empty assign it to ^
            if (commandArrow == string.Empty)
            {
                commandArrow = "^";
            }

            //move to the next command and update the bar
            ++curCommandNumber;
            progressBar.Update(curCommandNumber);

            //calculate the new position of the arrow
            commandArrowXPosition = Helper.InputFont.MeasureString("Command:xx" + input.Substring(0, commandOrder.Dequeue())).X + 3;
        }

        /// <summary>
        /// Draws text to a line on screen
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="lineNum">the linenumber</param>
        /// <param name="leftToRight">the side to draw from</param>
        private void DrawOnLine(string text, int lineNum, bool leftToRight = true)
            => screen.DrawText(Helper.InputFont, text, new Vector2(leftToRight ? margins.X : LevelContainer.WIDTH - margins.X - Helper.InputFont.MeasureString(text).X, (Helper.InputFont.MeasureString("S").Y + LINE_SPACING) * lineNum + margins.Y), Color.White);

        /// <summary>
        /// Draw on a line on the screen
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="lineNum">line number</param>
        /// <param name="xPosition">x position on that line</param>
        private void DrawOnLine(string text, int lineNum, int xPosition)
            => screen.DrawText(Helper.InputFont, text, new Vector2(xPosition, (Helper.InputFont.MeasureString("S").Y + LINE_SPACING) * lineNum + margins.Y), Color.White);
    }
}
