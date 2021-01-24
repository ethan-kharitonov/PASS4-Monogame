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

        private SpriteFont inputFont;

        private List<Keys> keysPressedLastFrame = new List<Keys>();
        private List<Keys> keysReleasedThisFrame = new List<Keys>();

        private const int HEIGHT = 150;

        public delegate void TakeCommands(Queue<char> commands);
        public event TakeCommands CommandReadingComplete;

        private bool inputFailed = false;

        private InputMenu()
        {

        }

        private Screen screen = new Screen(new Point(0, MainGame.HEIGHT), MainGame.WIDTH, HEIGHT);

        public void LoadContent()
        {
            inputFont = Helper.Content.Load<SpriteFont>("Fonts/InputFont");
        }

        public void Update()
        {
            GetReleasedKeys();

            switch (stage)
            {
                case Stage.Input:
                    for (int i = 0; i < keysReleasedThisFrame.Count; i++)
                    {
                        Keys key = keysReleasedThisFrame[i];
                        keysPressedLastFrame.Remove(key);
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
                            stage = Stage.Proccessing;
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
                    }
                    break;
                case Stage.Proccessing:
                    if (inputFailed && keysReleasedThisFrame.Contains(Keys.Enter))
                    {
                        keysPressedLastFrame.Remove(Keys.Enter);
                        StartInputProcess();
                        inputFailed = false;
                        break;
                    }

                    try
                    {
                        Queue<char> commands = ReadPlayerInput(input);
                        inputMessage = "Passed";

                        if (CommandReadingComplete != null)
                        {
                            CommandReadingComplete.Invoke(commands);
                        }
                        input += "|";
                        stage = Stage.Waiting;
                    }
                    catch (Exception e)
                    {
                        inputMessage = e.Message + " : Please press ENTER to try again.";
                        inputFailed = true;
                    }

                    break;
                case Stage.Waiting:
                    break;
            }
        }

        public static Queue<char> ReadPlayerInput(string input)
        {
            Queue<char> commands = new Queue<char>();
            LoopInfo loopInfo = new LoopInfo();
            for (int i = 0; i < input.Length; ++i)
            {
                if (i < input.Length && input[i] == 'F')
                {
                    if (loopInfo.StartIndex == -1)
                    {
                        throw new FormatException("Loops must start with 'S'");
                    }

                    if(i == loopInfo.StartIndex)
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
            screen.Draw(Helper.GetRectTexture(MainGame.WIDTH, HEIGHT, Color.Black), new Rectangle(0, 0, MainGame.WIDTH, HEIGHT));
            screen.DrawText(inputFont, "Command: " + input, new Vector2(10, 10), Color.White);
            screen.DrawText(inputFont, "Action       : " + commandArrow, new Vector2(10, 10 + inputFont.MeasureString("S").Y + 5), Color.White);
            screen.DrawText(inputFont, "Status: " + inputMessage, new Vector2(10, 10 + 3 * inputFont.MeasureString("S").Y + 5), Color.White);
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();

        public void StartInputProcess()
        {
            stage = Stage.Input;
            input = string.Empty;
            inputMessage = string.Empty;
        }

        private void GetReleasedKeys()
        {
            List<Keys> keysPressedThisFrame = Keyboard.GetState().GetPressedKeys().ToList();
            keysReleasedThisFrame = keysPressedLastFrame.Where(k => !keysPressedThisFrame.Contains(k)).ToList();

            keysPressedLastFrame = keysPressedThisFrame;
        }

        public void ShowNextCommand()
        {
            if(commandArrow.Length == 0)
            {
                commandArrow = "V";
            }
            else
            {
                commandArrow = commandArrow.Substring(0, commandArrow.Length - 1) + " V";
                float x = inputFont.MeasureString(" ").X;
                float y = inputFont.MeasureString("F").X;

            }
        }
    }
}
