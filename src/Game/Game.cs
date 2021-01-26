using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    static class Game
    {
        private static Texture2D legend;

        public static event Action AllLevelsComplete;

        private static ISection[] sections = new ISection[]
        {
            GameView.Instance,
            InputMenu.Instance
        };

        public static void LoadContent()
        {
            GameView.Instance.RunComplete += (m, l) => InputMenu.Instance.StartInputProcess(m, l);
            InputMenu.Instance.CommandReadingComplete += q => GameView.Instance.LoadCommands(q);

            GameView.Instance.ExecutingNextCommand += () => InputMenu.Instance.ShowNextCommand();

            InputMenu.Instance.CommandReadingStarting += () => GameView.Instance.ReStartLevel();

            GameView.Instance.KeysAndGemsCounted += (nk, ng) => InputMenu.Instance.SetNumKeys(nk, ng);
            GameView.Instance.playerKeyCollected += nk => InputMenu.Instance.UpdateNumCollectedKeys(nk);
            GameView.Instance.playerGemCollected += ng => InputMenu.Instance.UpdateNumCollectedGems(ng);

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
