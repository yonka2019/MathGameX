using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathGame
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        private Game currentGame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.game_screen);
            // count down on screen 3 seconds before start

            GameDifficulty selectDifficulty = (GameDifficulty)Intent.GetIntExtra("mode", 0);

            currentGame = new Game(selectDifficulty);



        }
    }
}