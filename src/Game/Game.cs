using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    static class Game
    {
        private static Texture2D legend;

        public static event Action AllLevelsComplete;

        private static ISection[] sections = new ISection[]
        {
            LevelContainer.Instance,
            InputMenu.Instance
        };

        public static void LoadContent()
        {
            LevelContainer.Instance.RunComplete += m => InputMenu.Instance.StartInputProcess(m);
            LevelContainer.Instance.AllLevelsComplete += () => InputMenu.Instance.WaitForEnterThenLeave();

            InputMenu.Instance.CommandReadingStarting += () => LevelContainer.Instance.ReStartLevel();

            InputMenu.Instance.CommandReadingComplete += q => LevelContainer.Instance.LoadCommands(q);
            LevelContainer.Instance.ExecutingNextCommand += () => InputMenu.Instance.ShowNextCommand();

            LevelContainer.Instance.KeysAndGemsCounted += (nk, ng) => InputMenu.Instance.SetNumKeys(nk, ng);
            LevelContainer.Instance.playerKeyCollected += nk => InputMenu.Instance.UpdateNumCollectedKeys(nk);
            LevelContainer.Instance.playerGemCollected += ng => InputMenu.Instance.UpdateNumCollectedGems(ng);

            InputMenu.Instance.PlayerReadyToExistMainGame += () => AllLevelsComplete.Invoke();




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
