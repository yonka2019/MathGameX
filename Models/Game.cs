﻿using MathGame.Models;
using MathGame.Models.MathQuestions;
using System;
using System.Collections.Generic;

namespace MathGame
{
    internal class Game
    {
        private readonly List<BaseQuestion> QuestionTypes = new List<BaseQuestion>();

        private readonly GameMode difficulty;

        public BaseQuestion CurrentQuestion { get; private set; }

        public int AnswerTime { get; private set; }


        public Game(GameMode gd)
        {
            difficulty = gd;
            SetOperatorsSettings();
            AnswerTime = GetAnswerTime();
        }

        /// <summary>
        /// Returns randomized question type [ - | + | * | / ] (according the setted settings)
        /// </summary>
        /// <returns>randomized question object</returns>
        public BaseQuestion GetRandomQuestionType()
        {
            int randomQuestionType = new Random().Next(0, QuestionTypes.Count);
            CurrentQuestion = QuestionTypes[randomQuestionType];

            return CurrentQuestion;
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
                GameMode.Infinity => 60,
                GameMode.Easy => 15,
                GameMode.Medium => 10,
                GameMode.Hard => 5,
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