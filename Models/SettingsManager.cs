using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private static Dictionary<string, Dictionary<char, bool>> gameSettings = new Dictionary<string, Dictionary<char, bool>>();

        static SettingsManager()  // initialize dictionaries
        {
            gameSettings["operators"] = new Dictionary<char, bool> {
                { '+', false }, { '-', false }, { '*', false }, { '/', false } };

            gameSettings["digits"] = new Dictionary<char, bool> {
                { '1', false }, { '2', false }, { '3', false }, { '4', false } };  // one, double, triple, fourth
        }

        public static Dictionary<string, Dictionary<char, bool>> Settings
        {
            get { return gameSettings; }
            set { gameSettings = value; }
        }

        /// <summary>
        /// Checks if the user allowed to run the game
        /// According the settings, the settings must have atleast one option enabled in the operators, and atleast one in the digits.
        /// </summary>
        public static bool ReadyToPlay
        {
            get
            {
                int trueCounters = 0; // should be 2 to allow game

                foreach (var value in gameSettings.Values) // [operators] & [digits]
                {
                    /*
                     * operators:
                     * one of the settings true +1 , leave loop, continue to check digits
                     * digits:
                     * one of the settings true + 1 , leave the loops
                     * = 2 [good]
                     */
                    foreach(var setting in value.Values)
                    {
                        if (setting)
                        {
                            trueCounters++;
                            break;
                        }
                    }
                }
                return trueCounters == 2;
            }
        }

    }
}