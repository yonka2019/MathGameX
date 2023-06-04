using System;
using System.Collections.Generic;

namespace MathGame.Models.MathQuestions
{
    internal abstract class BaseQuestion
    {
        private readonly List<(int, int)> DigitsPreferences = new List<(int, int)>();
        private readonly Random random = new Random();

        public int Operand1 { get; protected set; }
        public int Operand2 { get; protected set; }
        public double Answer { get; protected set; }

        public BaseQuestion()
        {
            SetDigitSettings();
        }

        /// <summary>
        /// Generates random question and returns his type (which could be used with ToString in order to get question text)
        /// </summary>
        /// <returns></returns>
        public abstract BaseQuestion GenerateQuestion();

        /// <summary>
        ///  '+'  |  '-'  |  '/'  |  '*'
        /// </summary>
        /// <returns></returns>
        public abstract char GetOperator();

        /// <summary>
        /// return current question
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Operand1} {GetOperator()} {Operand2}";
        }

        public bool CheckAnswer(double userAnswer)
        {
            return Answer == userAnswer;
        }

        protected int GetRandomNumber()
        {
            int digitPref = random.Next(0, DigitsPreferences.Count);  // randomized any "digit option" (one-double-triple.. digit numbers)

            return random.Next(DigitsPreferences[digitPref].Item1, DigitsPreferences[digitPref].Item2);
        }

        private void SetDigitSettings()
        {
            if (GameSettingsManager.Settings["digits"]['1'])  // one
                DigitsPreferences.Add((0, 9));

            if (GameSettingsManager.Settings["digits"]['2'])  // double
                DigitsPreferences.Add((10, 99));

            if (GameSettingsManager.Settings["digits"]['3'])  // triple
                DigitsPreferences.Add((100, 999));

            if (GameSettingsManager.Settings["digits"]['4'])  // fourth
                DigitsPreferences.Add((1000, 9999));
        }
    }
}