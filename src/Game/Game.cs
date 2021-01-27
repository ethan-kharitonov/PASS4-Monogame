using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    static class Game
    {
        public class FinalResult
        {
            public int TotalScore = 0;
            public int TotalTime = 0;
            public LevelResult[] LevelResults = new LevelResult[LevelContainer.NUM_LEVELS];
        }

        public class LevelResult
        {
            public int Score;
            public int Time;

            public LevelResult(int score, int time)
            {
                Score = score;
                Time = time;
            }
        }

        public static FinalResult FinalGameResult = new FinalResult();

        private static Texture2D legend;

        public static event Action<int> AllLevelsComplete;

        private static ISection[] sections = new ISection[]
        {
            LevelContainer.Instance,
            InputMenu.Instance
        };

        public static void LoadContent()
        {
            LevelContainer.Instance.RunCompleteFailed += m => InputMenu.Instance.ShowResultsForRoundFailed(m);
            LevelContainer.Instance.RunCompleteSuccess += li => InputMenu.Instance.ShowResultsForRoundSuccess(li);

            LevelContainer.Instance.AllLevelsComplete += lr => InputMenu.Instance.ShowResultsAllLevelsComplete(lr);

            InputMenu.Instance.CommandReadingStarting += () => LevelContainer.Instance.ReStartLevel();

            InputMenu.Instance.CommandReadingComplete += q => LevelContainer.Instance.LoadCommands(q);
            LevelContainer.Instance.ExecutingNextCommand += () => InputMenu.Instance.ShowNextCommand();

            LevelContainer.Instance.KeysAndGemsCounted += (nk, ng) => InputMenu.Instance.SetNumKeys(nk, ng);
            LevelContainer.Instance.playerKeyCollected += nk => InputMenu.Instance.UpdateNumCollectedKeys(nk);
            LevelContainer.Instance.playerGemCollected += ng => InputMenu.Instance.UpdateNumCollectedGems(ng);

            InputMenu.Instance.PlayerReadyToExistMainGame += s => AllLevelsComplete.Invoke(s);


            foreach (ISection section in sections)
            {
                section.LoadContent();
            }

            legend = Helper.LoadImage("Images/command legend ethan");
        }

        public static void Update()
        {
            foreach (ISection section in sections)
            {
                section.Update();
            }
        }

        public static void Draw()
        {
            foreach (ISection section in sections)
            {
                section.Draw();
            }

            if (InputMenu.Instance.ShowLegend)
            {
                Helper.SpriteBatch.Draw(legend, Vector2.Zero, Color.White);
            }
        }
    }
}
