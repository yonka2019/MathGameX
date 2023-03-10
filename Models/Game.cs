using MathGame.Models;
using MathGame.Models.MathQuestions;
using System;
using System.Collections.Generic;

namespace MathGame
{
    internal class Game
    {
        private readonly List<BaseQuestion> QuestionTypes = new List<BaseQuestion>();

        private readonly GameDifficulty difficulty;

        public int AnswerTime { get; private set; }


        public Game(GameDifficulty gd)
        {
            difficulty = gd;
            SetOperatorsSettings();
            AnswerTime = GetAnswerTime();
        }

        public BaseQuestion GetRandomQuestionType()
        {
            int randomQuestionType = new Random().Next(0, QuestionTypes.Count);
            return QuestionTypes[randomQuestionType];
        }

        private void SetOperatorsSettings()
        {
            foreach (KeyValuePair<char, bool> op in SettingsManager.Settings["operators"])
            {
                if (op.Value)  // add all allowed (according the settings) question types (-, + ..)
                    QuestionTypes.Add(GetQuestionByOperator(op.Key));
            }
        }

        private int GetAnswerTime()
        {
            return difficulty switch
            {
                GameDifficulty.Infinity => 0,
                GameDifficulty.Easy => 15,
                GameDifficulty.Medium => 10,
                GameDifficulty.Hard => 5,
                _ => 0,
            };
        }

        private BaseQuestion GetQuestionByOperator(char op)
        {
            return op switch
            {
                '+' => new AdditionQuestion(),
                '-' => new SubstructionQuestion(),
                '*' => new MultiplicationQuestion(),
                '/' => new DivisionQuestion(),
                _ => null,
            };
        }
    }
}