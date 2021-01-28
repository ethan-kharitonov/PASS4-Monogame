using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PASS4
{
    public class InputMenu : ISection
    {
        class LoopInfo
        {
            bool allowNestedLoops = false;

            private int startIndex = -1;
            private int numIterations;
            private LoopInfo nestedLoop;

            public int StartIndex { get => nestedLoop != null ? nestedLoop.StartIndex : startIndex; }

            public void StartLoop(int startIndex, int numIterations)
            {
                if (this.startIndex == -1)
                {
                    this.startIndex = startIndex;
                    this.numIterations = numIterations;
                }
                else
                {
                    if (!allowNestedLoops)
                    {
                        throw new Exception("Nested loops are not allowed");
                    }

                    if (nestedLoop == null)
                    {
                        nestedLoop = new LoopInfo();
                    }

                    nestedLoop.StartLoop(startIndex, numIterations);
                }
            }

            public bool DecrementNumIterations()
            {
                if (nestedLoop != null)
                {
                    if (nestedLoop.DecrementNumIterations())
                    {
                        return true;
                    }

                    nestedLoop = null;
                    return false;
                }

                if (numIterations == 0)
                {
                    Reset();
                    return false;
                }

                --numIterations;
                return true;

            }

            private void Reset()
            {
                startIndex = -1;
                nestedLoop = null;
            }

        }

        private enum Stage
        {
            Waiting,
            Input,
            Results,
            //Proccessing,
            WaitingForEnter
        }

        private Texture2D bgImg;

        private Stage stage = Stage.Input;
        public static readonly InputMenu Instance = new InputMenu();

        private string input = string.Empty;
        private string inputMessage = string.Empty;
        private string commandArrow = string.Empty;
        private float commandArrowXPosition;
        private Queue<int> commandOrder = new Queue<int>();
        private int curCommandNumber = 0;

        private Bar progressBar;

        private const int HEIGHT = 150;

        public event Action<Queue<char>> CommandReadingComplete;

        public event Action CommandReadingStarting;

        private bool showLegend = false;
        public bool ShowLegend => showLegend;

        private string numKeysDisplay;
        private string numGemsDisplay;

        public event Action<int> PlayerReadyToExistMainGame;

        private int numCommands = 0;
        private const int MAX_COMMANDS = 68;

        private Point margins = new Point(10, 10);
        private const int LINE_SPACING = 5;

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

        Predicate<char> isCharValidDigit = key =>
        {
            int keyNumber = Convert.ToInt32(key) - '0';
            return 0 < keyNumber && keyNumber < 10;
        };

        private bool waitingForEnter = false;
        private Action ActionOnEnter;

        private FinalResult finalResults = null;
        private LevelResult levelResults = null;

        private InputMenu()
        {
        }

        public void Reset()
        {
            stage = Stage.Input;
            numCommands = 0;
            showLegend = false;
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

        private Screen screen = new Screen(new Point(0, LevelContainer.HEIGHT), LevelContainer.WIDTH, HEIGHT);

        public void SetNumKeys(int totalNumKeys, int totalnumGems)
        {
            numKeysDisplay = $"0/{totalNumKeys} Keys";
            numGemsDisplay = $"0/{totalnumGems} Gems";
        }

        public void UpdateNumCollectedKeys(int numCollectedKeys)
        {
            numKeysDisplay = $"{numCollectedKeys}{numKeysDisplay.Substring(1, numKeysDisplay.Length - 1)}";
        }

        public void UpdateNumCollectedGems(int numCollectedGems)
        {
            numGemsDisplay = $"{numCollectedGems}{numGemsDisplay.Substring(1, numGemsDisplay.Length - 1)}";
        }

        public void LoadContent()
        {
            progressBar = new Bar(new Rectangle(10, 100, 200, 30));
            bgImg = Helper.LoadImage("Images/MenuBackground");
        }

        public void Update()
        {
            if (waitingForEnter)
            {
                if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                {
                    ActionOnEnter.Invoke();
                    waitingForEnter = false;
                }

                return;
            }

            switch (stage)
            {
                case Stage.Input:
                    if (Helper.KeysReleasedThisFrame.Contains(Keys.L))
                    {
                        showLegend = !showLegend;
                    }
                    input = string.Concat(Helper.UpdateStringWithInput(input).Where(k => validChars.Contains(k) || isCharValidDigit(k)).Take(MAX_COMMANDS));
                    numCommands = input.Length;

                    if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                    {
                        try
                        {
                            Queue<char> commands = ConvertInputIntoCommands(input);
                            CommandReadingComplete.Invoke(commands);
                            commandOrder.Enqueue(input.Length - 1);

                            progressBar.FullAmount = commandOrder.Count;
                            progressBar.Update(0);
                            showLegend = false;
                            stage = Stage.Waiting;
                        }
                        catch (Exception e)
                        {
                            inputMessage = e.Message + " : Please press ENTER to try again.";
                            waitingForEnter = true;
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

        public Queue<char> ConvertInputIntoCommands(string input)
        {
            Queue<char> commands = new Queue<char>();
            LoopInfo loopInfo = new LoopInfo();
            commandOrder.Clear();

            for (int i = 0; i < input.Length; ++i)
            {
                if (i < input.Length && input[i] == 'F')
                {
                    if (loopInfo.StartIndex == -1)
                    {
                        throw new FormatException("Loops must start with 'S'");
                    }

                    if (i == loopInfo.StartIndex)
                    {
                        throw new FormatException("Loops cannot be empty");
                    }

                    if (loopInfo.DecrementNumIterations())
                    {
                        i = loopInfo.StartIndex - 1;
                    }
                }
                else if (input[i] == 'S')
                {
                    if (i < input.Length - 1)
                    {
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

                    loopInfo.StartLoop(i + 2, Convert.ToInt32(input[i + 1]) - '0' - 1);
                    i += 1;
                }
                else if (input[i] == 'A' || input[i] == 'C' || input[i] == 'D' || input[i] == 'E' || input[i] == 'Q' || input[i] == '+' || input[i] == '-')
                {
                    commands.Enqueue(input[i]);
                    commandOrder.Enqueue(i);
                }
                else
                {
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

            if (loopInfo.StartIndex != -1)
            {
                throw new FormatException("All loops must be closed with an 'F'");
            }

            if (commands.IsEmpty)
            {
                throw new Exception("The input cannot be empty");
            }

            return commands;
        }

        public void Draw()
        {
            screen.Draw(bgImg, new Rectangle(0, 0, Main.WIDTH, HEIGHT));
            switch (stage)
            {
                case Stage.Input:
                    DrawOnLine($"Command: {input}", 0);
                    DrawOnLine($"{numCommands}/{MAX_COMMANDS} Commands", 1, false);
                    if (inputMessage != string.Empty)
                    {
                        DrawOnLine($"Error: {inputMessage}", 1);
                    }

                    DrawOnLine("X to Quit", 5);
                    DrawOnLine("L to Toggle Legend", 5, false);
                    break;
                case Stage.Waiting:
                    DrawOnLine($"Command: {input}", 0);
                    DrawOnLine("Action       : ", 1);
                    DrawOnLine(commandArrow, 1, (int)commandArrowXPosition);
                    DrawOnLine($"{numCommands}/{MAX_COMMANDS} Commands", 0, false);
                    DrawOnLine(numGemsDisplay, 2);
                    DrawOnLine(numKeysDisplay, 2, false);


                    progressBar.Draw(screen);
                    break;
                case Stage.Results:
                    DrawOnLine($"{inputMessage}", 0);

                    if(finalResults != null)
                    {
                        DrawOnLine($"Total Time: {finalResults.TotalTime} seconds, Total Score: {finalResults.TotalScore}", 0, false);

                        for (int i = 0; i < finalResults.LevelResults.Length; ++i)
                        {
                            DrawOnLine($"Level {i + 1}) Total Time: {finalResults.LevelResults[i].Time} seconds", i + 1);
                            DrawOnLine($"Total Score: {finalResults.LevelResults[i].Score}", i + 1, false);
                        }
                    }
                    else if(levelResults != null)
                    {
                        DrawOnLine($"Total Time: {levelResults.Time} seconds",1);
                        DrawOnLine($"Total Score: {levelResults.Score}", 1, false);

                    }

                    break;
            }
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();

        public void ShowResultsForRoundFailed(string message)
        {
            inputMessage = message;
            FinishRound();
        }

        private void FinishRound()
        {
            stage = Stage.Results;
            waitingForEnter = true;
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

        public void ShowResultsForRoundSuccess(LevelResult levelResults)
        {
            FinishRound();
            inputMessage = "Success! You've reached the goal : Press ENTER to continue to the next level.";
            this.levelResults = levelResults;
        }

        public void ShowResultsAllLevelsComplete(FinalResult finalResults)
        {
            inputMessage = "You've finised all the levels! : press ENTER to add name and save.";
            stage = Stage.Results;
            waitingForEnter = true;
            ActionOnEnter = () => PlayerReadyToExistMainGame.Invoke(finalResults.TotalScore);

            this.finalResults = finalResults;
        }

        public void ShowNextCommand()
        {
            if (commandArrow == string.Empty)
            {
                commandArrow = "^";
            }

            ++curCommandNumber;
            progressBar.Update(curCommandNumber);

            commandArrowXPosition = Helper.InputFont.MeasureString("Command:xx" + input.Substring(0, commandOrder.Dequeue())).X + 3;
        }

        private void DrawOnLine(string text, int lineNum, bool leftToRight = true)
            => screen.DrawText(Helper.InputFont, text, new Vector2(leftToRight ? margins.X : LevelContainer.WIDTH - margins.X - Helper.InputFont.MeasureString(text).X, (Helper.InputFont.MeasureString("S").Y + LINE_SPACING) * lineNum + margins.Y), Color.White);

        private void DrawOnLine(string text, int lineNum, int xPosition)
            => screen.DrawText(Helper.InputFont, text, new Vector2(xPosition, (Helper.InputFont.MeasureString("S").Y + LINE_SPACING) * lineNum + margins.Y), Color.White);
    }
}
