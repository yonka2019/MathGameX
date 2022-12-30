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
    internal class InfinityGame : BasicGame
    {
        public InfinityGame() : base(GameDifficulty.Infinity)
        {
        }
    }
}