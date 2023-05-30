using System.Collections.Generic;

namespace MathGame.Models
{
    public static class GameSettingsManager
    {
        static GameSettingsManager()  // initialize dictionaries
        {
            Settings["operators"] = new Dictionary<char, bool> {
                { '+', false }, { '-', false }, { '*', false }, { '/', false } };

            Settings["digits"] = new Dictionary<char, bool> {
                { '1', false }, { '2', false }, { '3', false }, { '4', false } };  // one, double, triple, fourth
        }

        public static Dictionary<string, Dictionary<char, bool>> Settings { get; set; } = new Dictionary<string, Dictionary<char, bool>>();

        /// <summary>
        /// Checks if the user allowed to run the game
        /// According the settings, the settings must have atleast one option enabled in the operators, and atleast one in the digits.
        /// </summary>
        public static bool ReadyToPlay
        {
            get
            {
                int trueCounters = 0; // should be 2 to allow game

                foreach (Dictionary<char, bool> value in Settings.Values) // [operators] & [digits]
                {
                    /*
                     * operators:
                     * one of the settings true +1 , leave loop, continue to check digits
                     * digits:
                     * one of the settings true + 1 , leave the loops
                     * = 2 [good]
                     */
                    foreach (bool setting in value.Values)
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