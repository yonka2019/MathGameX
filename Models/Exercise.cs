using System;
using System.Collections.Generic;

namespace MathGame.Models
{
    internal class Exercise
    {
        private static readonly Random rnd = new Random();

        private readonly List<(int, int)> DigitsPreferences = new List<(int, int)>();
        private readonly List<char> OperatorsPreferences = new List<char>();

        private int operand1;
        private char eOperator;
        private int operand2;

        private double answer;

        internal string Generate()
        {
            DigitP();
            OperatorP();

            operand1 = GenerateNumber();
            eOperator = GenerateOperator();
            operand2 = GenerateNumber();

            answer = CalculateAnswer(operand1, eOperator, operand2);
            return $"{operand1} {eOperator} {operand2}";
        }

        /// <summary>
        /// Check if user answer correct (according the real answer)
        /// </summary>
        /// <param name="userAnswer">answer of the player</param>
        /// <returns>true if users' answer correct</returns>
        internal bool Check(int userAnswer)
        {
            return answer == userAnswer;
        }

        private double CalculateAnswer(int num1, char oper, int num2)
        {
            switch (oper)
            {
                case '+':
                    return num1 + num2;

                case '-':
                    return num1 - num2;

                case '/':
                    if (num2 == 0)
                        return 0;

                    return Math.Round((double)num1 / num2, 2);

                case '*':
                    return num1 * num2;

                default:
                    return 0;
            }
        }

        private int GenerateNumber()
        {
            int digitPref = rnd.Next(0, DigitsPreferences.Count);

            return rnd.Next(DigitsPreferences[digitPref].Item1, DigitsPreferences[digitPref].Item2);
        }

        private char GenerateOperator()
        {
            int operatorPref = rnd.Next(0, OperatorsPreferences.Count);

            return OperatorsPreferences[operatorPref];
        }

        /// <summary>
        /// initialize digit preferences
        /// </summary>
        private void DigitP()
        {
            if (SettingsManager.Settings["digits"]["one"])
                DigitsPreferences.Add((0, 9));

            if (SettingsManager.Settings["digits"]["double"])
                DigitsPreferences.Add((10, 99));

            if (SettingsManager.Settings["digits"]["triple"])
                DigitsPreferences.Add((100, 999));

            if (SettingsManager.Settings["digits"]["fourth"])
                DigitsPreferences.Add((1000, 9999));
        }

        /// <summary>
        /// initialize operator preferences
        /// </summary>
        private void OperatorP()
        {
            if (SettingsManager.Settings["operators"]["+"])
                OperatorsPreferences.Add('+');

            if (SettingsManager.Settings["operators"]["-"])
                OperatorsPreferences.Add('-');

            if (SettingsManager.Settings["operators"]["*"])
                OperatorsPreferences.Add('*');

            if (SettingsManager.Settings["operators"]["/"])
                OperatorsPreferences.Add('/');
        }
    }
}