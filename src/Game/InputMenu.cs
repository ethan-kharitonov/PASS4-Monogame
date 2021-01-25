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

        private SpriteFont inputFont;

        private string input = string.Empty;
        private string inputMessage = string.Empty;
        private string commandArrow = string.Empty;
        private float commandArrowXPosition;
        private Queue<int> commandOrder = new Queue<int>();
        private int curCommandNumber = 0;

        private Bar progressBar;

        private List<Keys> keysPressedLastFrame = new List<Keys>();
        private List<Keys> keysReleasedThisFrame = new List<Keys>();

        private const int HEIGHT = 150;

        public delegate void TakeCommands(Queue<char> commands);
        public event TakeCommands CommandReadingComplete;

        public delegate void Notify();
        public event Notify CommandReadingStarting;
        
        private bool inputFailed = false;

        private bool showLegend = false;
        public bool ShowLegend => showLegend;

        private InputMenu()
        {
        }

        private Screen screen = new Screen(new Point(0, MainGame.HEIGHT), MainGame.WIDTH, HEIGHT);

        public void LoadContent()
        {
            inputFont = Helper.Content.Load<SpriteFont>("Fonts/InputFont");
            progressBar = new Bar(new Rectangle(10, 90, 200, 30));
        }

        public void Update()
        {
            GetReleasedKeys();

            switch (stage)
            {
                case Stage.Input:
                    if (keysReleasedThisFrame.Contains(Keys.L))
                    {
                        keysReleasedThisFrame.Remove(Keys.L);
                        showLegend = !showLegend;
                    }

                    if (inputFailed)
                    {
                        if(keysReleasedThisFrame.Contains(Keys.Enter))
                        {
                            keysReleasedThisFrame.Remove(Keys.Enter);
                            input = string.Empty;
                            inputMessage = string.Empty;
                            commandArrow = string.Empty;
                            inputFailed = false;
                            CommandReadingStarting.Invoke();
                            progressBar.Reset();
                        }
                        else
                        {
                            break;
                        }
                       
                    }

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
                    try
                    {
                        Queue<char> commands = ReadPlayerInput(input);
                        inputMessage = "Passed";

                        if (CommandReadingComplete != null)
                        {
                            CommandReadingComplete.Invoke(commands);
                        }
                        input += "~";
                        progressBar.FullAmount = commandOrder.Count;
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
            screen.Draw(Helper.GetRectTexture(MainGame.WIDTH, HEIGHT, Color.Black), new Rectangle(0, 0, MainGame.WIDTH, HEIGHT));
            screen.DrawText(inputFont, "Command: " + input, new Vector2(10, 10), Color.White);

            screen.DrawText(inputFont, "Action       : ", new Vector2(10, 10 + inputFont.MeasureString("S").Y + 5), Color.White);
            screen.DrawText(inputFont, commandArrow, new Vector2(commandArrowXPosition, 10 + inputFont.MeasureString("S").Y + 5), Color.White);

            screen.DrawText(inputFont, "Status: " + inputMessage, new Vector2(10, 10 + 3 * inputFont.MeasureString("S").Y + 5), Color.White);

            progressBar.Draw(screen);
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();

        public void StartInputProcess(string message)
        {
            inputMessage = message + " : Please press ENTER to try again.";
            inputFailed = true;
            stage = Stage.Input;
            commandOrder = new Queue<int>();
        }

        private void GetReleasedKeys()
        {
            List<Keys> keysPressedThisFrame = Keyboard.GetState().GetPressedKeys().ToList();
            keysReleasedThisFrame = keysPressedLastFrame.Where(k => !keysPressedThisFrame.Contains(k)).ToList();

            keysPressedLastFrame = keysPressedThisFrame;
        }

        public void ShowNextCommand()
        {
            if(commandArrow == string.Empty)
            {
                commandArrow = "^";
            }

            ++curCommandNumber;
            progressBar.Update(curCommandNumber);

            commandArrowXPosition = inputFont.MeasureString("Command:xx" + input.Substring(0, commandOrder.Dequeue())).X + 3;
        }
    }
}
