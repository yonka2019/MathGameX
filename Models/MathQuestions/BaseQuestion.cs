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

        public abstract void GenerateQuestion();
        protected abstract char GetOperator();

        public string GetQuestionText()
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
            if (SettingsManager.Settings["digits"]["one"])
                DigitsPreferences.Add((0, 9));

            if (SettingsManager.Settings["digits"]["double"])
                DigitsPreferences.Add((10, 99));

            if (SettingsManager.Settings["digits"]["triple"])
                DigitsPreferences.Add((100, 999));

            if (SettingsManager.Settings["digits"]["fourth"])
                DigitsPreferences.Add((1000, 9999));
        }
    }
}