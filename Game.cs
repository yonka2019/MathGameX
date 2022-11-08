using System.Collections.Generic;
using System;

namespace MathGame
{
    internal class Game
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

        public Game()
        {

        }

        public static Dictionary<string, Dictionary<string, bool>> GameSettings
        {
            get { return gameSettings; }
            set { gameSettings = value; }
        }
    }
}