using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathGame.Models
{
    internal class LevelGame : BasicGame
    {
        private int TimeSeconds { get; set; }  // (game seconds, total, not only to answer a question)

        public LevelGame(GameDifficulty gd, int timeSeconds) : base(gd)
        {
            TimeSeconds = timeSeconds;
        }
    }
}