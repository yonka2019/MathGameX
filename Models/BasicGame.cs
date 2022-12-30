using System.Collections.Generic;
using System;
using MathGame.Models;

namespace MathGame
{
    internal class BasicGame
    {
        protected GameDifficulty Difficulty { get; private set; }
        protected double Answer { get; private set; }

        public BasicGame(GameDifficulty gd)
        {
            Difficulty = gd;
        }

        protected string GenerateExercise()
        {
            Answer = 0;
            return "";
        }
    }
}