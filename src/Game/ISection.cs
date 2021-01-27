//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: ISection.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: A component that takes up a section of the screen and needs to be updated, draw, etc. (Somewhat like a smaller game)

namespace PASS4
{
    interface ISection
    {
        /// <summary>
        /// Used to load all the content
        /// </summary>
        public void LoadContent();

        /// <summary>
        /// Should be called every frame to update everything
        /// </summary>
        public void Update();

        /// <summary>
        /// Should be called every frame to draw everything
        /// </summary>
        public void Draw();

        /// <summary>
        /// Gets the highest X value the section reaches
        /// </summary>
        /// <returns>The highest X value the section reaches</returns>
        public int GetMaxX();

        /// <summary>
        /// Gets the highest Y value the section reaches
        /// </summary>
        /// <returns>The highest Y value the section reaches</returns>
        public int GetMaxY();

    }
}
