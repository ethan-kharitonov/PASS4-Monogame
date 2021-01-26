using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
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
            Input,
            Proccessing,
            Waiting
        }

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

        public delegate void TakeCommands(Queue<char> commands);
        public event TakeCommands CommandReadingComplete;

        public delegate void Notify();
        public event Notify CommandReadingStarting;

        private bool inputFailed = false;

        private bool showLegend = false;
        public bool ShowLegend => showLegend;

        private string numKeysDisplay;
        private string numGemsDisplay;

        private bool lastLevelComplete = false;

        public event Action PlayerReadyToExistMainGame;

        private int numCommands = 0;
        private const int MAX_COMMANDS = 70;

        private Point margins = new Point(10, 10);
        private int lineSpacing = 5;

        private InputMenu()
        {
        }

        private Screen screen = new Screen(new Point(0, GameView.HEIGHT), GameView.WIDTH, HEIGHT);
        
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
        }

        public void Update()
        {
            switch (stage)
            {
                case Stage.Input:
                    if (Helper.KeysReleasedThisFrame.Contains(Keys.L))
                    {
                        Helper.KeysReleasedThisFrame.Remove(Keys.L);
                        showLegend = !showLegend;
                    }

                    if (inputFailed)
                    {
                        if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                        {
                            Helper.KeysReleasedThisFrame.Remove(Keys.Enter);
                            
                            if (lastLevelComplete)
                            {
                                PlayerReadyToExistMainGame.Invoke();
                            }
                            else
                            {
                                input = string.Empty;
                                inputMessage = string.Empty;
                                commandArrow = string.Empty;
                                CommandReadingStarting.Invoke();
                                progressBar.Reset();
                                numCommands = 0;
                            }
                            inputFailed = false;
                        }
                        else
                        {
                            break;
                        }

                    }

                    for (int i = Helper.KeysReleasedThisFrame.Count - 1; i >= 0; --i)
                    {
                        Keys key = Helper.KeysReleasedThisFrame[i];
                        if (numCommands >= MAX_COMMANDS && !(key == Keys.Enter || key == Keys.Back))
                        {
                            continue;
                        }

                        Helper.KeysReleasedThisFrame.Remove(key);
                        if (key == Keys.Back)
                        {
                            if (input.Count() == 0)
                            {
                                continue;
                            }

                            input = input.Substring(0, input.Count() - 1);
                        }
                        else if (key == Keys.OemPlus)
                        {
                            input += "+";
                        }
                        else if (key == Keys.OemMinus)
                        {
                            input += "-";
                        }
                        else if (key == Keys.Enter)
                        {
                            //input = "s4qcfs5dfeeces8dfeedcqs6afs4qfa+eecedc+aqa+++eec++s4af+++dee".ToUpper();
                            stage = Stage.Proccessing;
                            --numCommands;
                        }
                        else
                        {
                            if (key.ToString().Count() > 1)
                            {
                                int num = Convert.ToInt32(key.ToString()[1]) - '0';
                                if (0 < num && num < 10)
                                {
                                    input += key.ToString()[1];
                                }
                                continue;
                            }

                            input += key.ToString();
                        }
                        numCommands = input.Length;
                    }
                    break;
                case Stage.Proccessing:
                    try
                    {
                        Queue<char> commands = ReadPlayerInput(input);
                        inputMessage = "Passed";

                        CommandReadingComplete.Invoke(commands);

                        input += "~";
                        progressBar.FullAmount = commandOrder.Count;
                        curCommandNumber = 0;
                        commandOrder.Enqueue(input.Length - 1);
                        stage = Stage.Waiting;
                    }
                    catch (Exception e)
                    {
                        inputMessage = e.Message + " : Please press ENTER to try again.";
                        stage = Stage.Input;
                        inputFailed = true;
                    }

                    break;
                case Stage.Waiting:
                    break;
            }
        }

        public Queue<char> ReadPlayerInput(string input)
        {
            Queue<char> commands = new Queue<char>();
            LoopInfo loopInfo = new LoopInfo();
            commandOrder = new Queue<int>();
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
            screen.Draw(Helper.GetRectTexture(GameView.WIDTH, HEIGHT, Color.Black), new Rectangle(0, 0, GameView.WIDTH, HEIGHT));

            DrawOnLine($"Command: {input}", 0);

            DrawOnLine("Action       : ", 1);
            DrawOnLine(commandArrow, 1, (int)commandArrowXPosition);

            DrawOnLine($"Status: {inputMessage}", 2);
            DrawOnLine($"{numCommands}/{MAX_COMMANDS} Commands", 2, false);

            DrawOnLine(numKeysDisplay, 3);
            DrawOnLine(numGemsDisplay, 3, false);

            progressBar.Draw(screen);
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();

        public void StartInputProcess(string message, bool lastLevelComplete)
        {
            inputMessage = message;
            inputFailed = true;
            stage = Stage.Input;
            commandOrder = new Queue<int>();
            this.lastLevelComplete = lastLevelComplete;
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
            => screen.DrawText(Helper.InputFont, text, new Vector2(leftToRight ? margins.X : GameView.WIDTH - margins.X - Helper.InputFont.MeasureString(text).X, (Helper.InputFont.MeasureString("S").Y + lineSpacing) * lineNum + margins.Y), Color.White);

        private void DrawOnLine(string text, int lineNum, int xPosition)
            => screen.DrawText(Helper.InputFont, text, new Vector2(xPosition, (Helper.InputFont.MeasureString("S").Y + lineSpacing) * lineNum + margins.Y), Color.White);
    }
}
