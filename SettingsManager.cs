using System.Collections.Generic;

namespace MathGame
{
    public static class SettingsManager
    {
        /// <summary>
        /// DICT<DICT>:  gameSettings
        /// {
        ///     [string: (settings group)] = {
        ///         [string: (setting name)] = [bool: true/false]
        ///     }
        /// }
        /// 
        /// Example: gameSettings["operators"]["minus"] = true;
        /// </summary>
        private static Dictionary<string, Dictionary<string, bool>> gameSettings = new Dictionary<string, Dictionary<string, bool>>();

        static SettingsManager() // initialize dictionaries
        {
            gameSettings["operators"] = new Dictionary<string, bool> {
                { "plus", false }, { "minus", false }, { "multiply", false }, { "divide", false } };

            gameSettings["digits"] = new Dictionary<string, bool> {
                { "one", false }, { "double", false }, { "triple", false }, { "fourth", false } };
        }

        public static Dictionary<string, Dictionary<string, bool>> GameSettings
        {
            get { return gameSettings; }
            set { gameSettings = value; }
        }

    }
}