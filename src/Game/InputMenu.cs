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

            private int startIndex = -1;
            private int numIterations;
            private LoopInfo nestedLoop;

            public int StartIndex { get => nestedLoop != null ? nestedLoop.StartIndex : startIndex; }

            public void StartLoop(int startIndex, int numIterations)
            {
                if(this.startIndex == -1)
                {
                    this.startIndex = startIndex;
                    this.numIterations = numIterations;
                }
                else
                {
                    if(nestedLoop == null)
                    {
                        nestedLoop = new LoopInfo();
                    }

                    nestedLoop.StartLoop(startIndex, numIterations);
                }
            }

            public bool DecrementNumIterations()
            {
                if(nestedLoop != null)
                {
                    if (nestedLoop.DecrementNumIterations())
                    {
                        return true;
                    }

                    nestedLoop = null;
                    return false;
                }

                if(numIterations == 0)
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
            Proccessing
        }

        private Stage stage = Stage.Input;
        public static readonly InputMenu Instance = new InputMenu();

        private string input = string.Empty;
        private string inputMessage = "STATUS: ";

        private List<Keys> keysPressedLastFrame = new List<Keys>();
        private SpriteFont inputFont;

        private const int HEIGHT = 150;

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
            switch (stage)
            {
                case Stage.Input:
                    Keys[] newKeys = Keyboard.GetState().GetPressedKeys();
                    keysPressedLastFrame = newKeys.Union(keysPressedLastFrame).ToList();

                    for (int i = 0; i < keysPressedLastFrame.Count; i++)
                    {
                        Keys key = keysPressedLastFrame[i];
                        if (!Keyboard.GetState().IsKeyDown(key))
                        {
                            keysPressedLastFrame.Remove(key);
                            if (key == Keys.Back)
                            {
                                if (input.Count() == 0)
                                {
                                    continue;
                                }

                                input = input.Substring(0, input.Count() - 1);
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
                    }
                    break;
                case Stage.Proccessing:

                    Queue<char> commands = ReadPlayerInput(input);


                    break;
            }
        }

        public static Queue<char> ReadPlayerInput(string input)
        {
            Queue<char> commands = new Queue<char>();
            LoopInfo loopInfo = new LoopInfo();
            for (int i = 0; i < input.Length; ++i)
            {
                if (i < input.Length && input[i] == 'F' && loopInfo.DecrementNumIterations())
                {
                    i = loopInfo.StartIndex - 1;
                }
                else if(input[i] == 'S')
                {
                    try
                    {
                        loopInfo.StartLoop(i + 2, Convert.ToInt32(input[i + 1]) - '0' - 1);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException("All loop starts (S) must be followed with a digit (1 - 9)");
                    }

                    i += 1;
                }
                else if (input[i] == 'A' || input[i] == 'C' || input[i] == 'D' || input[i] == 'E' || input[i] == 'Q')
                {
                    commands.Enqueue(input[i]);
                }
            }

            return commands;
        }

        public void Draw()
        {
            screen.Draw(Helper.GetRectTexture(MainGame.WIDTH, HEIGHT, Color.Black), new Rectangle(0, 0, MainGame.WIDTH, HEIGHT));
            screen.DrawText(inputFont, input, new Vector2(5, 5), Color.White);
            screen.DrawText(inputFont, inputMessage, new Vector2(5, 30), Color.White);
        }

        public int GetMaxX() => screen.GetMaxX();

        public int GetMaxY() => screen.GetMaxY();

    }
}
