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
            Proccessing,
            WaitingForEnter
        }

        /* private enum EnterAction
         {
             None,
             StartInput,
             GoToNameEntry
         }

         private EnterAction enterAction = EnterAction.None;*/

        private bool OnEnterGoToMenu = false;

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

        private bool inputReadingRestarting = false;

        private bool showLegend = false;
        public bool ShowLegend => showLegend;

        private string numKeysDisplay;
        private string numGemsDisplay;

        public event Action PlayerReadyToExistMainGame;

        private int numCommands = 0;
        private const int MAX_COMMANDS = 70;

        private Point margins = new Point(10, 10);
        private int lineSpacing = 5;

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

        private InputMenu()
        {
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
        }

        public void Update()
        {
            if (Helper.KeysReleasedThisFrame.Contains(Keys.L))
            {
                Helper.KeysReleasedThisFrame.Remove(Keys.L);
                showLegend = !showLegend;
            }
            switch (stage)
            {
                case Stage.WaitingForEnter:
                    if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                    {
                        stage = Stage.Input;

                        input = string.Empty;
                        inputMessage = string.Empty;
                        commandArrow = string.Empty;
                        CommandReadingStarting.Invoke();
                        progressBar.Reset();
                        numCommands = 0;
                        commandOrder.Clear();

                        if (OnEnterGoToMenu)
                        {
                            PlayerReadyToExistMainGame.Invoke();
                        }
                    }
                    break;

                case Stage.Input:
                    input = string.Concat(Helper.UpdateStringWithInput(input).Where(k => validChars.Contains(k) || isCharValidDigit(k)).Take(MAX_COMMANDS));

                    if (Helper.KeysReleasedThisFrame.Contains(Keys.Enter))
                    {
                        stage = Stage.Proccessing;
                    }

                    break;
                case Stage.Proccessing:
                    try 
                    {
                        Queue<char> commands = ConvertInputIntoCommands(input);
                        input += "~";
                        commands.Enqueue('~');
                        commandOrder.Enqueue(input.Length - 1);
                        numCommands = input.Length;


                        inputMessage = "Passed";

                        CommandReadingComplete.Invoke(commands);

                        progressBar.FullAmount = commandOrder.Count - 1;
                        curCommandNumber = 0;
                        stage = Stage.Waiting;
                    }
                    catch (Exception e)
                    {
                        inputMessage = e.Message + " : Please press ENTER to try again.";
                        stage = Stage.WaitingForEnter;
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
            screen.Draw(Helper.GetRectTexture(LevelContainer.WIDTH, HEIGHT, Color.Black), new Rectangle(0, 0, LevelContainer.WIDTH, HEIGHT));

            DrawOnLine($"Command: {input}", 0);

            DrawOnLine("Action       : ", 1);
            DrawOnLine(commandArrow, 1, (int)commandArrowXPosition);

            DrawOnLine($"Status: {inputMessage}", 2);
            DrawOnLine($"{numCommands}/{MAX_COMMANDS} Commands", 2, false);

            DrawOnLine(numKeysDisplay, 3);
            DrawOnLine(numGemsDisplay, 3, false);

            DrawOnLine($"Time: {LevelContainer.timer.ElapsedMilliseconds}", 4, false);


            progressBar.Draw(screen);
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();

        public void StartInputProcess(string message)
        {
            inputReadingRestarting = true;
            inputMessage = message;
            stage = Stage.WaitingForEnter;
        }

        public void WaitForEnterThenLeave()
        {
            stage = Stage.WaitingForEnter;
            OnEnterGoToMenu = true;

            inputMessage = "You beat all the levels! : Press ENTER to enter your name and save.";
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
            => screen.DrawText(Helper.InputFont, text, new Vector2(leftToRight ? margins.X : LevelContainer.WIDTH - margins.X - Helper.InputFont.MeasureString(text).X, (Helper.InputFont.MeasureString("S").Y + lineSpacing) * lineNum + margins.Y), Color.White);

        private void DrawOnLine(string text, int lineNum, int xPosition)
            => screen.DrawText(Helper.InputFont, text, new Vector2(xPosition, (Helper.InputFont.MeasureString("S").Y + lineSpacing) * lineNum + margins.Y), Color.White);
    }
}
