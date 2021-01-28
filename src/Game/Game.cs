//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Game.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Main game class. Connects Input menu and level container which are the main classes that run the game.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PASS4
{
    static class Game
    {

        //Stores the image for the command legend
        private static Texture2D legend;

        //Gets invoked when the game stage is complete
        public static event Action<int> AllLevelsComplete;

        //Stores the instances of the LevelContainer and InputMenu classes
        private static ISection[] sections = new ISection[]
        {
            LevelContainer.Instance,
            InputMenu.Instance
        };
        
        //Invoked when player presses X
        public static event Action BackToMenu;

        /// <summary>
        /// Resets the game stage
        /// called when game starts
        /// </summary>
        public static void Reset()
        {
            LevelContainer.Instance.Reset();
            InputMenu.Instance.Reset();
        }

        /// <summary>
        /// Loads all the content needed for the game stage
        /// </summary>
        public static void LoadContent()
        {
            //Link togther the LevelContainer and InputMenu class throug events

            //Level Events
            LevelContainer.Instance.RunCompleteFailed += m => InputMenu.Instance.ShowResultsForRoundFailed(m);
            LevelContainer.Instance.RunCompleteSuccess += li => InputMenu.Instance.ShowResultsForRoundSuccess(li);
            LevelContainer.Instance.AllLevelsComplete += lr => InputMenu.Instance.ShowResultsAllLevelsComplete(lr);


            //Command events
            InputMenu.Instance.CommandReadingStarting += () => LevelContainer.Instance.ReStartLevel();
            InputMenu.Instance.CommandReadingComplete += q => LevelContainer.Instance.LoadCommands(q);
            LevelContainer.Instance.ExecutingNextCommand += () => InputMenu.Instance.ShowNextCommand();

            //Collectable events
            LevelContainer.Instance.KeysAndGemsCounted += (nk, ng) => InputMenu.Instance.SetNumKeys(nk, ng);
            LevelContainer.Instance.playerKeyCollected += nk => InputMenu.Instance.UpdateNumCollectedKeys(nk);
            LevelContainer.Instance.playerGemCollected += ng => InputMenu.Instance.UpdateNumCollectedGems(ng);

            //leaving the game stage
            InputMenu.Instance.PlayerReadyToExistMainGame += s => AllLevelsComplete.Invoke(s);
            InputMenu.Instance.QuitGame += () => BackToMenu.Invoke();

             


            //Load content for LevelContainer and InputMenu
            foreach (ISection section in sections)
            {
                section.LoadContent();
            }

            //Load the legend image
            legend = Helper.LoadImage("Images/command legend ethan");
        }

        /// <summary>
        /// Update LevelContainer and InputMenu 
        /// </summary>
        public static void Update()
        {
            foreach (ISection section in sections)
            {
                section.Update();
            }
        }

        /// <summary>
        /// Draw the game stage
        /// </summary>
        public static void Draw()
        {
            //Draw LevelContainer and InputMenu
            foreach (ISection section in sections)
            {
                section.Draw();
            }

            //Draw the legend if needed
            if (InputMenu.Instance.ShowLegend)
            {
                Helper.SpriteBatch.Draw(legend, Vector2.Zero, Color.White);
            }
        }
    }
}
